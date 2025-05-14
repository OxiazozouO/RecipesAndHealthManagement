using System.ComponentModel.DataAnnotations;
using AnyLibrary.Constants;
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
public class UserCommentController(RecipeAndHealthSystemContext db, JwtHelper jwt) : MyControllerBase(db, jwt)
{
    // 添加评论
    [HttpPost]
    [Authorize(Roles = nameof(JwtType.User))]
    public IActionResult AddComment(CommentCreationDto dto)
    {
        try
        {
            if (this.CheckUserRole(dto.UserId.ToString(), out var result, out var user))
                return result;

            var comment = new Comment
            {
                UserId = user.UserId,
                TId = dto.TId,
                TypeId = dto.TypeId,
                Content = dto.Content,
                UpdateDate = DateTime.Now,
                Status = CommentStatus.Usable
            };
            Db.Comments.Add(comment);
            return Db.SaveChanges() == 1
                ? ApiResponses.Success("添加评论成功")
                : ApiResponses.Error("添加评论失败");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    // 删除评论
    [HttpPost]
    [Authorize(Roles = nameof(JwtType.User))]
    public IActionResult DeleteComment(CommentDeleteDto dto)
    {
        try
        {
            if (this.CheckUserRole(dto.UserId.ToString(), out var result, out var user))
                return result;

            var comment = Db.Comments.FirstOrDefault(f => f.CommentId == dto.CommentId && f.UserId == user.UserId);
            if (comment == null)
                return ApiResponses.ErrorResult;

            Db.Comments.Remove(comment);
            return Db.SaveChanges() == 1
                ? ApiResponses.Success("评论已删除")
                : ApiResponses.Error("删除评论失败");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    // 获取所有评论
    [HttpPost]
    [Authorize(Roles = nameof(JwtType.User))]
    public IActionResult GetComments(CommentAllDto dto)
    {
        try
        {
            if (this.CheckUserRole(dto.UserId.ToString(), out var result, out var user))
                return result;

            var comments = Db.Comments
                .Where(f => f.TId == dto.TId
                            && f.TypeId == dto.TypeId
                            && f.Status == CommentStatus.Usable)
                .Join(Db.Users, c => c.UserId, u => u.UserId, (c, u) => new
                {
                    c.CommentId,
                    u.UserId,
                    FileUrl = Url.GetUserUrl(Request, u.FileUrl),
                    u.UName,
                    c.TId,
                    c.TypeId,
                    c.Content,
                    c.CreateDate
                }).Skip((dto.PageIndex - 1) * 10)
                .Take(10)
                .ToList();
            return ApiResponses.Success("请求评论成功", comments);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }
}

public record CommentCreationDto
{
    [Required(ErrorMessage = "用户ID是必需的")]
    [Range(1, long.MaxValue - 2, ErrorMessage = "请求错误")]
    public long UserId { get; set; }

    [Required(ErrorMessage = "目标ID是必需的")]
    [Range(1, long.MaxValue - 2, ErrorMessage = "请求错误")]
    public long TId { get; set; }

    [Required(ErrorMessage = "目标ID是必需的")]
    [Range(1, long.MaxValue - 2, ErrorMessage = "请求错误")]
    public sbyte TypeId { get; set; }

    [Required(ErrorMessage = "评论内容是必需的")] public string Content { get; set; }
}


public record CommentDeleteDto
{
    [Range(1, long.MaxValue - 2, ErrorMessage = "请求错误")]
    public long UserId { get; set; }

    [Required(ErrorMessage = "评论ID是必需的")]
    [Range(1, long.MaxValue - 2, ErrorMessage = "评论ID范围错误")]
    public long CommentId { get; set; }
}

public record CommentAllDto
{
    [Range(1, long.MaxValue - 2, ErrorMessage = "请求错误")]
    public long UserId { get; set; }

    [Range(1, long.MaxValue - 2, ErrorMessage = "请求错误")]
    public long TId { get; set; }

    [Range(1, int.MaxValue - 2, ErrorMessage = "请求错误")]
    public long TypeId { get; set; }

    [Range(0, int.MaxValue - 2, ErrorMessage = "请求错误")]
    public int PageIndex { get; set; }
}