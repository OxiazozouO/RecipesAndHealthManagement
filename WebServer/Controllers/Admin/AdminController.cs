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
public class AdminController(RecipeAndHealthSystemContext db, JwtHelper jwt) : MyControllerBase(db, jwt)
{
    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost]
    public IActionResult Login(LoginDto dto)
    {
        try
        {
            var admin = Db.Admins.FirstOrDefault(a => a.Name == dto.Username);
            if (admin == null)
                return ApiResponses.Error("管理员不存在");

            EncryptionHelper.HasPassword(dto.Password, admin.Salt, out var hashpswd);

            if (hashpswd != admin.Password) return ApiResponses.Error("密码错误");

            return ApiResponses.Success("登录成功", new
            {
                Jwt = Jwt.GetJwtToken(admin.Id, admin.Name, JwtType.Admin),
                Id = admin.Id,
                Name = admin.Name,
                Url = Url.AdminGetAdminUrl(Request, admin.FileUrl),
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
    public IActionResult GetAdmins(AdminDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out _))
                return responses;

            var result = (from a in Db.Admins
                join r in Db.Roles on a.RoleId equals r.Id
                select new
                {
                    a.Id,
                    FileUrl = Url.AdminGetAdminUrl(Request, a.FileUrl),
                    a.Name,
                    a.Status,
                    RoleId = r.Id,
                    RoleName = r.Name
                }).ToList();
            return ApiResponses.Success("获取成功", result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult AddAdmin(RegisterDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out _))
                return responses;

            Url.TryReplaceFile(FileUrlHelper.Admins, dto.FileUrl, s => dto.FileUrl = s);
            var salt = StringHelper.GetRandomString();

            EncryptionHelper.HasPassword(dto.Password, salt, out var hashpswd);
            Db.Admins.Add(new DatabaseModel.Admin
            {
                FileUrl = dto.FileUrl,
                Name = dto.Name,
                Password = hashpswd,
                Salt = salt,
                Status = UserStatus.Usable,
                RoleId = dto.RoleId
            });

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
    public IActionResult ChangeAdminInfo(ChangeInfoDto dto)
    {
        if (!string.IsNullOrEmpty(dto?.NewPassword) && dto.NewPassword?.Length is > 20 or < 6)
            return ApiResponses.Error("新密码长度为6-20位");
        if (string.IsNullOrEmpty(dto.Password))
            return ApiResponses.Error("密码不能为空");
        if (dto.Password == dto.NewPassword)
            return ApiResponses.Error("新密码不能与旧密码相同");
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out _))
                return responses;
            var admin2 = Db.Admins.FirstOrDefault(a => a.Id == dto.Id);

            if (admin2 is null) return ApiResponses.Error("账号不存在");

            EncryptionHelper.HasPassword(dto.Password, admin2.Salt, out var hashpswd);
            if (admin2.Password != hashpswd)
                return ApiResponses.Error("密码错误");

            if (!string.IsNullOrEmpty(dto.NewPassword))
            {
                EncryptionHelper.HasPassword(dto.NewPassword, admin2.Salt, out var newhashpswd);
                admin2.Password = newhashpswd;
            }

            Url.TryReplaceFile(FileUrlHelper.Admins, dto.FileUrl, s => admin2.FileUrl = s);
            admin2.Name = dto.Name;
            if (admin2.Id != 1)
                admin2.RoleId = dto.RoleId;

            Db.Admins.Update(admin2);
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
    public IActionResult Logout(AdminUserLogoutDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;
            var admin2 = Db.Admins.FirstOrDefault(u => u.Id == dto.Id);
            if (admin2 is null) return ApiResponses.Error("管理员不存在");

            if (admin2.Id == 1)
                return ApiResponses.Error("该管理员不能被注销");

            if (dto.Status && admin2.Status != UserStatus.Usable)
            {
                admin2.Status = UserStatus.Usable;
            }
            else if (!dto.Status && admin2.Status != UserStatus.Logout)
            {
                admin2.Status = UserStatus.Logout;
            }

            Db.Admins.Update(admin2);
            return ApiResponses.Auto(Db, "注销成功", "注销失败");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }
}

public record RegisterDto : AdminDto
{
    [Required(ErrorMessage = "请输入姓名")]
    [StringLength(20, ErrorMessage = "姓名长度不能超过{1}")]
    public string Name { get; set; }

    [MaxLength(300, ErrorMessage = "文件名长度不能大于{1}")]
    public string? FileUrl { get; set; }

    [Required(ErrorMessage = "密码不能为空")]
    [MinLength(6, ErrorMessage = "密码长度不能小于{1}")]
    [MaxLength(20, ErrorMessage = "密码长度不能大于{1}")]
    public string Password { get; set; }

    [Required(ErrorMessage = "请求错误")]
    [Range(1, int.MaxValue - 2, ErrorMessage = "请求错误")]
    public int RoleId { get; set; }
}

public record LoginDto
{
    [Required(ErrorMessage = "用户名/邮箱/手机号不能为空")]
    [MinLength(1, ErrorMessage = "最小长度为 {1} 字符")]
    [MaxLength(20, ErrorMessage = "最大长度为 {1} 字符")]
    public string Username { get; set; }

    [Required(ErrorMessage = "密码不能为空")]
    [MinLength(6, ErrorMessage = "密码长度不能小于{1}")]
    [MaxLength(20, ErrorMessage = "密码长度不能大于{1}")]
    public string Password { get; set; }
}

public record ChangeInfoDto : AdminUserDto
{
    [Required(ErrorMessage = "请输入姓名")]
    [StringLength(20, ErrorMessage = "姓名长度不能超过{1}")]
    public string Name { get; set; }

    [MaxLength(300, ErrorMessage = "文件名长度不能大于{1}")]
    public string? FileUrl { get; set; }

    [MinLength(6, ErrorMessage = "密码长度不能小于{1}")]
    [MaxLength(20, ErrorMessage = "密码长度不能大于{1}")]
    [Required(ErrorMessage = "请求错误")]
    public string Password { get; set; }

    public string? NewPassword { get; set; }

    [Required(ErrorMessage = "请求错误")]
    [Range(1, int.MaxValue - 2, ErrorMessage = "请求错误")]
    public int RoleId { get; set; }
}