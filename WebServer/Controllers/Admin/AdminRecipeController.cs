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
public class AdminRecipeController(RecipeAndHealthSystemContext db, JwtHelper jwt) : MyControllerBase(db, jwt)
{
    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult GetRecipes(AdminDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;

            var recipe = (from r in Db.Recipes
                join u in Db.Users on r.AuthorId equals u.UserId
                select new
                {
                    UFileUrl = Url.AdminGetUserUrl(Request, u.FileUrl),
                    u.UName,
                    r.RecipeId,
                    r.Title,
                    r.RName,
                    FileUrl = Url.AdminGetRecipeUrl(Request, r.FileUrl),
                    r.Summary,
                    r.Status,
                    FavoriteCount = (from fi in db.FavoriteItems
                        join f in db.Favorites on fi.FavoriteId equals f.FavoriteId
                        where fi.TId == r.RecipeId && f.IdCategory == IdCategory.Recipe
                        select f.UserId).Count()
                }).ToList();

            return ApiResponses.Success("获取食谱详细信息成功", recipe);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult GetRecipe(AdminUserDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;

            var recipe = (from r in Db.Recipes
                where r.RecipeId == dto.Id
                select new
                {
                    r.RecipeId,
                    r.Title,
                    r.RName,
                    FileUrl = Url.AdminGetRecipeUrl(Request, r.FileUrl),
                    r.Summary
                }).FirstOrDefault();

            if (recipe == null)
                return ApiResponses.Error("没有找到此食谱");

            var steps = Db.PreparationSteps
                .Where(step => step.RecipeId == dto.Id)
                .OrderBy(step => step.SequenceNumber)
                .Select(step => new
                {
                    Id = step.PreparationStepId,
                    step.Title,
                    FileUrl = Url.AdminGetRecipeUrl(Request, step.FileUrl),
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
                        i.Unit,
                        i.Quantity,
                        i.FileUrl,
                        item.Dosage,
                        Nutritional = (from ii in db.IngredientNutritionals
                            where ii.IngredientId == i.IngredientId
                            join n in db.Nutrients on ii.NutritionalId equals n.Id
                            select new
                            {
                                n.Id,
                                n.Name,
                                Value = (decimal)ii.Value,
                                n.Unit
                            }).ToList()
                    })
                .AsEnumerable()
                .Select(x => new
                {
                    x.IngredientId,
                    x.IName,
                    x.Unit,
                    Nutritional = x.Nutritional.ToDictionary(n => n.Id, n => n),
                    FileUrl = Url.AdminGetIngredientUrl(Request, x.FileUrl),
                    x.Dosage
                })
                .ToList();

            var categories = (from ci in db.CategoryItems
                join c in db.Categories on ci.CategoryId equals c.CategoryId
                where ci.UserId == UserRoles.CategoryAdmin
                      && ci.IdCategory == IdCategory.Ingredient
                      && ci.TId == dto.Id
                select c.CName).ToList();

            return ApiResponses.Success("获取食谱详细信息成功", new
            {
                recipe.RecipeId,
                recipe.Title,
                recipe.RName,
                recipe.FileUrl,
                recipe.Summary,
                Steps = steps,
                Ingredients = ingredients,
                Categories = categories
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult GetRecipeIngredients(RecipeIngredientsDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;

            var ingredients = (from i in Db.Ingredients
                    where dto.Ids.Contains(i.IngredientId)
                    select new
                    {
                        i.IngredientId,
                        i.IName,
                        i.Unit,
                        i.FileUrl,
                        Nutritional = (from ii in db.IngredientNutritionals
                            where ii.IngredientId == i.IngredientId
                            join n in db.Nutrients on ii.NutritionalId equals n.Id
                            select new
                            {
                                n.Id,
                                n.Name,
                                Value = (decimal)ii.Value,
                                n.Unit
                            }).ToList()
                    })
                .AsEnumerable()
                .Select(x => new
                {
                    x.IngredientId,
                    x.IName,
                    x.Unit,
                    Nutritional = x.Nutritional.ToDictionary(n => n.Id, n => n),
                    FileUrl = Url.AdminGetIngredientUrl(Request, x.FileUrl),
                    Dosage = 100
                })
                .ToList();

            return ApiResponses.Success("获取食谱详细信息成功", ingredients);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult EditRecipe(EditRecipeDto dto)
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
                TargetType = IdCategory.Recipe,
                Info = null,
                R = dto.Recipe,
            };

            if (editor.EditorEnt(out res, out var fun))
                return res;

            return Db.TransactionScope(fun, "添加成功，请等待管理员审核", "添加失败");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.Error("提交失败");
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult AddRecipe(EditRecipeDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var res, out var admin))
                return res;
            var recipe = dto.Recipe;
            if (Url.TestRecipe(recipe, out res))
                return res;

            var release = new Release
            {
                AuthorId = dto.AdminId,
                OpFlag = UserTypes.Admin,
                TId = -1,
                IdCategory = IdCategory.Recipe,
                Content = recipe.ToJson(),
                ReleaseInfo = "后台自动添加",
                Title = recipe.Title,
                FileUrl = recipe.FileUrl,
                CreateDate = DateTime.Now,
            };


            return Db.TransactionScope(() =>
            {
                Db.Releases.Add(release);
                Db.SaveChanges();
                var flowHistory = new ReleaseFlowHistory
                {
                    ReleaseId = release.ReleaseId,
                    OpId = dto.AdminId,
                    CreateDate = DateTime.Now,
                    OpFlag = UserTypes.Admin,
                    Info = "新增食谱",
                    Status = Status.Pending
                };
                Db.ReleaseFlowHistories.Add(flowHistory);
            }, "添加成功", "添加失败");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.Error("提交失败");
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult ReverseRecipeStatus(StatusDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;
            return Db.ReverseRecipeStatus(dto.Id, dto.Status, UserTypes.Admin);
        }
        catch (Exception ex)
        {
            return ApiResponses.ErrorResult;
        }
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult DeleteRecipe(AdminUserDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;

            var recipe = Db.Recipes.FirstOrDefault(i => i.RecipeId == dto.Id);
            if (recipe is null) return ApiResponses.Error("食谱不存在");
            if (recipe.Status is not Status.ForceOff)
                return ApiResponses.Error("该食谱状态不许删除");
            recipe.Status = Status.Deleted;
            Db.Recipes.Update(recipe);
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

public record RecipeIngredientsDto : AdminDto
{
    [Required(ErrorMessage = "请求错误")] public HashSet<long> Ids { set; get; }
}

public record EditRecipeDto : AdminDto
{
    [Range(-1, int.MaxValue - 2, ErrorMessage = "请求错误")]
    [Required(ErrorMessage = "申请id不能为空")]
    public int ReleaseId { get; set; }

    [Required(ErrorMessage = "食谱是必需的")] public AddRecipeModel Recipe { get; set; }
}