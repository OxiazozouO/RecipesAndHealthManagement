using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
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
public class AdminCategoryController(RecipeAndHealthSystemContext db, JwtHelper jwt) : MyControllerBase(db, jwt)
{
    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult GetCategories(AdminDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;

            var result = (from c in Db.Categories
                join ci in Db.CategoryItems on c.CategoryId equals ci.CategoryId into gj
                from subCi in gj.DefaultIfEmpty()
                group new { c, ci = subCi } by c
                into g
                select new
                {
                    Id = g.Key.CategoryId,
                    Name = g.Key.CName,
                    Type = g.Key.TypeId,
                    Categories = (from cci in g
                        where cci.ci != null
                        group cci.ci by cci.ci.IdCategory
                        into gg
                        select new
                        {
                            IdCategory = gg.Key,
                            Count = gg.Count()
                        }).ToList()
                }).ToList();

            return ApiResponses.Success("获取食谱详细信息成功", result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult GetCategoryIngredients(AdminDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;

            var result = (from i in Db.Ingredients
                select new
                {
                    i.IngredientId,
                    FileUrl = Url.AdminGetIngredientUrl(Request, i.FileUrl),
                    i.IName,
                    i.Refer,
                    i.Status,
                    Categories = (from ci in db.CategoryItems
                        join c in db.Categories on ci.CategoryId equals c.CategoryId
                        where ci.IdCategory == IdCategory.Ingredient && ci.TId == i.IngredientId
                        group c by c.CategoryId
                        into g
                        select new
                        {
                            Id = g.Key,
                            Name = g.First().CName,
                            Count = g.Count(),
                            g.First().TypeId,
                            isLike = db.CategoryItems.Any(f => f.UserId == UserRoles.CategoryAdmin
                                                               && f.IdCategory == IdCategory.Ingredient
                                                               && f.TId == i.IngredientId
                                                               && f.CategoryId == g.Key)
                        }).ToList()
                }).ToList();

            return ApiResponses.Success("获取食材详细信息成功", result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult GetCategoryRecipes(AdminDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;

            var result = (from r in Db.Recipes
                select new
                {
                    r.RecipeId,
                    r.Title,
                    r.RName,
                    FileUrl = Url.AdminGetRecipeUrl(Request, r.FileUrl),
                    r.Summary,
                    r.Status,
                    Categories = (from ci in db.CategoryItems
                        join c in db.Categories on ci.CategoryId equals c.CategoryId
                        where ci.IdCategory == IdCategory.Recipe && ci.TId == r.RecipeId
                        group c by c.CategoryId
                        into g
                        select new
                        {
                            Id = g.Key,
                            Name = g.First().CName,
                            Count = g.Count(),
                            g.First().TypeId,
                            isLike = db.CategoryItems.Any(f => f.UserId == UserRoles.CategoryAdmin
                                                               && f.IdCategory == IdCategory.Recipe
                                                               && f.TId == r.RecipeId
                                                               && f.CategoryId == g.Key)
                        }).ToList()
                }).ToList();

            return ApiResponses.Success("获取食谱详细信息成功", result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult GetCategoryCollections(AdminDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;

            var result = (from r in Db.Collections
                select new
                {
                    r.CollectionId,
                    FileUrl = Url.AdminGetCollectionUrl(Request, r.FileUrl),
                    r.Title,
                    r.Summary,
                    r.Status,
                    Categories = (from ci in db.CategoryItems
                        join c in db.Categories on ci.CategoryId equals c.CategoryId
                        where ci.IdCategory == IdCategory.Collection && ci.TId == r.CollectionId
                        group c by c.CategoryId
                        into g
                        select new
                        {
                            Id = g.Key,
                            Name = g.First().CName,
                            Count = g.Count(),
                            g.First().TypeId,
                            isLike = db.CategoryItems.Any(f => f.UserId == UserRoles.CategoryAdmin
                                                               && f.IdCategory == IdCategory.Collection
                                                               && f.TId == r.CollectionId
                                                               && f.CategoryId == g.Key)
                        }).ToList()
                }).ToList();

            return ApiResponses.Success("获取食谱详细信息成功", result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult AddCategory(AddCategoryDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;

            if (dto.Type == CategoryType.Emoji && !new Regex(RegexHelper.Emoji).Match(dto.Name).Success)
                return ApiResponses.Error("添加表情失败，这不是emoji");

            var category = Db.Categories.FirstOrDefault(f => f.CName == dto.Name && f.TypeId == dto.Type);
            if (category != null)
                return ApiResponses.Error("添加分类失败，该分类已存在");

            Db.Categories.Add(new Category
            {
                CName = dto.Name,
                TypeId = dto.Type
            });
            return Db.SaveChanges() == 1 ? ApiResponses.Success("添加分类成功") : ApiResponses.Error("添加分类失败");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult RemoveCategory(AdminUserDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;

            var category = Db.Categories.FirstOrDefault(f => f.CategoryId == dto.Id);
            if (category == null)
                return ApiResponses.Error("删除分类失败，该分类不存在");
            var categoryItems = Db.CategoryItems.Where(f => f.CategoryId == dto.Id).ToList();


            return Db.TransactionScope(() =>
            {
                Db.CategoryItems.RemoveRange(categoryItems);
                Db.Categories.Remove(category);
            }, "删除成功", "删除失败");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult EditCategoryName(EditCategoryNameDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;

            var category = Db.Categories.FirstOrDefault(f => f.CategoryId == dto.Id);
            if (category == null)
                return ApiResponses.Error("删除分类失败，该分类不存在");

            category.CName = dto.Name;

            Db.Categories.Update(category);

            return Db.SaveChanges() == 1 ? ApiResponses.Success("修改成功") : ApiResponses.Error("修改失败");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }


    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult GetIngredientCategoryItems(AdminUserDto dto)
    {
        return GetCategoryItems(dto, IdCategory.Ingredient);
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult GetRecipeCategoryItems(AdminUserDto dto)
    {
        return GetCategoryItems(dto, IdCategory.Recipe);
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult GetCollectionCategoryItems(AdminUserDto dto)
    {
        return GetCategoryItems(dto, IdCategory.Collection);
    }


    private IActionResult GetCategoryItems(AdminUserDto dto, sbyte idCategory)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;

            var categories = CategoriesService
                .GetCategories(Db, UserRoles.CategoryAdmin, dto.Id, idCategory);
            var hashSet = categories.Select(c => c.Id).ToHashSet();

            var result = (from c in Db.Categories
                where !hashSet.Contains(c.CategoryId)
                select new CategoriesDto
                {
                    Id = c.CategoryId,
                    Name = c.CName,
                    Count = 0,
                    TypeId = c.TypeId,
                    IsLike = false
                }).ToList();

            categories.AddRange(result);

            return ApiResponses.Success("获取食谱详细信息成功", categories);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult UpdateIngredientCategoryItems(UpdateCategoryDto dto)
    {
        return UpdateCategoryItems(dto, IdCategory.Ingredient);
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult UpdateRecipeCategoryItems(UpdateCategoryDto dto)
    {
        return UpdateCategoryItems(dto, IdCategory.Recipe);
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult UpdateCollectionCategoryItems(UpdateCategoryDto dto)
    {
        return UpdateCategoryItems(dto, IdCategory.Collection);
    }

    private IActionResult UpdateCategoryItems(UpdateCategoryDto dto, sbyte idCategory)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;
            var fun = Db.UpdateCategoryItems(dto.Id, UserRoles.CategoryAdmin, dto.Items, idCategory);
            return Db.TransactionScope(fun, "修改成功", "修改失败");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }
}

public record AddCategoryDto : AdminDto
{
    [Required(ErrorMessage = "分类名称不能为空")]
    [MinLength(1, ErrorMessage = "分类名称长度不能小于{1}")]
    [MaxLength(30, ErrorMessage = "分类名称长度不能大于{1}")]
    public string Name { get; set; }

    [Required(ErrorMessage = "分类类型不能为空")]
    [Range(1, sbyte.MaxValue - 2, ErrorMessage = "请求错误")]
    public sbyte Type { get; set; }
}

public record EditCategoryNameDto : AdminUserDto
{
    [Required(ErrorMessage = "分类名称不能为空")]
    [MinLength(1, ErrorMessage = "分类名称长度不能小于{1}")]
    [MaxLength(30, ErrorMessage = "分类名称长度不能大于{1}")]
    public string Name { get; set; }
}

public record UpdateCategoryDto : AdminUserDto
{
    [Required(ErrorMessage = "请求错误")]
    public Dictionary<long, bool> Items { get; set; }
}