using System.ComponentModel.DataAnnotations;
using AnyLibrary.Constants;
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
public class AdminCommentController(RecipeAndHealthSystemContext db, JwtHelper jwt) : MyControllerBase(db, jwt)
{
    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult GetComments(AdminDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;
            var result = (from c in Db.Comments
                    select new
                    {
                        c.CommentId,
                        User = (from u in Db.Users where u.UserId == c.UserId select new { u.FileUrl, u.UName })
                            .FirstOrDefault(),
                        Ent = c.TypeId == IdCategory.Ingredient
                            ? (from i in Db.Ingredients
                                where i.IngredientId == c.TypeId
                                select new { i.FileUrl, Title = i.IName }).FirstOrDefault()
                            : c.TypeId == IdCategory.Recipe
                                ? (from r in Db.Recipes
                                    where r.RecipeId == c.TypeId
                                    select new { r.FileUrl, Title = r.RName }).FirstOrDefault()
                                : c.TypeId == IdCategory.Collection
                                    ? (from r in Db.Collections
                                        where r.CollectionId == c.TypeId
                                        select new { r.FileUrl, Title = r.Title }).FirstOrDefault()
                                    : null,
                        c.TypeId,
                        c.Content,
                        c.CreateDate,
                        c.Status
                    }).AsEnumerable()
                .Select(c => new
                {
                    c.CommentId,
                    UFileUrl = Url.AdminGetUsersUrl(Request, true, c.User.FileUrl),
                    UName = c.User.UName,
                    EFileUrl = Url.AdminGetUrl(Request, (int)c.TypeId, c.Ent.FileUrl),
                    EName = c.Ent.Title,
                    c.Content,
                    CreateDate = c.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    c.Status
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
    public IActionResult ReverseCommentStatus(CommentDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;

            var result = (from c in Db.Comments
                where c.CommentId == dto.Id
                select c).FirstOrDefault();
            if (result is null)
                return ApiResponses.Error("没有找到相关数据");
            if (result.Status != dto.Status)
                return ApiResponses.Error("状态错误");
            switch (result.Status)
            {
                case CommentStatus.Usable:
                    result.Status = CommentStatus.ForceOff;
                    break;
                case CommentStatus.ForceOff:
                    result.Status = CommentStatus.Usable;
                    break;
                default:
                    return ApiResponses.Error("状态错误");
            }

            Db.Comments.Update(result);
            return Db.SaveChanges() == 1
                ? ApiResponses.Success("修改成功")
                : ApiResponses.Error("修改失败");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult DeleteComment(AdminUserDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;

            var result = (from c in Db.Comments
                where c.CommentId == dto.Id
                select c).FirstOrDefault();
            if (result is null)
                return ApiResponses.Error("没有找到相关数据");
            result.Status = Status.ForceOff;
            Db.Comments.Remove(result);
            return Db.SaveChanges() == 1
                ? ApiResponses.Success("删除成功")
                : ApiResponses.Error("删除失败");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }
}

public record CommentDto : AdminUserDto
{
    [Range(0, int.MaxValue - 2, ErrorMessage = "请求错误")]
    [Required(ErrorMessage = "请求失败")]
    public int Status { set; get; }
}