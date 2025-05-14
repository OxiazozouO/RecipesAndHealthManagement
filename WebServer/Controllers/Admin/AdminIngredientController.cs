using System.ComponentModel.DataAnnotations;
using AnyLibrary.Constants;
using AnyLibrary.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebServer.Controllers.Any;
using WebServer.Controllers.Service;
using WebServer.DatabaseModel;
using WebServer.DTOs;
using WebServer.Helper;
using WebServer.Http;

namespace WebServer.Controllers.Admin;

[Route("admin/[controller]/[action]")]
[ApiController]
public class AdminIngredientController(RecipeAndHealthSystemContext db, JwtHelper jwt) : MyControllerBase(db, jwt)
{
    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult GetIngredients(AdminDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;

            var result = (from i in Db.Ingredients
                join u in Db.Users on i.UserId equals u.UserId
                select new
                {
                    i.IngredientId,
                    UFileUrl = Url.AdminGetUserUrl(Request, u.FileUrl),
                    FileUrl = Url.AdminGetIngredientUrl(Request, i.FileUrl),
                    UName = u.UName,
                    i.IName,
                    i.Refer,
                    i.Unit,
                    i.Quantity,
                    i.Allergy,
                    i.Content,
                    i.Status,
                    FavoriteCount = (from fi in db.FavoriteItems
                        join f in db.Favorites on fi.FavoriteId equals f.FavoriteId
                        where fi.TId == i.IngredientId && f.IdCategory == IdCategory.Ingredient
                        select f.UserId).Count()
                }).ToList();

            return ApiResponses.Success("请求成功", result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult GetIngredient(AdminUserDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;

            var i = Db.Ingredients.FirstOrDefault(i => i.IngredientId == dto.Id);
            if (i is null) return ApiResponses.Error("食材不存在");

            var list = (from inu in Db.IngredientNutritionals
                where inu.IngredientId == i.IngredientId
                join n in Db.Nutrients on inu.NutritionalId equals n.Id
                select new
                {
                    n.Id,
                    n.Name,
                    n.Unit,
                    Value = inu.Value
                }).ToList();

            var result = new
            {
                i.IngredientId,
                FileUrl = Url.AdminGetIngredientUrl(Request, i.FileUrl),
                i.IName,
                i.Refer,
                i.Unit,
                Quantity = i.Quantity.ToEntity<Dictionary<string, decimal>>(),
                i.Allergy,
                i.Content,
                Nutritional = list,
            };

            return ApiResponses.Success("请求成功", result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult GetNutrients(AdminDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;

            var result = Db.Nutrients.Select(i => new
            {
                i.Id,
                i.Name,
                i.Unit
            }).ToList();
            return ApiResponses.Success("请求成功", result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult AddIngredient(AddIngredientDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var res, out var admin))
                return res;
            var editor = new EditorContext
            {
                Db = Db,
                Url = Url,
                ReleaseId = -1,
                OpId = dto.AdminId,
                OpFlag = UserTypes.Admin,
                TargetType = IdCategory.Ingredient,
                Info = null,
                I = dto.Ingredient,
            };

            if (editor.EditorEnt(out res, out var fun))
                return res;

            return Db.TransactionScope(fun, "添加成功，请等待管理员审核", "添加失败");
        }
        catch (Exception ex)
        {
            return ApiResponses.ErrorResult;
        }
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult EditIngredient(EditIngredientDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var res, out var admin))
                return res;

            var editor = new EditorContext
            {
                Db = Db,
                Url = Url,
                ReleaseId = dto.ReleaseId < 0 ? -1 : dto.ReleaseId,
                OpId = dto.AdminId,
                OpFlag = UserTypes.Admin,
                TargetType = IdCategory.Ingredient,
                Info = null,
                I = dto.Ingredient,
            };

            if (editor.EditorEnt(out res, out var fun))
                return res;

            return Db.TransactionScope(fun, "提交成功，请等待管理员审核", "提交失败");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult ReverseIngredientStatus(StatusDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;
            return Db.ReverseIngredientStatus(dto.Id, dto.Status, UserTypes.Admin);
        }
        catch (Exception ex)
        {
            return ApiResponses.ErrorResult;
        }
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult DeleteIngredient(AdminUserDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;

            var ingredient = Db.Ingredients.FirstOrDefault(i => i.IngredientId == dto.Id);
            if (ingredient is null) return ApiResponses.Error("食谱不存在");
            if (ingredient.Status is not Status.ForceOff)
                return ApiResponses.Error("该食材状态不许删除");
            ingredient.Status = Status.Deleted;
            Db.Ingredients.Update(ingredient);
            return Db.SaveChanges() == 1
                ? ApiResponses.Success("修改成功")
                : ApiResponses.Error("修改失败");
        }
        catch (Exception ex)
        {
            return ApiResponses.ErrorResult;
        }
    }
}

public record EditIngredientDto : AdminDto
{
    [Range(-1, int.MaxValue - 2, ErrorMessage = "请求错误")]
    [Required(ErrorMessage = "申请id不能为空")]
    public int ReleaseId { get; set; }

    [Required(ErrorMessage = "需审核的数据不能为空")]
    public AddIngredientModel Ingredient { get; set; }
}

public record AddIngredientDto : AdminDto
{
    public AddIngredientModel Ingredient { get; set; }
}

public record StatusDto : AdminUserDto
{
    [Range(0, int.MaxValue - 2, ErrorMessage = "请求错误")]
    public int Status { get; set; }
}