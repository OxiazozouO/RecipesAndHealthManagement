using System.ComponentModel.DataAnnotations;
using AnyLibrary.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebServer.Configurations;
using WebServer.Controllers.Any;
using WebServer.Controllers.Service;
using WebServer.DatabaseModel;
using WebServer.Helper;
using WebServer.Http;

namespace WebServer.Controllers.User;

[Route("user/[controller]/[action]")]
[ApiController]
public class UserReportController(RecipeAndHealthSystemContext db, JwtHelper jwt) : MyControllerBase(db, jwt)
{
    // 添加举报
    [HttpPost]
    [Authorize(Roles = nameof(JwtType.User))]
    public IActionResult AddReport(ReportCreationDto dto)
    {
        try
        {
            if (this.CheckUserRole(dto.Id.ToString(), out var result, out var user))
                return result;

            if (Db.Reports.Count(r => r.UserId == dto.Id) > AppSettings.UserConfig.UserMaxReportCount)
                return ApiResponses.Error($"举报次数已达上限,请等待处理，当前限制为{AppSettings.UserConfig.UserMaxReportCount}");

            var orDefault = Db.Reports
                .FirstOrDefault(r => r.UserId == dto.Id && r.TId == dto.TId && r.IdCategory == dto.Category);
            if (orDefault != null)
                return ApiResponses.Error("已举报");

            var report = new Report
            {
                UserId = dto.Id,
                TId = dto.TId,
                IdCategory = dto.Category,
                RType = dto.Status,
                Content = dto.Content,
                Status = Status.Pending
            };
            Db.Reports.Add(report);
            return Db.SaveChanges() == 1
                ? ApiResponses.Success("添加举报成功")
                : ApiResponses.Error("添加举报失败");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }
}

public record ReportCreationDto
{
    [Required(ErrorMessage = "用户id是必需的")]
    [Range(1, long.MaxValue - 2, ErrorMessage = "请求错误")]
    public long Id { get; set; }

    [Required(ErrorMessage = "关联对象ID是必需的")]
    [Range(1, long.MaxValue - 2, ErrorMessage = "请求错误")]
    public long TId { get; set; }

    [Required(ErrorMessage = "关联对象ID是必需的")]
    [Range(1, sbyte.MaxValue - 2, ErrorMessage = "请求错误")]
    public sbyte Status { get; set; }

    [Required(ErrorMessage = "关联对象ID的类型是必需的")]
    [Range(0, sbyte.MaxValue - 2, ErrorMessage = "请求错误")]
    public sbyte Category { get; set; }

    [Required(ErrorMessage = "举报内容是必需的")]
    [StringLength(100, ErrorMessage = "举报内容长度不能超过{1}")]
    public string Content { get; set; }
}