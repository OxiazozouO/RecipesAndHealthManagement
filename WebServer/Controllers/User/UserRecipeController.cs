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
public class UserRecipeController(RecipeAndHealthSystemContext db, JwtHelper jwt) : MyControllerBase(db, jwt)
{
    [HttpPost]
    [Authorize(Roles = nameof(JwtType.User))]
    public IActionResult GetRecipe(GetRecipeDto dto)
    {
        try
        {
            if (this.CheckUserRole(dto.UserId.ToString(), out var res, out var user))
                return res;

            FavoriteItemsService.GetLikeCount(db, user.UserId, dto.Id, IdCategory.Recipe,
                out var FavoriteCount,
                out var IsLike
            );

            var recipe = (from r in Db.Recipes
                where r.RecipeId == dto.Id
                select new
                {
                    r.RecipeId,
                    r.Title,
                    r.RName,
                    FileUrl = Url.GetRecipeUrl(Request, r.FileUrl),
                    r.Summary
                }).FirstOrDefault();

            if (recipe == null)
                return ApiResponses.Error("没有找到此食谱");

            var categories = CategoriesService
                .GetCategories(Db, dto.UserId, dto.Id, IdCategory.Recipe);

            var steps = Db.PreparationSteps
                .Where(step => step.RecipeId == dto.Id)
                .OrderBy(step => step.SequenceNumber)
                .Select(step => new
                {
                    step.Title,
                    FileUrl = Url.GetRecipeUrl(Request, step.FileUrl),
                    step.Refer,
                    step.RequiredTime,
                    step.RequiredIngredient,
                    step.Summary
                }).ToList();

            var ingredients = (from item in Db.RecipeItems
                    join i in Db.Ingredients on item.IngredientId equals i.IngredientId
                    where item.RecipeId == dto.Id
                    select new
                    {
                        i.IngredientId,
                        i.IName,
                        i.Refer,
                        i.Unit,
                        i.Quantity,
                        i.Allergy,
                        i.Content,
                        i.FileUrl,
                        item.Dosage,
                        Nutritional = (from ii in db.IngredientNutritionals
                            where ii.IngredientId == i.IngredientId
                            join n in db.Nutrients on ii.NutritionalId equals n.Id
                            select new
                            {
                                n.Name,
                                Value = (decimal)ii.Value
                            }).ToList()
                    })
                .AsEnumerable()
                .Select(x => new
                {
                    x.IngredientId,
                    x.IName,
                    x.Refer,
                    x.Unit,
                    Quantity = x.Quantity.ToEntity<Dictionary<string, decimal>>(),
                    Nutritional = x.Nutritional.ToDictionary(n => n.Name, n => n.Value),
                    x.Allergy,
                    x.Content,
                    FileUrl = Url.GetIngredientUrl(Request, x.FileUrl),
                    x.Dosage
                })
                .ToList();

            return ApiResponses.Success("获取食谱详细信息成功", new
            {
                recipe.RecipeId,
                recipe.Title,
                recipe.RName,
                recipe.FileUrl,
                recipe.Summary,
                FavoriteCount,
                IsLike,
                Category = categories,
                Steps = steps,
                Ingredients = ingredients
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }
}

public record GetRecipeDto
{
    [Required]
    [Range(1, long.MaxValue - 2, ErrorMessage = "请求错误")]
    public long UserId { set; get; }

    [Required]
    [Range(1, long.MaxValue - 2, ErrorMessage = "请求错误")]
    public long Id { set; get; }
}