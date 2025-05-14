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
public class AdminUserController(RecipeAndHealthSystemContext db, JwtHelper jwt) : MyControllerBase(db, jwt)
{
    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult GetUsers(AdminDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out _))
                return responses;

            var result = Db.Users.Select(u => new
            {
                Id = u.UserId,
                Name = u.UName,
                FileUrl = Url.AdminGetUserUrl(Request, u.FileUrl),
                Status = u.Status,
                UnbanTime = u.Status == UserStatus.Ban ? u.UnbanTime.ToString("yyyy-MM-dd HH:mm:ss") : ""
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
    public IActionResult GetUser(AdminUserDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out _))
                return responses;

            var user = (from u in Db.Users
                where dto.Id == u.UserId
                select new
                {
                    Id = u.UserId,
                    Name = u.UName,
                    FileUrl = Url.AdminGetUserUrl(Request, u.FileUrl),
                    Gender = u.Gender,
                    BirthDate = u.BirthDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    Status = u.Status,
                    UnbanTime = u.Status == UserStatus.Ban ? u.UnbanTime.ToString("yyyy-MM-dd HH:mm:ss") : ""
                }).FirstOrDefault();

            if (user == null)
                return ApiResponses.Error("用户不存在");
            return user.Id is < 100 and >= 1
                ? ApiResponses.Success("获取成功", user)
                : ApiResponses.Error("不允许修改此用户的个人信息");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult AddUser(AdminRegisterDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out _))
                return responses;

            if (dto.Email is null || dto.Phone is null || dto.Name is null || dto.Password is null)
                return ApiResponses.Error("注册信息不为空");
            var user = Db.Users.FirstOrDefault(u =>
                u.Email == dto.Email || u.Phone == dto.Phone || u.UName == dto.Name);
            if (user is not null) return ApiResponses.Error("账号已存在");

            Db.Users.Add(new DatabaseModel.User
            {
                UName = dto.Name,
                Password = dto.Password,
                Salt = dto.Salt,
                Email = dto.Email,
                Phone = dto.Phone
            });
            return Db.SaveChanges() == 1 ? ApiResponses.Success("注册成功") : ApiResponses.Error("注册失败");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }


    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult Ban(AdminUserBanDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;
            if (dto.Id < 100) return ApiResponses.Error("不允许封禁此用户");
            var user = Db.Users.FirstOrDefault(u => u.UserId == dto.Id);
            if (user is null) return ApiResponses.Error("用户不存在");
            if (user.Status == UserStatus.Logout)
                return ApiResponses.Error("用户已注销");

            if (dto.Status && user.Status != UserStatus.Usable)
            {
                user.Status = UserStatus.Usable;
            }
            else if (!dto.Status && user.Status != UserStatus.Ban)
            {
                user.Status = UserStatus.Ban;
                user.UnbanTime = DateTime.Now.AddDays(dto.Day);
            }

            Db.Users.Update(user);
            return Db.SaveChanges() == 1
                ? ApiResponses.Success("封禁成功")
                : ApiResponses.Error("封禁失败");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }


    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult ChangeInfo(AdminUserChangeInfoDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;
            if (dto.Id >= 100)
                return ApiResponses.Error("不允许修改此用户的个人信息");
            var user = Db.Users.FirstOrDefault(u => u.UserId == dto.Id);
            if (user is null) return ApiResponses.Error("用户不存在");

            user.UName = dto.Name;
            user.Email = "null";
            user.Phone = "null";
            user.Password = "null";
            user.Salt = "null";
            user.Status = UserStatus.Usable;
            user.Gender = dto.Gender;
            user.BirthDate = dto.BirthDate;
            Url.TryReplaceFile(FileUrlHelper.Users, dto.FileUrl, s => user.FileUrl = s);
            Db.Users.Update(user);
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
    public IActionResult ChangePassword(AdminUserChangePasswordDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;
            if (dto.Id < 100)
                return ApiResponses.Error("不允许修改此用户的密码");
            var user = Db.Users.FirstOrDefault(u => u.UserId == dto.Id);
            if (user is null) return ApiResponses.Error("用户不存在");

            if (string.IsNullOrEmpty(dto.NewPassword))
                return ApiResponses.Error("密码不能为空");

            if (dto.OldPassword == dto.NewPassword)
            {
                return ApiResponses.Error("新密码不能与旧密码相同");
            }

            EncryptionHelper.HasPassword(dto.OldPassword, user.Salt, out var oldhashpswd);
            if (user.Password == oldhashpswd)
            {
                EncryptionHelper.HasPassword(dto.NewPassword, user.Salt, out var newhashpswd);
                user.Password = newhashpswd;
                Db.Users.Update(user);
            }


            return Db.SaveChanges() == 1 ? ApiResponses.Success("修改成功") : ApiResponses.Error("修改失败");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }
}

public record AdminRegisterDto
{
    [Range(1, int.MaxValue - 2, ErrorMessage = "请求错误")]
    public int AdminId { get; set; }

    [MinLength(1, ErrorMessage = "用户名长度不能小于{1}")]
    [MaxLength(20, ErrorMessage = "用户名长度不能大于{1}")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "密码不能为空")]
    [StringLength(44, MinimumLength = 44, ErrorMessage = "客户端错误，加密后密码长度为{1}")]
    public string? Password { get; set; }

    [Required(ErrorMessage = "客户端错误")]
    [StringLength(49, MinimumLength = 49, ErrorMessage = "客户端错误，密码盐长度为{1}")]
    public string? Salt { get; set; }

    [EmailAddress(ErrorMessage = "邮箱格式错误")]
    public string? Email { get; set; }

    [StringLength(11, MinimumLength = 11, ErrorMessage = "手机号长度要等于{1}")]
    public string? Phone { get; set; }
}

public record AdminUserChangeInfoDto : AdminUserDto
{
    [Required(ErrorMessage = "请输入姓名")]
    [MinLength(1, ErrorMessage = "用户名长度不能小于{1}")]
    [MaxLength(20, ErrorMessage = "用户名长度不能大于{1}")]
    public string Name { get; set; }

    [MaxLength(300, ErrorMessage = "文件名长度不能大于{1}")]
    public string? FileUrl { get; set; }

    [Required(ErrorMessage = "请输入性别")] public bool Gender { get; set; }

    [Required(ErrorMessage = "请输入出生日期")] public DateTime BirthDate { get; set; }
}

public record AdminUserChangePasswordDto : AdminUserDto
{
    [Required(ErrorMessage = "密码不能为空")]
    [MinLength(6, ErrorMessage = "密码长度不能小于{1}")]
    [MaxLength(20, ErrorMessage = "密码长度不能大于{1}")]
    public string OldPassword { get; set; }

    [Required(ErrorMessage = "密码不能为空")]
    [MinLength(6, ErrorMessage = "密码长度不能小于{1}")]
    [MaxLength(20, ErrorMessage = "密码长度不能大于{1}")]
    public string NewPassword { get; set; }
}

public record AdminUserBanDto : AdminUserDto
{
    [Required(ErrorMessage = "状态值不能为空")] public bool Status { get; set; }
    [Required(ErrorMessage = "状态值不能为空")] public int Day { get; set; }
}

public record AdminUserLogoutDto : AdminUserDto
{
    [Required(ErrorMessage = "状态值不能为空")] public bool Status { get; set; }
}