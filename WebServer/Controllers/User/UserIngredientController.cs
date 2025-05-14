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
public class UserIngredientController(RecipeAndHealthSystemContext db, JwtHelper jwt) : MyControllerBase(db, jwt)
{
    [HttpPost]
    [Authorize(Roles = nameof(JwtType.User))]
    public IActionResult GetIngredient(GetIngredientDto dto)
    {
        try
        {
            if (this.CheckUserRole(dto.UserId.ToString(), out var result, out var user))
                return result;

            var i = Db.Ingredients.FirstOrDefault(i => i.IngredientId == dto.Id);
            if (i == null)
                return ApiResponses.Error("该食材不存在");


            var categories = CategoriesService
                .GetCategories(Db, dto.UserId, dto.Id, IdCategory.Ingredient);

            FavoriteItemsService.GetLikeCount(db, user.UserId, dto.Id, IdCategory.Ingredient,
                out var favoriteCount, out var isLike);

            return ApiResponses.Success("获取成功", new
            {
                Category = categories,
                Ingredient = new
                {
                    i.IngredientId,
                    i.IName,
                    i.Refer,
                    i.Unit,
                    Quantity = i.Quantity?.ToEntity<Dictionary<string, decimal>>(),
                    Nutritional = (from ii in db.IngredientNutritionals
                        where ii.IngredientId == i.IngredientId
                        join n in db.Nutrients on ii.NutritionalId equals n.Id
                        select new
                        {
                            n.Name,
                            Value = (decimal)ii.Value
                        }).ToList().ToDictionary(n => n.Name, n => n.Value),
                    i.Allergy,
                    i.Content,
                    Dosage = 100,
                    FileUrl = Url.GetIngredientUrl(Request, i.FileUrl),
                    FavoriteCount = favoriteCount,
                    IsLike = isLike
                }
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.User))]
    public IActionResult GetIngredientInfo(GetIngredientDto dto)
    {
        try
        {
            if (this.CheckUserRole(dto.UserId.ToString(), out var result, out var user))
                return result;

            var i = Db.Ingredients.FirstOrDefault(i => i.IngredientId == dto.Id);
            if (i == null)
                return ApiResponses.Error("该食材不存在");

            return ApiResponses.Success("获取成功", new
            {
                i.IngredientId,
                i.IName,
                i.Refer,
                i.Unit,
                Quantity = i.Quantity.ToEntity<Dictionary<string, decimal>>(),
                Nutritional = (from ii in db.IngredientNutritionals
                    where ii.IngredientId == i.IngredientId
                    join n in db.Nutrients on ii.NutritionalId equals n.Id
                    select new
                    {
                        n.Name,
                        Value = (decimal)ii.Value
                    }).ToList().ToDictionary(n => n.Name, n => n.Value),
                i.Allergy,
                i.Content,
                FileUrl = Url.GetIngredientUrl(Request, i.FileUrl),
                Dosage = 100
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }
}

public record GetIngredientDto
{
    [Required]
    [Range(1, long.MaxValue - 2, ErrorMessage = "请求错误")]
    public long UserId { set; get; }

    [Required]
    [Range(1, long.MaxValue - 2, ErrorMessage = "请求错误")]
    public long Id { set; get; }
}