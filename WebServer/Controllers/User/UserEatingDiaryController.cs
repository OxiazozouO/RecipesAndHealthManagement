using System.ComponentModel.DataAnnotations;
using AnyLibrary.Constants;
using AnyLibrary.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebServer.Controllers.Any;
using WebServer.Controllers.Service;
using WebServer.DatabaseModel;
using WebServer.Helper;
using WebServer.Http;

namespace WebServer.Controllers.User;

[Route("user/[controller]/[action]")]
[ApiController]
public class UserEatingDiaryController(RecipeAndHealthSystemContext db, JwtHelper jwt) : MyControllerBase(db, jwt)
{
    [HttpPost]
    [Authorize(Roles = nameof(JwtType.User))]
    public IActionResult AddEatingDiary(EatingDiaryAddDto dto)
    {
        try
        {
            if (this.CheckUserRole(dto.Id.ToString(), out var result, out var user))
                return result;

            //时间是[生日, 生日+100年]
            if (dto.UpdateTime < user.BirthDate || dto.UpdateTime > user.BirthDate.AddYears(100))
                return ApiResponses.Error("选择的时间错误");

            var dietary = new DietaryRecord
            {
                UserId = user.UserId,
                IdCategory = dto.Flag,
                TieUpDate = dto.UpdateTime,
                Dosages = dto.Dosages.ToJson(),
                NutrientContent = dto.Nutrients.ToJson()
            };
            if (dto.TId is not null)
                dietary.Tid = dto.TId.Value;

            Db.DietaryRecords.Add(dietary);
            return Db.SaveChanges() == 1
                ? ApiResponses.Success("添加饮食记录成功")
                : ApiResponses.Error("添加饮食记录失败");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.User))]
    public IActionResult DeleteEatingDiary(EatingDiaryDeleteDto dto)
    {
        try
        {
            if (this.CheckUserRole(dto.Id.ToString(), out var result, out var user))
                return result;

            var eatingDiaries = Db.DietaryRecords.FirstOrDefault(ed =>
                ed.IdCategory == dto.Flag && ed.UserId == user.UserId && ed.EdId == dto.EdId);
            if (eatingDiaries is null) return ApiResponses.Error("饮食记录不存在");

            Db.DietaryRecords.Remove(eatingDiaries);
            return Db.SaveChanges() == 1 ? ApiResponses.Success("删除饮食记录成功") : ApiResponses.Error("删除食用日记失败");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.User))]
    public IActionResult GetEatingDiaries(EatingDiaryDto dto)
    {
        try
        {
            if (this.CheckUserRole(dto.Id.ToString(), out var result, out var user))
                return result;

            // 计算本周周一的日期
            int daysSinceMonday = ((int)dto.Time.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
            var start = dto.Time.AddDays(-daysSinceMonday).Date;
            var end = dto.Time.AddDays(7).Date;

            var data = (from ed in Db.DietaryRecords
                    where ed.UserId == user.UserId && ed.TieUpDate >= start && ed.TieUpDate < end
                    select new
                    {
                        ed.EdId,
                        ed.Tid,
                        ed.TieUpDate,
                        Flag = ed.IdCategory,
                        Info = ed.IdCategory == IdCategory.Ingredient
                            ? (from i in Db.Ingredients
                                where i.IngredientId == ed.Tid
                                select new
                                {
                                    Name = i.IName,
                                    FileUrl = Url.GetIngredientUrl(Request, i.FileUrl)
                                }).FirstOrDefault()
                            : ed.IdCategory == IdCategory.Recipe
                                ? (from r in Db.Recipes
                                    where r.RecipeId == ed.Tid
                                    select new
                                    {
                                        Name = r.RName,
                                        FileUrl = Url.GetRecipeUrl(Request, r.FileUrl)
                                    }).FirstOrDefault()
                                : null,
                        ed.Dosages,
                        Nutrients = ed.NutrientContent
                    })
                .Select(x => new
                {
                    x.EdId,
                    x.Tid,
                    x.TieUpDate,
                    x.Flag,
                    x.Info.Name,
                    x.Info.FileUrl,
                    x.Dosages,
                    x.Nutrients
                }).ToList();

            return ApiResponses.Success("饮食记录获取成功", data);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }
}

public record EatingDiaryDto
{
    [Required]
    [Range(1, long.MaxValue - 2, ErrorMessage = "请求错误")]
    public long Id { get; set; }

    [Required] public DateTime Time { get; set; }
}

public record EatingDiaryAddDto
{
    [Range(1, int.MaxValue - 2, ErrorMessage = "请求错误")]
    public long Id { get; set; }

    [Required]
    [Range(1, 254, ErrorMessage = "请求错误")]
    public sbyte Flag { get; set; }

    [Required] public Dictionary<long, decimal> Dosages { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "营养素列表不能为空")]
    public Dictionary<string, decimal> Nutrients { get; set; }

    [Required] public DateTime UpdateTime { get; set; }

    [Required] public long? TId { get; set; }
}

public record EatingDiaryDeleteDto
{
    [Range(1, long.MaxValue - 2, ErrorMessage = "请求错误")]
    public long Id { get; set; }

    [Required]
    [Range(1, 254, ErrorMessage = "请求错误")]
    public byte Flag { get; set; }

    [Required]
    [Range(1, 254, ErrorMessage = "请求错误")]
    public int EdId { get; set; }
}