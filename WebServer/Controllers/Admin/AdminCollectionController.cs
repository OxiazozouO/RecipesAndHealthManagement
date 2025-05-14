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
public class AdminCollectionController(RecipeAndHealthSystemContext db, JwtHelper jwt) : MyControllerBase(db, jwt)
{
    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult GetCollections(AdminDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;

            var result = (from c in Db.Collections
                join u in Db.Users on c.UserId equals u.UserId
                select new
                {
                    c.CollectionId,
                    UFileUrl = Url.AdminGetUserUrl(Request, u.FileUrl),
                    UName = u.UName,
                    FileUrl = Url.AdminGetCollectionUrl(Request, c.FileUrl),
                    c.Title,
                    c.Summary,
                    c.Status,
                    CreateDate = c.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    ModifyDate = c.ModifyDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    FavoriteCount = (from fi in db.FavoriteItems
                        join f in db.Favorites on fi.FavoriteId equals f.FavoriteId
                        where fi.TId == c.CollectionId && f.IdCategory == IdCategory.Collection
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
    public IActionResult GetCollection(AdminUserDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var res, out var admin))
                return res;

            var c = Db.Collections.FirstOrDefault(c => c.CollectionId == dto.Id);
            if (c is null) return ApiResponses.Error("合集不存在");

            var collection = new AddCollectionModel
            {
                CollectionId = c.CollectionId,
                FileUrl = Url.AdminGetCollectionUrl(Request, c.FileUrl),
                Title = c.Title,
                Summary = c.Summary,
                Content = c.Content.ToEntity<HtmlData>()
            };
            var images = CollectionService.GetImages(Url, Request, collection.Content.Images);
            var result = CollectionService.GetTabs(Db, Url, Request, collection.Content.Dirs);

            return ApiResponses.Success("获取合集详细信息成功", new
            {
                collection.FileUrl,
                collection.Summary,
                collection.Title,
                collection.Content.Html,
                Tabs = result,
                Images = images
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
    public IActionResult AddCollection(AddCollectionDto dto)
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
                TargetType = IdCategory.Collection,
                Info = null,
                C = dto.Collection,
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
    public IActionResult EditCollection(EditCollectionDto dto)
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
                TargetType = IdCategory.Collection,
                Info = null,
                C = dto.Collection,
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
    public IActionResult ReverseCollectionStatus(SetStatusDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;
            return Db.ReverseCollectionStatus(dto.Id, dto.Status, UserTypes.Admin);
        }
        catch (Exception ex)
        {
            return ApiResponses.ErrorResult;
        }
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult DeleteCollection(AdminUserDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;

            var collection = Db.Collections.FirstOrDefault(i => i.CollectionId == dto.Id);
            if (collection is null) return ApiResponses.Error("合集不存在");
            if (collection.Status is not Status.ForceOff)
                return ApiResponses.Error("该合集状态不许删除");
            collection.Status = Status.Deleted;
            Db.Collections.Update(collection);
            return Db.SaveChanges() == 1
                ? ApiResponses.Success("修改成功")
                : ApiResponses.Error("修改失败");
        }
        catch (Exception ex)
        {
            return ApiResponses.ErrorResult;
        }
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult GetTabs(GetTabsDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;
            object? res = null;

            switch (dto.Flag)
            {
                case IdCategory.Ingredient:
                    res = (from i in Db.Ingredients
                        where dto.Ids.Contains(i.IngredientId)
                        select new
                        {
                            IdCategory = dto.Flag,
                            Id = i.IngredientId,
                            FileUrl = Url.AdminGetIngredientUrl(Request, i.FileUrl),
                            Title = i.IName,
                            Refer = i.Refer
                        }).ToList();
                    break;
                case IdCategory.Recipe:
                    res = (from r in Db.Recipes
                        where dto.Ids.Contains(r.RecipeId)
                        select new
                        {
                            IdCategory = dto.Flag,
                            Id = r.RecipeId,
                            FileUrl = Url.AdminGetRecipeUrl(Request, r.FileUrl),
                            Title = r.RName,
                            Refer = r.Summary
                        }).ToList();
                    break;
                case IdCategory.Collection:
                    res = (from c in Db.Collections
                        where dto.Ids.Contains(c.CollectionId)
                        select new
                        {
                            IdCategory = dto.Flag,
                            Id = c.CollectionId,
                            FileUrl = Url.AdminGetCollectionUrl(Request, c.FileUrl),
                            Title = c.Title,
                            Refer = c.Summary
                        }).ToList();
                    break;
                default:
                    return ApiResponses.Error("参数错误");
            }

            return res is null
                ? ApiResponses.Error("没有找到相关数据")
                : ApiResponses.Success("获取成功", res);
        }
        catch (Exception ex)
        {
            return ApiResponses.ErrorResult;
        }
    }
}

public record EditCollectionDto : AdminDto
{
    [Range(-1, int.MaxValue - 2, ErrorMessage = "请求错误")]
    [Required(ErrorMessage = "申请id不能为空")]
    public int ReleaseId { get; set; }

    [Required(ErrorMessage = "需审核的数据不能为空")]
    public AddCollectionModel Collection { get; set; }
}

public record AddCollectionDto : AdminDto
{
    public AddCollectionModel Collection { get; set; }
}

public record SetStatusDto : AdminUserDto
{
    [Range(0, int.MaxValue - 2, ErrorMessage = "请求错误")]
    public int Status { get; set; }
}

public record GetTabsDto : AdminDto
{
    [Range(-1, int.MaxValue - 2, ErrorMessage = "请求错误")]
    [Required(ErrorMessage = "请求错误")]
    public int Flag { get; set; }

    [Required(ErrorMessage = "图片不能为空")]
    [CollectionLength(1, 1000, ErrorMessage = "实体最多只能选择{1}个")]
    public HashSet<long> Ids { get; set; }
}