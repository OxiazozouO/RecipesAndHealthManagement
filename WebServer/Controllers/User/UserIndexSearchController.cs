using System.ComponentModel.DataAnnotations;
using AnyLibrary.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebServer.Controllers.Any;
using WebServer.Controllers.Service;
using WebServer.DatabaseModel;
using WebServer.Helper;
using WebServer.Http;

namespace WebServer.Controllers.User;

[Route("user/[controller]/[action]")]
[ApiController]
public class UserIndexSearchController(RecipeAndHealthSystemContext db, JwtHelper jwt) : MyControllerBase(db, jwt)
{
    //分页查找出所有食材
    [HttpPost]
    [Authorize(Roles = nameof(JwtType.User))]
    public IActionResult IndexIngredientList(UserIndexIngredientDto dto)
    {
        try
        {
            if (this.CheckUserRole(dto.Id.ToString(), out var result, out var user))
                return result;
            if (dto.Flag != 0 && dto.Flag != 1)
            {
                return ApiResponses.Error("参数错误");
            }

            var list = from f in Db.Favorites
                join fi in Db.FavoriteItems on f.FavoriteId equals fi.FavoriteId
                where f.IdCategory == IdCategory.Ingredient
                group fi by fi.TId
                into g
                select new
                {
                    g.Key,
                    Count = g.Count() == null ? 0 : g.Count()
                };
            var baseQuery = from i in Db.Ingredients
                where i.Status == Status.On
                join u in Db.Users.DefaultIfEmpty() on i.UserId equals u.UserId
                join c in list.DefaultIfEmpty() on i.IngredientId equals c.Key into joinedC
                from subI in joinedC.DefaultIfEmpty()
                select new
                {
                    i.IngredientId,
                    i.IName,
                    Nutritional = (from ii in db.IngredientNutritionals
                        where ii.IngredientId == i.IngredientId
                        join n in db.Nutrients on ii.NutritionalId equals n.Id
                        select new
                        {
                            n.Name,
                            Value = (decimal)ii.Value
                        }).ToList(),
                    i.UserId,
                    AuthorUName = u.UName,
                    ModifyDate = i.ModifyDate,
                    AuthorFileUrl = u.FileUrl,
                    FileUrl = i.FileUrl,
                    Favorites = (from fi in db.FavoriteItems
                        join f in db.Favorites on fi.FavoriteId equals f.FavoriteId
                        where fi.TId == i.IngredientId && f.IdCategory == IdCategory.Ingredient
                        group new { f.UserId, fi.TId } by new { f.UserId, fi.TId }
                        into g
                        select g.Key.UserId).ToList()
                };

            var query = (dto.Flag == 0
                    ? baseQuery.OrderByDescending(i => i.ModifyDate).ThenBy(i => i.IngredientId)
                    : baseQuery.OrderByDescending(i => i.Favorites.Count).ThenBy(i => i.IngredientId))
                .Skip((dto.PageIndex - 1) * 10)
                .Take(10)
                .AsEnumerable()
                .Select(i => new
                {
                    Ingredient = new
                    {
                        i.IngredientId,
                        i.IName,
                        Nutritional = i.Nutritional.ToDictionary(n => n.Name, n => n.Value),
                        FileUrl = Url.GetIngredientUrl(Request, i.FileUrl),
                        FavoriteCount = i.Favorites.Count,
                        IsLike = i.Favorites.Any(x => x == user.UserId)
                    },
                    User = new
                    {
                        i.UserId,
                        i.AuthorUName,
                        i.ModifyDate,
                        AuthorFileUrl = Url.GetUserUrl(Request, i.AuthorFileUrl)
                    }
                }).ToList();

            return ApiResponses.Success("获取成功", query);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    //分页查找出所有食材
    [HttpPost]
    [Authorize(Roles = nameof(JwtType.User))]
    public IActionResult IndexSearchIngredientList(UserIndexSearchIngredientDto dto)
    {
        try
        {
            if (this.CheckUserRole(dto.Id.ToString(), out var result, out var user))
                return result;

            if (dto.Flag != 0 && dto.Flag != 1)
                return ApiResponses.Error("参数错误");

            var baseQuery =
                from i in Db.Ingredients
                where i.Status == Status.On
                from ci in
                    Db.CategoryItems
                        .Where(x => x.TId == i.IngredientId && x.IdCategory == IdCategory.Ingredient)
                        .DefaultIfEmpty() // 左连接分类项
                from c in Db.Categories
                    .Where(x => x.CategoryId == ci.CategoryId)
                    .DefaultIfEmpty() // 左连接分类
                from u in Db.Users
                    .Where(x => x.UserId == i.UserId)
                    .DefaultIfEmpty() // 左连接用户
                let favorites = Db.FavoriteItems
                    .Where(fi => fi.TId == i.IngredientId)
                    .Join(Db.Favorites.Where(f => f.IdCategory == IdCategory.Ingredient),
                        fi => fi.FavoriteId,
                        f => f.FavoriteId,
                        (fi, f) => f.UserId)
                    .Distinct()
                    .ToList()
                where string.IsNullOrEmpty(dto.Search)
                      || EF.Functions.Like(i.IName, $"%{dto.Search}%")
                      || EF.Functions.Like(i.Allergy, $"%{dto.Search}%")
                      || EF.Functions.Like(i.Quantity, $"%{dto.Search}%")
                      || EF.Functions.Like(i.Refer, $"%{dto.Search}%")
                      || (c != null && EF.Functions.Like(c.CName, $"%{dto.Search}%"))
                select new
                {
                    i.IngredientId,
                    i.IName,
                    Nutritional = Db.IngredientNutritionals
                        .Where(ii => ii.IngredientId == i.IngredientId)
                        .Join(Db.Nutrients,
                            ii => ii.NutritionalId,
                            n => n.Id,
                            (ii, n) => new { n.Name, Value = (decimal)ii.Value })
                        .ToList(),
                    i.FileUrl,
                    i.UserId,
                    AuthorUName = u.UName,
                    ModifyDate = i.ModifyDate,
                    AuthorFileUrl = u.FileUrl,
                    Favorites = favorites
                };

            // 排序
            var orderedQuery = dto.Flag switch
            {
                0 => baseQuery.OrderByDescending(i => i.ModifyDate).ThenBy(i => i.IngredientId),
                1 => baseQuery.OrderByDescending(i => i.Favorites.Count).ThenBy(i => i.IngredientId),
                _ => throw new ArgumentException("参数错误")
            };

            // 分页
            var results = orderedQuery
                .Skip((dto.PageIndex - 1) * 10)
                .Take(10)
                .AsEnumerable()
                .GroupBy(x => x.IngredientId)
                .Select(g => g.First())
                .Select(i => new
                {
                    Ingredient = new
                    {
                        i.IngredientId,
                        i.IName,
                        Nutritional = i.Nutritional.ToDictionary(n => n.Name, n => n.Value),
                        FileUrl = Url.GetIngredientUrl(Request, i.FileUrl),
                        FavoriteCount = i.Favorites.Count,
                        IsLike = i.Favorites.Contains(user.UserId)
                    },
                    User = new
                    {
                        i.UserId,
                        i.AuthorUName,
                        i.ModifyDate,
                        AuthorFileUrl = Url.GetUserUrl(Request, i.AuthorFileUrl)
                    }
                })
                .ToList();

            return ApiResponses.Success("获取成功", results);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return ApiResponses.ErrorResult;
        }
    }

    //分页查找出所有食谱
    [HttpPost]
    [Authorize(Roles = nameof(JwtType.User))]
    public IActionResult IndexRecipeList(UserIndexDto dto)
    {
        try
        {
            if (this.CheckUserRole(dto.Id.ToString(), out var result, out var user))
                return result;
            var query = (from r in Db.Recipes
                    where r.Status == Status.On
                    join u in Db.Users.DefaultIfEmpty() on r.AuthorId equals u.UserId
                    orderby r.RecipeId
                    select new
                    {
                        r.RecipeId,
                        r.AuthorId,
                        r.Title,
                        FileUrl = Url.GetRecipeUrl(Request, r.FileUrl),
                        r.Summary,
                        r.ModifyDate,
                        AuthorUName = u.UName,
                        AuthorFileUrl = Url.GetUserUrl(Request, u.FileUrl),
                        Favorites = (from fi in db.FavoriteItems
                            join f in db.Favorites on fi.FavoriteId equals f.FavoriteId
                            where fi.TId == r.RecipeId && f.IdCategory == IdCategory.Recipe
                            group new { f.UserId, fi.TId } by new { f.UserId, fi.TId }
                            into g
                            select g.Key.UserId).ToList()
                    })
                .Skip((dto.PageIndex - 1) * 10)
                .Take(10)
                .ToList()
                .Select(r => new
                {
                    r.RecipeId,
                    r.AuthorId,
                    r.Title,
                    r.FileUrl,
                    r.Summary,
                    r.ModifyDate,
                    r.AuthorUName,
                    r.AuthorFileUrl,
                    FavoriteCount = r.Favorites.Count,
                    IsLike = r.Favorites.Any(x => x == user.UserId)
                })
                .ToList();


            return ApiResponses.Success("获取成功", query);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    //分页查找出所有食谱
    [HttpPost]
    [Authorize(Roles = nameof(JwtType.User))]
    public IActionResult IndexSearchRecipeList(UserIndexSearchDto dto)
    {
        try
        {
            if (this.CheckUserRole(dto.Id.ToString(), out var result, out var user))
                return result;

            var queryBase = from r in Db.Recipes
                where r.Status == Status.On
                join u in Db.Users on r.AuthorId equals u.UserId into userJoin
                from u in userJoin.DefaultIfEmpty()
                join ci in Db.CategoryItems on r.RecipeId equals ci.TId into ciJoin
                from ci in ciJoin.Where(x => x.IdCategory == IdCategory.Recipe).DefaultIfEmpty()
                join c in Db.Categories on ci.CategoryId equals c.CategoryId into cJoin
                from c in cJoin.DefaultIfEmpty()
                where string.IsNullOrEmpty(dto.Search)
                      || EF.Functions.Like(r.Title, $"%{dto.Search}%")
                      || EF.Functions.Like(r.Summary, $"%{dto.Search}%")
                      || EF.Functions.Like(r.RName, $"%{dto.Search}%")
                      || EF.Functions.Like(c.CName, $"%{dto.Search}%")
                orderby r.RecipeId
                select new
                {
                    r.RecipeId,
                    r.AuthorId,
                    r.Title,
                    FileUrl = Url.GetRecipeUrl(Request, r.FileUrl),
                    r.Summary,
                    r.ModifyDate,
                    AuthorUName = u.UName,
                    AuthorFileUrl = Url.GetUserUrl(Request, u.FileUrl),
                    CategoryName = c.CName,
                    Favorites = (from fi in Db.FavoriteItems
                        join f in Db.Favorites on fi.FavoriteId equals f.FavoriteId
                        where fi.TId == r.RecipeId && f.IdCategory == IdCategory.Recipe
                        select f.UserId).ToList()
                };

            var query = queryBase
                .GroupBy(x => x.RecipeId) // 按食谱ID分组去重
                .Select(group => group.First())
                .Skip((dto.PageIndex - 1) * 10)
                .Take(10)
                .AsEnumerable()
                .Select(r => new
                {
                    r.RecipeId,
                    r.AuthorId,
                    r.Title,
                    r.FileUrl,
                    r.Summary,
                    r.ModifyDate,
                    r.AuthorUName,
                    r.AuthorFileUrl,
                    FavoriteCount = r.Favorites.Count,
                    IsLike = r.Favorites.Contains(user.UserId)
                })
                .ToList();

            return ApiResponses.Success("获取成功", query);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return ApiResponses.ErrorResult;
        }
    }


    //分页查找出所有食谱
    [HttpPost]
    [Authorize(Roles = nameof(JwtType.User))]
    public IActionResult IndexCollectionList(UserIndexDto dto)
    {
        try
        {
            if (this.CheckUserRole(dto.Id.ToString(), out var result, out var user))
                return result;
            var query = (from r in Db.Collections
                    where r.Status == Status.On
                    join u in Db.Users.DefaultIfEmpty() on r.UserId equals u.UserId
                    orderby r.CollectionId
                    select new
                    {
                        r.CollectionId,
                        r.UserId,
                        r.Title,
                        FileUrl = Url.GetCollectionUrl(Request, r.FileUrl),
                        r.Summary,
                        r.ModifyDate,
                        AuthorUName = u.UName,
                        AuthorFileUrl = Url.GetUserUrl(Request, u.FileUrl),
                        Favorites = (from fi in db.FavoriteItems
                            join f in db.Favorites on fi.FavoriteId equals f.FavoriteId
                            where fi.TId == r.CollectionId && f.IdCategory == IdCategory.Collection
                            group new { f.UserId, fi.TId } by new { f.UserId, fi.TId }
                            into g
                            select g.Key.UserId).ToList()
                    })
                .Skip((dto.PageIndex - 1) * 10)
                .Take(10)
                .ToList()
                .Select(r => new
                {
                    Collection = new
                    {
                        r.CollectionId,
                        r.FileUrl,
                        Refer = r.Summary,
                        r.Title,
                        FavoriteCount = r.Favorites.Count,
                        IsLike = r.Favorites.Any(x => x == user.UserId)
                    },
                    User = new
                    {
                        r.UserId,
                        r.AuthorUName,
                        r.AuthorFileUrl,
                        r.ModifyDate
                    }
                })
                .ToList();

            return ApiResponses.Success("获取成功", query);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    //分页查找出所有食谱
    [HttpPost]
    [Authorize(Roles = nameof(JwtType.User))]
    public IActionResult IndexSearchCollectionList(UserIndexSearchDto dto)
    {
        try
        {
            if (this.CheckUserRole(dto.Id.ToString(), out var result, out var user))
                return result;

            var queryBase = from r in Db.Collections
                where r.Status == Status.On
                join u in Db.Users on r.UserId equals u.UserId into userJoin
                from u in userJoin.DefaultIfEmpty()
                join ci in Db.CategoryItems on r.CollectionId equals ci.TId into ciJoin
                from ci in ciJoin.Where(x => x.IdCategory == IdCategory.Recipe).DefaultIfEmpty()
                join c in Db.Categories on ci.CategoryId equals c.CategoryId into cJoin
                from c in cJoin.DefaultIfEmpty()
                where string.IsNullOrEmpty(dto.Search)
                      || EF.Functions.Like(r.Title, $"%{dto.Search}%")
                      || EF.Functions.Like(r.Summary, $"%{dto.Search}%")
                      || EF.Functions.Like(c.CName, $"%{dto.Search}%")
                orderby r.CollectionId
                select new
                {
                    r.CollectionId,
                    r.UserId,
                    r.Title,
                    FileUrl = Url.GetRecipeUrl(Request, r.FileUrl),
                    r.Summary,
                    r.ModifyDate,
                    AuthorUName = u.UName,
                    AuthorFileUrl = Url.GetUserUrl(Request, u.FileUrl),
                    CategoryName = c.CName,
                    Favorites = (from fi in Db.FavoriteItems
                        join f in Db.Favorites on fi.FavoriteId equals f.FavoriteId
                        where fi.TId == r.CollectionId && f.IdCategory == IdCategory.Collection
                        select f.UserId).ToList()
                };

            var query = queryBase
                .GroupBy(x => x.CollectionId) // 按食谱ID分组去重
                .Select(group => group.First())
                .Skip((dto.PageIndex - 1) * 10)
                .Take(10)
                .AsEnumerable()
                .Select(r => new
                {
                    Collection = new
                    {
                        r.CollectionId,
                        r.FileUrl,
                        Refer = r.Summary,
                        r.Title,
                        FavoriteCount = r.Favorites.Count,
                        IsLike = r.Favorites.Any(x => x == user.UserId)
                    },
                    User = new
                    {
                        r.UserId,
                        r.AuthorUName,
                        r.AuthorFileUrl,
                        r.ModifyDate
                    }
                })
                .ToList();

            return ApiResponses.Success("获取成功", query);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return ApiResponses.ErrorResult;
        }
    }


    [HttpPost]
    [Authorize(Roles = nameof(JwtType.User))]
    public IActionResult IndexSearchBaseInfo(UserIndexSearchBaseInfoDto dto)
    {
        try
        {
            if (this.CheckUserRole(dto.Id.ToString(), out var result, out var user))
                return result;

            object? res = null;

            switch (dto.Flag)
            {
                case IdCategory.Ingredient:
                    res = (from i in Db.Ingredients
                        where i.IngredientId == dto.TId && i.Status == Status.On
                        select new
                        {
                            IdCategory = dto.Flag,
                            Id = i.IngredientId,
                            FileUrl = Url.GetIngredientUrl(Request, i.FileUrl),
                            Title = i.IName,
                            Refer = i.Refer
                        }).FirstOrDefault();
                    break;
                case IdCategory.Recipe:
                    res = (from r in Db.Recipes
                        where r.RecipeId == dto.TId && r.Status == Status.On
                        select new
                        {
                            IdCategory = dto.Flag,
                            Id = r.RecipeId,
                            FileUrl = Url.GetRecipeUrl(Request, r.FileUrl),
                            Title = r.RName,
                            Refer = r.Summary
                        }).FirstOrDefault();
                    break;
                case IdCategory.Collection:
                    res = (from c in Db.Collections
                        where c.CollectionId == dto.TId && c.Status == Status.On
                        select new
                        {
                            IdCategory = dto.Flag,
                            Id = c.CollectionId,
                            FileUrl = Url.GetCollectionUrl(Request, c.FileUrl),
                            Title = c.Title,
                            Refer = c.Summary
                        }).FirstOrDefault();
                    break;
                default:
                    return ApiResponses.Error("参数错误");
            }

            return res is null
                ? ApiResponses.Error("没有找到相关数据")
                : ApiResponses.Success("获取成功", res);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }
}

public record UserIndexDto
{
    [Required] public int Id { set; get; }

    [Range(1, int.MaxValue - 2, ErrorMessage = "请求错误")]
    public int PageIndex { set; get; }
}

public record UserIndexSearchDto
{
    [Required] public int Id { set; get; }

    [Range(1, int.MaxValue - 2, ErrorMessage = "请求错误")]
    public int PageIndex { set; get; }

    [Required]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "搜索内容长度不能小于{1},大于{2}")]
    public string Search { set; get; }
}

public record UserIndexIngredientDto
{
    [Required] public int Id { set; get; }

    [Range(0, int.MaxValue - 2, ErrorMessage = "请求错误")]
    public int Flag { set; get; }

    [Range(1, int.MaxValue - 2, ErrorMessage = "请求错误")]
    public int PageIndex { set; get; }
}

public record UserIndexSearchIngredientDto
{
    [Required] public int Id { set; get; }

    [Range(0, int.MaxValue - 2, ErrorMessage = "请求错误")]
    public int Flag { set; get; }

    [Range(1, int.MaxValue - 2, ErrorMessage = "请求错误")]
    public int PageIndex { set; get; }

    [Required]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "搜索内容长度不能小于{1},大于{2}")]
    public string Search { set; get; }
}

public record UserIndexSearchBaseInfoDto
{
    [Required] public int Id { set; get; }

    [Range(0, int.MaxValue - 2, ErrorMessage = "请求错误")]
    public int Flag { set; get; }

    [Range(1, int.MaxValue - 2, ErrorMessage = "请求错误")]
    public int TId { set; get; }
}