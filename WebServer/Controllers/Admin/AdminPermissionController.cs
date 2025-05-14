using System.ComponentModel.DataAnnotations;
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
public class AdminPermissionController(RecipeAndHealthSystemContext db, JwtHelper jwt) : MyControllerBase(db, jwt)
{
    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult GetPermissions(AdminDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;

            var result = (from p in Db.Permissions
                select new
                {
                    p.Id,
                    p.Name,
                    p.Category,
                    p.Title
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
    public IActionResult GetRolePermissions(AdminUserDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;

            var result = (from p in Db.Permissions
                select new
                {
                    p.Id,
                    p.Category,
                    p.Title,
                    p.Name
                }).ToList();

            var permissionIds = (from rp in Db.RolePermissions
                where rp.RoleId == dto.Id
                select rp.PermissionId).ToHashSet();

            var finalResult = result.Select(p => new
            {
                p.Id,
                p.Category,
                p.Title,
                p.Name,
                Checked = permissionIds.Contains(p.Id)
            }).ToList();

            return ApiResponses.Success("获取成功", finalResult);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult GetSimpleRolePermissions(AdminDto dto)
    {
        try
        {
            var roleId = Db.Admins.FirstOrDefault(a => a.Id == dto.AdminId)?.RoleId;
            if (roleId is null) return ApiResponses.Error("管理员不存在");

            var result = (from r in Db.RolePermissions
                where r.RoleId == roleId
                select r.PermissionId).ToList();

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
    public IActionResult AddPermission(PermissionDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;

            Db.Permissions.Add(new Permission
            {
                Name = dto.Url,
                Category = dto.Category,
                Title = dto.Title
            });

            return Db.SaveChanges() == 1 ? ApiResponses.Success("添加成功") : ApiResponses.Error("添加失败");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }


    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult EditPermission(EditPermissionDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;

            var permission = Db.Permissions.FirstOrDefault(p => p.Id == dto.Id);
            if (permission is null) return ApiResponses.Error("权限不存在");


            if (dto.Name == "name")
            {
                if (dto.Value.Length > 50)
                    return ApiResponses.Error("权限名字长度不能大于50");
                permission.Name = dto.Value;
            }
            else if (dto.Name == "category")
            {
                if (dto.Value.Length > 80)
                    return ApiResponses.Error("分组名字长度不能大于80");
                permission.Category = dto.Value;
            }
            else if (dto.Name == "title")
            {
                if (dto.Value.Length > 80)
                    return ApiResponses.Error("权限名字长度不能大于80");
                permission.Title = dto.Value;
            }

            Db.Permissions.Update(permission);

            if (Db.SaveChanges() == 1)
                return ApiResponses.Success("更改成功");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult DeletePermission(AdminUserDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;

            var permission = Db.Permissions.FirstOrDefault(p => p.Id == dto.Id);
            if (permission is null) return ApiResponses.Error("权限不存在");

            return Db.TransactionScope(() =>
            {
                Db.RolePermissions.RemoveRange(Db.RolePermissions.Where(rp => rp.PermissionId == dto.Id));
                Db.Permissions.Remove(permission);
            }, "删除成功", "删除失败");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }
}

public record PermissionDto : AdminDto
{
    [Required(ErrorMessage = "请输入权限路径")]
    [MinLength(1, ErrorMessage = "权限路径长度不能小于{1}")]
    [MaxLength(50, ErrorMessage = "权限路径长度不能大于{1}")]
    public string Url { get; set; }

    [Required(ErrorMessage = "请输入分组名字")]
    [MinLength(1, ErrorMessage = "分组名字长度不能小于{1}")]
    [MaxLength(80, ErrorMessage = "分组名字长度不能大于{1}")]
    public string Category { get; set; }

    [Required(ErrorMessage = "请输入权限名字")]
    [MinLength(1, ErrorMessage = "权限名字长度不能小于{1}")]
    [MaxLength(80, ErrorMessage = "权限名字长度不能大于{1}")]
    public string Title { get; set; }
}

public record EditPermissionDto : AdminUserDto
{
    [Required(ErrorMessage = "请求错误")]
    [MinLength(1, ErrorMessage = "请求错误")]
    [MaxLength(20, ErrorMessage = "请求错误")]
    public string Name { get; set; }

    [Required(ErrorMessage = "请求错误")]
    [MinLength(1, ErrorMessage = "请求错误")]
    [MaxLength(80, ErrorMessage = "请求错误")]
    public string Value { get; set; }
}