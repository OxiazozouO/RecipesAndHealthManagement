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
public class UserFavoriteController(RecipeAndHealthSystemContext db, JwtHelper jwt) : MyControllerBase(db, jwt)
{
    #region Add

    [HttpPost]
    public IActionResult AddFavorite(FavoriteCreationDto dto)
    {
        try
        {
            if (this.CheckUserRole(dto.Id.ToString(), out var result, out var user))
                return result;

            var count = Db.Favorites.Count(f => f.UserId == user.UserId && f.IdCategory == dto.Flag);
            if (count >= 200)
                return ApiResponses.Error($"{IdCategory.GetName(dto.Flag)}收藏夹数量已满");

            var favorite =
                Db.Favorites.FirstOrDefault(f => f.UserId == user.UserId && f.FName == dto.FName);
            if (favorite is not null) return ApiResponses.Error("此收藏夹已存在");

            if (!Url.TryReplaceFile(FileUrlHelper.Favorites, dto.FileUrl))
                return ApiResponses.Error("添加失败");

            favorite = new Favorite
            {
                UserId = user.UserId,
                FileUrl = dto.FileUrl,
                FName = dto.FName,
                IdCategory = dto.Flag,
                Refer = dto.Refer
            };

            Db.Favorites.Add(favorite);
            return Db.SaveChanges() > 0
                ? ApiResponses.Success("添加收藏夹成功", favorite.FavoriteId.ToString())
                : ApiResponses.Error("添加失败");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    #endregion

    #region Remove

    [HttpPost]
    public IActionResult RemoveFavorites(FavoriteRemoveDto dto)
    {
        if (this.CheckUserRole(dto.Id.ToString(), out var result, out var user))
            return result;

        try
        {
            var favorite = Db.Favorites.Where(f =>
                dto.FavoriteIds.Contains(f.FavoriteId) &&
                f.UserId == user.UserId &&
                f.IdCategory == dto.Flag
            ).ToList();
            if (favorite.Count == 0)
                return ApiResponses.Error("收藏夹不存在");
            var favoriteItems = Db.FavoriteItems
                .Where(fi => dto.FavoriteIds.Contains(fi.FavoriteId))
                .ToList();
            Db.FavoriteItems.RemoveRange(favoriteItems);
            Db.Favorites.RemoveRange(favorite);

            return Db.SaveChanges() == favorite.Count + favoriteItems.Count
                ? ApiResponses.Success("删除收藏夹成功")
                : ApiResponses.Error("删除收藏夹失败");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return ApiResponses.ErrorResult;
        }
    }

    #endregion

    #region Update

    [HttpPost]
    public IActionResult EditFavorite(FavoriteUpdateDto dto)
    {
        if (this.CheckUserRole(dto.Id.ToString(), out var result, out var user))
            return result;

        try
        {
            var favorite = Db.Favorites.FirstOrDefault(f => f.FavoriteId == dto.FavoriteId && f.UserId == user.UserId);
            if (favorite is null) return ApiResponses.Error("收藏夹不存在");

            if (dto.FileName != "")
            {
                if (!Url.TryReplaceFile(FileUrlHelper.Favorites, dto.FileName, s => favorite.FileUrl = s))
                    return ApiResponses.ErrorResult;
            }

            favorite.FName = dto.FName;
            favorite.IdCategory = dto.Flag;
            favorite.Refer = dto.Refer;

            Db.Favorites.Update(favorite);

            if (Db.SaveChanges() == 1)
                return ApiResponses.Success("更改成功",
                    dto.FileName == "" ? null : Url.GetFavoriteUrl(Request, favorite.FileUrl));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    #endregion

    #region Select

    [HttpPost]
    public IActionResult UserFavorites(FavoriteBaseDto dto)
    {
        try
        {
            if (this.CheckUserRole(dto.Id.ToString(), out var result, out var user))
                return result;
            IQueryable<Favorite> favorites;
            if (dto.Flag == IdCategory.All)
            {
                favorites = Db.Favorites
                    .Where(f => f.UserId == user.UserId);
            }
            else
            {
                favorites = Db.Favorites
                    .Where(f => f.UserId == user.UserId && f.IdCategory == dto.Flag);
            }

            var ret = favorites.Select(f => new
            {
                f.FavoriteId,
                f.FName,
                FileUrl = Url.GetFavoriteUrl(Request, f.FileUrl),
                Flag = f.IdCategory,
                f.ModifyDate,
                itemsCount = Db.FavoriteItems.Count(fi => fi.FavoriteId == f.FavoriteId)
            }).ToList();

            return ApiResponses.Success("获取收藏夹成功", ret);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    #endregion
}

public record FavoriteBaseDto
{
    [Range(1, long.MaxValue - 2, ErrorMessage = "请求错误")]
    public long Id { get; set; }

    [Required]
    [Range(1, 254, ErrorMessage = "请求错误")]
    public sbyte Flag { get; set; }
}

public record FavoriteRemoveDto : FavoriteBaseDto
{
    [Required] public List<long> FavoriteIds { get; set; }
}

public record FavoriteCreationDto : FavoriteBaseDto
{
    [Required]
    [Range(1, 254, ErrorMessage = "请求错误")]
    public sbyte Flag { get; set; }

    [StringLength(300, ErrorMessage = "上传错误")]
    public string FileUrl { get; set; }

    [Required(ErrorMessage = "收藏夹名称不能为空")]
    [StringLength(30, ErrorMessage = "名称不能超过{1}个字符")]
    public string FName { get; set; }

    [Required(ErrorMessage = "收藏夹简介不能为空")]
    [StringLength(200, ErrorMessage = "简介不能超过{1}个字符")]
    public string Refer { get; set; }
}

public record FavoriteUpdateDto : FavoriteBaseDto
{
    [Required]
    [Range(1, long.MaxValue - 2, ErrorMessage = "请求错误")]
    public long FavoriteId { get; set; }

    [StringLength(60, ErrorMessage = "上传错误")]
    public string FileName { get; set; }

    [Required(ErrorMessage = "收藏夹名称不能为空")]
    [StringLength(100, ErrorMessage = "简介不能超过{1}个字符")]
    public string FName { get; set; }

    [Required(ErrorMessage = "收藏夹简介不能为空")]
    [StringLength(200, ErrorMessage = "简介不能超过{1}个字符")]
    public string Refer { get; set; }
}