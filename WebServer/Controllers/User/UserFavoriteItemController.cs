using System.ComponentModel.DataAnnotations;
using AnyLibrary.Constants;
using Microsoft.AspNetCore.Mvc;
using WebServer.Controllers.Any;
using WebServer.Controllers.Service;
using WebServer.DatabaseModel;
using WebServer.Helper;
using WebServer.Http;

namespace WebServer.Controllers.User;

[Route("user/[controller]/[action]")]
[ApiController]
public class UserFavoriteItemController(RecipeAndHealthSystemContext db, JwtHelper jwt) : MyControllerBase(db, jwt)
{
    [HttpPost]
    public IActionResult AddFavoriteItems(FavoriteItemCreationDto dto)
    {
        if (this.CheckUserRole(dto.Id.ToString(), out var result, out var user))
            return result;

        try
        {
            var favoriteItems = Db.FavoriteItems
                .Where(fi => dto.FavoriteIds.Contains(fi.FavoriteId) && fi.TId == dto.TId)
                .Select(fi => fi.FavoriteId)
                .ToList();

            if (favoriteItems.Count == dto.FavoriteIds.Count)
                return ApiResponses.Error("已存在");
            foreach (var item in favoriteItems)
            {
                dto.FavoriteIds.Remove(item);
            }

            if (dto.FavoriteIds.Count == 0) return ApiResponses.ErrorResult;

            var favorites = Db.Favorites
                .Where(f => dto.FavoriteIds.Contains(f.FavoriteId) && f.UserId == user.UserId)
                .ToList();
            if (favorites.Count == 0)
                return ApiResponses.Error("收藏夹不存在");

            if (favorites.Any(f => f.IdCategory != dto.Flag))
                return ApiResponses.Error("收藏夹类型不符");

            var list = favorites
                .Select(favorite => new FavoriteItem
                {
                    FavoriteId = favorite.FavoriteId, TId = dto.TId
                })
                .ToList();

            foreach (var favorite in favorites)
                favorite.ModifyDate = DateTime.Now;

            Db.Favorites.UpdateRange(favorites);
            Db.FavoriteItems.AddRange(list);

            return Db.SaveChanges() == list.Count + favorites.Count
                ? ApiResponses.Success("成功添加到收藏夹", 1)
                : ApiResponses.Error("添加失败");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    public IActionResult RemoveFavoriteItemsById(FavoriteItemSeleteDto dto)
    {
        if (this.CheckUserRole(dto.Id.ToString(), out var result, out var user))
            return result;

        try
        {
            var fis = (from fi in Db.FavoriteItems
                join f in Db.Favorites on fi.FavoriteId equals f.FavoriteId
                where f.UserId == user.UserId && f.IdCategory == dto.Flag && fi.TId == dto.TId
                select fi).ToList();

            if (fis is null || fis.Count == 0)
                return ApiResponses.Error("无收藏");

            Db.FavoriteItems.RemoveRange(fis);

            return Db.SaveChanges() == fis.Count
                ? ApiResponses.Success("已删除", -1)
                : ApiResponses.Error("删除失败");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    public IActionResult RemoveFavoriteItems(FavoriteItemRemoveDto dto)
    {
        if (this.CheckUserRole(dto.Id.ToString(), out var result, out var user))
            return result;

        try
        {
            var favorite = Db.Favorites
                .FirstOrDefault(f => dto.FavoriteId == f.FavoriteId && f.UserId == user.UserId);
            if (favorite is null)
                return ApiResponses.Error("收藏夹不存在");

            var favoriteItem = Db.FavoriteItems
                .Where(fi => dto.FavoriteId == fi.FavoriteId && dto.TIds.Contains(fi.TId))
                .ToList();

            if (favoriteItem is null)
                return ApiResponses.Error("无此收藏");

            Db.FavoriteItems.RemoveRange(favoriteItem);
            return Db.SaveChanges() == favoriteItem.Count
                ? ApiResponses.Success("已删除")
                : ApiResponses.Error("删除失败");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    public IActionResult GetFavoriteItems(FavoriteListItemDto dto)
    {
        if (this.CheckUserRole(dto.Id.ToString(), out var result, out var user))
            return result;

        try
        {
            var favorite = Db.Favorites
                .FirstOrDefault(f => f.FavoriteId == dto.FavoriteId);
            if (favorite is null)
                return ApiResponses.Error("收藏夹不存在");

            object data = null;
            switch (favorite.IdCategory)
            {
                case IdCategory.Ingredient:

                    var list1 = from f in Db.Favorites
                        join fi in Db.FavoriteItems on f.FavoriteId equals fi.FavoriteId
                        where f.IdCategory == IdCategory.Ingredient
                        group fi by fi.TId
                        into g
                        select new
                        {
                            g.Key,
                            Count = g.Count() == null ? 0 : g.Count()
                        };

                    var query1 = (from fi in Db.FavoriteItems
                        where fi.FavoriteId == favorite.FavoriteId
                        join i in Db.Ingredients on fi.TId equals i.IngredientId
                        join u in Db.Users.DefaultIfEmpty() on i.UserId equals u.UserId
                        join c in list1.DefaultIfEmpty() on i.IngredientId equals c.Key into joinedC
                        from subI in joinedC.DefaultIfEmpty()
                        orderby i.ModifyDate descending, i.IngredientId
                        select new
                        {
                            FavoriteItem = new
                            {
                                FileUrl = Url.GetIngredientUrl(Request, i.FileUrl),
                                Id = i.IngredientId,
                                Name = i.IName,
                                i.Refer,
                                Nutrients = new List<dynamic>
                                {
                                    (from ii in db.IngredientNutritionals
                                        where ii.IngredientId == i.IngredientId
                                        join n in db.Nutrients on ii.NutritionalId equals n.Id
                                        select new
                                        {
                                            n.Name,
                                            Value = (decimal)ii.Value
                                        }).ToList()
                                },
                                FavoriteCount = subI.Count == null ? 0 : 1
                            },
                            User = new
                            {
                                i.UserId,
                                AuthorUName = u.UName,
                                ModifyDate = i.ModifyDate,
                                AuthorFileUrl = Url.GetUserUrl(Request, u.FileUrl)
                            }
                        }).ToList();

                    data = new
                    {
                        Favorite = new
                        {
                            favorite.FavoriteId,
                            favorite.FName,
                            FileUrl = Url.GetFavoriteUrl(Request, favorite.FileUrl),
                            Flag = favorite.IdCategory,
                            favorite.ModifyDate,
                            favorite.Refer,
                            Count = query1.Sum(w => w.FavoriteItem.FavoriteCount)
                        },
                        FavoriteItems = query1
                    };
                    break;
                case IdCategory.Recipe:
                    var list2 = from f in Db.Favorites
                        join fi in Db.FavoriteItems on f.FavoriteId equals fi.FavoriteId
                        where f.IdCategory == IdCategory.Recipe
                        group fi by fi.TId
                        into g
                        select new
                        {
                            g.Key,
                            Count = g.Count() == null ? 0 : g.Count()
                        };

                    var query2 = (from fi in Db.FavoriteItems
                        where fi.FavoriteId == favorite.FavoriteId
                        join r in Db.Recipes on fi.TId equals r.RecipeId
                        join u in Db.Users.DefaultIfEmpty() on r.AuthorId equals u.UserId
                        join c in list2.DefaultIfEmpty() on r.RecipeId equals c.Key into joinedC
                        from subI in joinedC.DefaultIfEmpty()
                        orderby r.ModifyDate descending, r.RecipeId
                        select new
                        {
                            FavoriteItem = new
                            {
                                FileUrl = Url.GetRecipeUrl(Request, r.FileUrl),
                                Id = r.RecipeId,
                                Name = r.RName,
                                Refer = r.Summary,
                                Nutrients = (from item in Db.RecipeItems
                                        join i in Db.Ingredients on item.IngredientId equals i.IngredientId
                                        where item.RecipeId == r.RecipeId
                                        select (from ii in db.IngredientNutritionals
                                            where ii.IngredientId == i.IngredientId
                                            join n in db.Nutrients on ii.NutritionalId equals n.Id
                                            select new
                                            {
                                                n.Name,
                                                Value = (decimal)ii.Value
                                            }).ToList())
                                    .ToList(),
                                FavoriteCount = subI.Count == null ? 0 : 1
                            },
                            User = new
                            {
                                UserId = r.AuthorId,
                                AuthorUName = u.UName,
                                ModifyDate = r.ModifyDate,
                                AuthorFileUrl = Url.GetUserUrl(Request, u.FileUrl)
                            }
                        }).ToList();

                    data = new
                    {
                        Favorite = new
                        {
                            favorite.FavoriteId,
                            favorite.FName,
                            FileUrl = Url.GetFavoriteUrl(Request, favorite.FileUrl),
                            Flag = favorite.IdCategory,
                            favorite.ModifyDate,
                            favorite.Refer,
                            Count = query2.Sum(w => w.FavoriteItem.FavoriteCount)
                        },
                        FavoriteItems = query2
                    };
                    break;
                default:
                    return ApiResponses.Error("收藏夹不存在");
            }

            return ApiResponses.Success("获取收藏夹成功", data);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }
}

public record FavoriteItemBaseDto
{
    [Range(1, long.MaxValue - 2, ErrorMessage = "请求错误")]
    public long Id { get; set; }

    [Required]
    [Range(1, 254, ErrorMessage = "请求错误")]
    public sbyte Flag { get; set; }
}

public record FavoriteItemCreationDto : FavoriteItemBaseDto
{
    [Required] public HashSet<long> FavoriteIds { get; set; }

    [Required(ErrorMessage = "请求错误")] public long TId { get; set; }
}

public record FavoriteItemRemoveDto : FavoriteItemBaseDto
{
    [Required]
    [Range(1, long.MaxValue - 2, ErrorMessage = "请求错误")]
    public long FavoriteId { get; set; }

    [Required(ErrorMessage = "请求错误")] public List<long> TIds { get; set; }
}

public record FavoriteItemSeleteDto : FavoriteItemBaseDto
{
    [Required(ErrorMessage = "请求错误")]
    [Range(1, long.MaxValue - 2, ErrorMessage = "类型错误")]
    public long TId { get; set; }
}

public record FavoriteListItemDto
{
    [Range(1, long.MaxValue - 2, ErrorMessage = "请求错误")]
    public long Id { get; set; }

    [Required]
    [Range(1, long.MaxValue - 2, ErrorMessage = "请求错误")]
    public long FavoriteId { get; set; }
}