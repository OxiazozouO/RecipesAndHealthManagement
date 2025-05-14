using System.ComponentModel.DataAnnotations;
using AnyLibrary.Constants;
using AnyLibrary.Helper;
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
public class UserController(RecipeAndHealthSystemContext db, JwtHelper jwt) : MyControllerBase(db, jwt)
{
    [HttpPost]
    public IActionResult Register(UserRegisterDto dto)
    {
        try
        {
            if (dto.Email is null || dto.Phone is null || dto.Name is null || dto.Password is null)
                return ApiResponses.Error("注册信息不为空");

            var user = Db.Users.FirstOrDefault(u =>
                u.Email == dto.Email || u.Phone == dto.Phone || u.UName == dto.Name);

            if (user is not null)
                return ApiResponses.Error("账号已存在");
            if (!Url.TryReplaceFile(FileUrlHelper.Users, dto.UserImgUrl))
                return ApiResponses.Error("添加失败");

            Db.Users.Add(new DatabaseModel.User
            {
                UName = dto.Name,
                Password = dto.Password,
                Salt = dto.Salt,
                Email = dto.Email,
                Phone = dto.Phone,
                FileUrl = dto.UserImgUrl,
                Status = UserStatus.Usable
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
    public IActionResult Login(UserLoginDto dto)
    {
        try
        {
            var user = Db.Users.FirstOrDefault(u =>
                u.UName == dto.Identifier
                || u.Email == dto.Identifier
                || u.Phone == dto.Identifier);
            if (user == null) return ApiResponses.Error("用户不存在");
            if (user.Status == UserStatus.Logout) return ApiResponses.Error("用户已注销");
            if (user.Status == UserStatus.Ban)
            {
                if (DateTime.Now < user.UnbanTime)
                {
                    return ApiResponses.Error($"用户已被封禁,封禁时间到{user.UnbanTime:yyyy-MM-dd HH:mm:ss}");
                }

                user.Status = UserStatus.Usable;
                Db.Users.Update(user);
                Db.SaveChanges();
            }

            EncryptionHelper.HasPassword(dto.Password, user.Salt, out var hashpswd);

            return hashpswd == user.Password
                ? ApiResponses.Success("登录成功", new
                {
                    jwt = Jwt.GetJwtToken(user.UserId, user.UName, JwtType.User),
                    id = user.UserId
                })
                : ApiResponses.Error("密码错误");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    public IActionResult LoginImg(UserLoginImgDto dto)
    {
        try
        {
            var user = Db.Users.FirstOrDefault(u =>
                u.UName == dto.Identifier
                || u.Email == dto.Identifier
                || u.Phone == dto.Identifier);
            if (user == null) return ApiResponses.Error("");
            if (user.Status == UserStatus.Logout) return ApiResponses.Success("","");
            return ApiResponses.Success("", Url.GetUserUrl(Request, user.FileUrl));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.User))]
    public IActionResult Logout(UserLogoutDto dto)
    {
        try
        {
            if (this.CheckUserRole(dto.Id.ToString(), out var result, out var user))
                return result;

            if (user.Status != UserStatus.Usable
                || (user.Email != dto.Identifier
                    && user.Phone != dto.Identifier)
               ) return Ok(new ApiResponses { Code = -1, Message = "用户不存在" });

            EncryptionHelper.HasPassword(dto.Password, user.Salt, out var hashpswd);
            if (hashpswd != user.Password) return ApiResponses.Error("密码错误");

            user.Status = UserStatus.Logout;
            Db.Users.Update(user);
            return Db.SaveChanges() == 1 ? ApiResponses.Success("注销成功") : ApiResponses.Error("注销失败");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }


    [HttpPost]
    [Authorize(Roles = nameof(JwtType.User))]
    public IActionResult ChangeInfo(UserChangeInfoDto dto)
    {
        try
        {
            if (this.CheckUserRole(dto.Id.ToString(), out var result, out var user))
                return result;
            if (!Url.TryReplaceFile(FileUrlHelper.Users, dto.FileUrl, s => user.FileUrl = s))
                return ApiResponses.Error("修改失败");

            if (!string.IsNullOrEmpty(dto.Name))
            {
                user.UName = dto.Name;
                user.Gender = dto.Gender;
                user.BirthDate = dto.BirthDate;
                Db.Users.Update(user);
                return Db.SaveChanges() == 1 ? ApiResponses.Success("修改成功") : ApiResponses.Error("修改失败");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.User))]
    public IActionResult ChangePassword(UserChangePasswordDto dto)
    {
        try
        {
            if (string.IsNullOrEmpty(dto.Password))
                return ApiResponses.Error("密码不能为空");

            if (this.CheckUserRole(dto.Id.ToString(), out var result, out var user))
                return result;

            if (user.Phone != dto.PhoneOrEmail && user.Email != dto.PhoneOrEmail)
                return ApiResponses.Error("手机号/邮箱错误");

            EncryptionHelper.HasPassword(dto.Password, user.Salt, out var hashpswd);

            if (hashpswd != user.Password)
                return ApiResponses.Error("密码错误");

            EncryptionHelper.HasPassword(dto.NewPassword, user.Salt, out var newhashpswd);
            if (user.Password == newhashpswd)
                return ApiResponses.Error("新密码不能与旧密码相同");

            user.Password = newhashpswd;
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
    [Authorize(Roles = nameof(JwtType.User))]
    public IActionResult GetInfo(UserInfoDto dto)
    {
        try
        {
            if (this.CheckUserRole(dto.Id.ToString(), out var result, out var user))
                return result;
            return ApiResponses.Success("", new
            {
                user.BirthDate,
                FileUrl = Url.GetUserUrl(Request, user.FileUrl),
                user.Gender,
                UserName = user.UName
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }
}

public class UserRegisterDto
{
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

    public string UserImgUrl { get; set; }
}

public record UserLoginImgDto
{
    [Required(ErrorMessage = "用户名/邮箱/手机号不能为空")]
    [MinLength(1, ErrorMessage = "最小长度为 {1} 字符")]
    [MaxLength(20, ErrorMessage = "最大长度为 {1} 字符")]
    public string Identifier { get; set; }
}

public record UserChangeInfoDto
{
    [Range(1, long.MaxValue - 2, ErrorMessage = "请求错误")]
    public long Id { get; set; }

    [StringLength(300, ErrorMessage = "上传错误")]
    public string FileUrl { get; set; }

    [Required(ErrorMessage = "请输入姓名")]
    [StringLength(20, ErrorMessage = "姓名长度不能超过{1}")]
    public string Name { get; set; }

    [Required(ErrorMessage = "请输入性别")] public bool Gender { get; set; }

    [Required(ErrorMessage = "请输入出生日期")] public DateTime BirthDate { get; set; }
}

public record UserChangePasswordDto
{
    [Range(1, long.MaxValue - 2, ErrorMessage = "请求错误")]
    public long Id { get; set; }

    [Required(ErrorMessage = "密码不能为空")]
    [MinLength(6, ErrorMessage = "密码长度不能小于{1}")]
    [MaxLength(20, ErrorMessage = "密码长度不能大于{1}")]
    public string Password { get; set; }

    [Required(ErrorMessage = "密码不能为空")]
    [MinLength(6, ErrorMessage = "密码长度不能小于{1}")]
    [MaxLength(20, ErrorMessage = "密码长度不能大于{1}")]
    public string NewPassword { get; set; }

    [Required(ErrorMessage = "必填项")]
    [MinLength(6, ErrorMessage = "邮箱或手机长度不能小于{1}")]
    [MaxLength(50, ErrorMessage = "邮箱或手机长度不能大于{1}")]
    public string PhoneOrEmail { get; set; }
}

public record UserLogoutDto
{
    [Range(1, long.MaxValue - 2, ErrorMessage = "请求错误")]
    public long Id { get; set; }

    [Required(ErrorMessage = "用户名/邮箱/手机号不能为空")]
    [MinLength(1, ErrorMessage = "最小长度为 {1} 字符")]
    [MaxLength(20, ErrorMessage = "最大长度为 {1} 字符")]
    public string Identifier { get; set; }

    [Required(ErrorMessage = "密码不能为空")]
    [MinLength(6, ErrorMessage = "密码长度不能小于{1}")]
    [MaxLength(20, ErrorMessage = "密码长度不能大于{1}")]
    public string Password { get; set; }
}

public record UserInfoDto
{
    [Range(1, long.MaxValue - 2, ErrorMessage = "请求错误")]
    public long Id { get; set; }
}

public record UserLoginDto
{
    [Required(ErrorMessage = "用户名/邮箱/手机号不能为空")]
    [MinLength(1, ErrorMessage = "最小长度为 {1} 字符")]
    [MaxLength(20, ErrorMessage = "最大长度为 {1} 字符")]
    public string Identifier { get; set; }

    [Required(ErrorMessage = "密码不能为空")]
    [MinLength(6, ErrorMessage = "密码长度不能小于{1}")]
    [MaxLength(20, ErrorMessage = "密码长度不能大于{1}")]
    public string Password { get; set; }
}