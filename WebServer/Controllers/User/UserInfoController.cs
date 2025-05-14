using System.ComponentModel.DataAnnotations;
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
public class UserInfoController(RecipeAndHealthSystemContext db, JwtHelper jwt) : MyControllerBase(db, jwt)
{
    [HttpPost]
    [Authorize(Roles = nameof(JwtType.User))]
    public IActionResult MyAllInfo(UserSelectionDto dto)
    {
        try
        {
            if (this.CheckUserRole(dto.Id.ToString(), out var result, out var user))
                return result;

            var data = (from info in Db.PhysicalSignsRecords
                where info.UId == user.UserId
                orderby info.CreateDate descending
                select new
                {
                    info.UpiId,
                    info.Height,
                    info.Weight,
                    ActivityLevel = new
                    {
                        info.Cal.Id,
                        Name = info.Cal.Key,
                        info.Cal.Value
                    },
                    info.ProteinPercentage,
                    info.FatPercentage,
                    info.CreateDate
                }).ToList();

            return ApiResponses.Success("获取用户信息成功", data);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.User))]
    public IActionResult AddMyInfo(UserInfoAddDto dto)
    {
        try
        {
            if (this.CheckUserRole(dto.Id.ToString(), out var result, out var user))
                return result;

            var info = Db.PhysicalSignsRecords.FirstOrDefault(u => u.CreateDate.Date == DateTime.Now.Date);
            if (info is null)
            {
                info = new PhysicalSignsRecord
                {
                    UId = user.UserId,
                    Height = dto.Height,
                    Weight = dto.Weight,
                    CalId = dto.CalId,
                    ProteinPercentage = dto.ProteinPercentage,
                    FatPercentage = dto.FatPercentage,
                    CreateDate = DateTime.Now
                };
                Db.PhysicalSignsRecords.Add(info);
            }
            else
            {
                info.Height = dto.Height;
                info.Weight = dto.Weight;
                info.CalId = dto.CalId;
                info.ProteinPercentage = dto.ProteinPercentage;
                info.FatPercentage = dto.FatPercentage;
                Db.PhysicalSignsRecords.Update(info);
            }

            return Db.SaveChanges() == 1
                ? ApiResponses.Success("添加成功")
                : ApiResponses.Error("添加失败");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }
}

public record UserSelectionDto
{
    [Range(1, long.MaxValue - 2, ErrorMessage = "请求错误")]
    public long Id { get; set; }
}

public record UserInfoAddDto
{
    [Range(1, long.MaxValue - 2, ErrorMessage = "请求错误")]
    public long Id { get; set; }

    [Required(ErrorMessage = "参数错误")]
    [Range(120, 255, ErrorMessage = "身高只能在{1}cm~{2}cm之间")]
    public double Height { get; set; }

    [Required(ErrorMessage = "参数错误")]
    [Range(30, 255, ErrorMessage = "体重只能在{1}kg~{2}kg之间")]
    public double Weight { get; set; }

    [Required(ErrorMessage = "参数错误")]
    [Range(1, int.MaxValue - 2, ErrorMessage = "活动强度参数错误")]
    public int CalId { get; set; }

    [Required(ErrorMessage = "参数错误")]
    [Range(0.1, 0.2, ErrorMessage = "蛋白质摄入量必须在{1}%~{2}%之间")]
    public double ProteinPercentage { get; set; }

    [Required(ErrorMessage = "参数错误")]
    [Range(0.2, 0.3, ErrorMessage = "脂肪摄入量必须在{1}%~{2}%之间")]
    public double FatPercentage { get; set; }
}