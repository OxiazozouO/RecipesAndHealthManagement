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
public class AdminRoleController(RecipeAndHealthSystemContext db, JwtHelper jwt) : MyControllerBase(db, jwt)
{
    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult GetRoles(AdminDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;

            var mp = (from rp in Db.RolePermissions
                group rp by rp.RoleId
                into g
                select new
                {
                    RoleId = g.Key,
                    Count = g.Count()
                }).ToDictionary(x => x.RoleId, x => x.Count);

            var result = (from r in Db.Roles
                join u in Db.Admins on r.Id equals u.RoleId into grouping
                from u in grouping.DefaultIfEmpty()
                group u by r
                into g
                select new
                {
                    g.Key.Id,
                    g.Key.Name,
                    g.Key.Refer,
                    uCount = g.Count(u => u != null),
                    pCount = mp.GetValueOrDefault(g.Key.Id, 0)
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
    public IActionResult GetSimpleRoles(AdminDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;

            var result = (from r in Db.Roles
                select new
                {
                    r.Id,
                    r.Name
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
    public IActionResult EditRolePermissions(EditRolePermissionDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;
            var rm = Db.RolePermissions
                .Where(rp => rp.RoleId == dto.Id)
                .ToDictionary(r => r.PermissionId, r => r);
            List<RolePermission> add = [];
            foreach (var permissionId in dto.Permissions)
                if (!rm.Remove(permissionId))
                    add.Add(new RolePermission { RoleId = (int)dto.Id, PermissionId = permissionId });

            return Db.TransactionScope(() =>
            {
                Db.RolePermissions.RemoveRange(rm.Values);
                Db.RolePermissions.AddRange(add);
            }, "修改成功", "修改失败");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult AddRole(AddRoleDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;

            Db.Roles.Add(new Role
            {
                Name = dto.Name,
                Refer = dto.Refer
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
    public IActionResult EditRole(EditRoleDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;

            var role = Db.Roles.FirstOrDefault(p => p.Id == dto.Id);
            if (role is null) return ApiResponses.Error("角色不存在");

            if (dto.Name == "name")
            {
                if (dto.Value.Length > 50)
                    return ApiResponses.Error("权限名字长度不能大于50");
                role.Name = dto.Value;
            }
            else if (dto.Name == "refer")
            {
                if (dto.Value.Length > 80)
                    return ApiResponses.Error("分组名字长度不能大于80");
                role.Refer = dto.Value;
            }

            Db.Roles.Update(role);

            if (Db.SaveChanges() == 1)
                return ApiResponses.Success("更改成功");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }
}

public record EditRolePermissionDto : AdminUserDto
{
    [Required(ErrorMessage = "请求错误")] public HashSet<int> Permissions { set; get; }
}

public record AddRoleDto : AdminDto
{
    [Required(ErrorMessage = "请输入权限路径")]
    [MinLength(1, ErrorMessage = "权限路径长度不能小于{1}")]
    [MaxLength(50, ErrorMessage = "权限路径长度不能大于{1}")]
    public string Name { get; set; }

    [Required(ErrorMessage = "请输入分组名字")]
    [MinLength(1, ErrorMessage = "分组名字长度不能小于{1}")]
    [MaxLength(200, ErrorMessage = "分组名字长度不能大于{1}")]
    public string Refer { get; set; }
}

public record EditRoleDto : AdminUserDto
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