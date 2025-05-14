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
public class AdminReportController(RecipeAndHealthSystemContext db, JwtHelper jwt) : MyControllerBase(db, jwt)
{
    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult GetReports(AdminDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;

            var result = (from r in Db.Reports
                    join u in Db.Users on r.UserId equals u.UserId
                    select new
                    {
                        r.ReportId,
                        u.UName,
                        UFileUrl = u.FileUrl,
                        r.TId,
                        r.IdCategory,
                        r.RType,
                        r.Content,
                        r.CreateDate,
                        r.Status,
                        r.ProcessingTime,
                        Ent = r.IdCategory == IdCategory.Ingredient ? (from i in Db.Ingredients
                                where i.IngredientId == r.TId
                                select new { Title = i.IName, FileUrl = i.FileUrl }).FirstOrDefault()
                            : r.IdCategory == IdCategory.Recipe ? (from rr in Db.Recipes
                                where rr.RecipeId == r.TId
                                select new { Title = rr.Title, FileUrl = rr.FileUrl }).FirstOrDefault()
                            : r.IdCategory == IdCategory.Collection ? (from c in Db.Collections
                                where c.CollectionId == r.TId
                                select new { Title = c.Title, FileUrl = c.FileUrl }).FirstOrDefault()
                            : r.IdCategory == IdCategory.Comment ? (from comment in Db.Comments
                                where comment.CommentId == r.TId
                                select new { Title = comment.Content, FileUrl = "" }).FirstOrDefault()
                            : null
                    }).AsEnumerable()
                // .Where(r => r.Ent is not null)
                .Select(r => new
                {
                    r.ReportId,
                    r.UName,
                    UFileUrl = Url.AdminGetUserUrl(Request, r.UFileUrl),
                    r.TId,
                    r.IdCategory,
                    r.RType,
                    r.Content,
                    CreateDate = r.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    r.Status,
                    ProcessingTime = r.ProcessingTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    ETitle = r.Ent?.Title,
                    EFileUrl = r.IdCategory switch
                    {
                        IdCategory.Recipe => Url.AdminGetRecipeUrl(Request, r.Ent?.FileUrl),
                        IdCategory.Ingredient => Url.AdminGetIngredientUrl(Request, r.Ent?.FileUrl),
                        IdCategory.Collection => Url.AdminGetCollectionUrl(Request, r.Ent?.FileUrl),
                        _ => ""
                    }
                }).ToList();

            return ApiResponses.Success("获取举报列表成功", result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }


    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult RejectReport(AdminUserDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;

            var report = Db.Reports.FirstOrDefault(r => r.ReportId == dto.Id);
            if (report is null)
                return ApiResponses.Error("没有找到相关数据");
            report.Status = Status.Reject;
            report.StatusContent = "无效举报";
            report.ProcessingTime = DateTime.Now;
            Db.Reports.Update(report);
            return Db.SaveChanges() == 1
                ? ApiResponses.Success("驳回成功")
                : ApiResponses.Error("驳回失败");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult ReportOff(ReportOffDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;

            var report = Db.Reports.FirstOrDefault(r => r.ReportId == dto.Id && r.IdCategory == dto.IdCategory);
            if (report is null)
                return ApiResponses.Error("没有找到相关数据");
            Ingredient? ingredient = null;
            Recipe? recipe = null;
            Collection? collection = null;
            Comment? comment = null;

            switch (dto.IdCategory)
            {
                case IdCategory.Ingredient:
                    ingredient = Db.Ingredients.FirstOrDefault(i => i.IngredientId == report.TId);
                    if (ingredient is null)
                        return ApiResponses.Error("没有找到相关数据");
                    ingredient.Status = Status.ReportOff;
                    report.StatusContent = $"已下架食材[{ingredient.IngredientId}] {ingredient.IName}";
                    break;
                case IdCategory.Recipe:
                    recipe = Db.Recipes.FirstOrDefault(i => i.RecipeId == report.TId);
                    if (recipe is null)
                        return ApiResponses.Error("没有找到相关数据");
                    recipe.Status = Status.ReportOff;
                    report.StatusContent = $"已下架食谱[{recipe.RecipeId}] {recipe.Title}";
                    break;
                case IdCategory.Collection:
                    collection = Db.Collections.FirstOrDefault(i => i.CollectionId == report.TId);
                    if (collection is null)
                        return ApiResponses.Error("没有找到相关数据");
                    collection.Status = Status.ReportOff;
                    report.StatusContent = $"已下架合集[{collection.CollectionId}] {collection.Title}";
                    break;
                case IdCategory.Comment:
                    comment = Db.Comments.FirstOrDefault(i => i.CommentId == report.TId);
                    if (comment is null)
                        return ApiResponses.Error("没有找到相关数据");
                    report.StatusContent = $"已删除评论[{comment.CommentId}] {comment.Content}";
                    break;
                default:
                    return ApiResponses.Error("没有找到相关数据");
            }

            //标记为锁定
            report.Status = Status.Locked;
            report.ProcessingTime = DateTime.Now;

            //开启事务
            return Db.TransactionScope(() =>
            {
                switch (dto.IdCategory)
                {
                    case IdCategory.Ingredient:
                        //标记为举报下架
                        Db.Ingredients.Update(ingredient);
                        break;
                    case IdCategory.Recipe:
                        //标记为举报下架
                        Db.Recipes.Update(recipe);
                        break;
                    case IdCategory.Collection:
                        //标记为举报下架
                        Db.Collections.Update(collection);
                        break;
                    case IdCategory.Comment:
                        //删除评论
                        Db.Comments.Remove(comment);
                        break;
                }
                Db.Reports.Update(report);
            }, "操作成功", "操作失败");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }


    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult DeleteReject(AdminUserDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;

            var report = Db.Reports.FirstOrDefault(r => r.ReportId == dto.Id);
            if (report is null)
                return ApiResponses.Error("没有找到相关数据");
            Db.Reports.Remove(report);
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

public record ReportOffDto : AdminUserDto
{
    [Required(ErrorMessage = "举报id不能为空")] public int IdCategory { get; set; }
}