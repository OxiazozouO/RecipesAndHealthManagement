using System.ComponentModel.DataAnnotations;
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
public class UserCategoryItemController(RecipeAndHealthSystemContext db, JwtHelper jwt) : MyControllerBase(db, jwt)
{
    // 添加分类项目
    [HttpPost]
    [Authorize(Roles = nameof(JwtType.User))]
    public IActionResult AddCategoryItem(CategoryItemCreationDto dto)
    {
        try
        {
            if (this.CheckUserRole(dto.UserId.ToString(), out var result, out var user))
                return result;

            var item = Db.CategoryItems.FirstOrDefault(f =>
                f.TId == dto.TId
                && f.IdCategory == dto.IdCategory
                && f.UserId == dto.UserId
                && f.CategoryId == dto.CategoryId);

            if (item == null)
            {
                var categoryItem = new CategoryItem
                {
                    CategoryId = dto.CategoryId,
                    UserId = dto.UserId,
                    TId = dto.TId,
                    IdCategory = dto.IdCategory
                };
                Db.CategoryItems.Add(categoryItem);
                return Db.SaveChanges() == 1
                    ? ApiResponses.Success("添加分类项目成功")
                    : ApiResponses.Error("添加分类项目失败");
            }

            return ApiResponses.Error("该分类项目已存在");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    // 创建分类并添加到分类项目

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.User))]
    public IActionResult CreatCategoryItem(CategoryCreationDto dto)
    {
        try
        {
            if (this.CheckUserRole(dto.UserId.ToString(), out var result, out var user))
                return result;

            var category = Db.Categories.FirstOrDefault(c => c.CName == dto.Name && c.TypeId == dto.TypeId);
            if (category == null)
            {
                category = new Category
                {
                    CName = dto.Name,
                    TypeId = dto.TypeId
                };
                Db.Categories.Add(category);
                if (Db.SaveChanges() != 1)
                {
                    return ApiResponses.Error("添加分类失败");
                }
            }

            var item = Db.CategoryItems.FirstOrDefault(f =>
                f.TId == dto.TId
                && f.IdCategory == dto.IdCategory
                && f.UserId == dto.UserId
                && f.CategoryId == category.CategoryId);

            if (item == null)
            {
                var categoryItem = new CategoryItem
                {
                    CategoryId = category.CategoryId,
                    UserId = dto.UserId,
                    TId = dto.TId,
                    IdCategory = dto.IdCategory
                };
                Db.CategoryItems.Add(categoryItem);
                return Db.SaveChanges() == 1
                    ? ApiResponses.Success("添加分类项目成功", new
                    {
                        Id = category.CategoryId,
                        Name = category.CName,
                        Count = 1,
                        category.TypeId,
                        IsLike = true
                    })
                    : ApiResponses.Error("添加分类项目失败");
            }

            return ApiResponses.Error("未知错误");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }


    // 删除分类项目
    [HttpPost]
    [Authorize(Roles = nameof(JwtType.User))]
    public IActionResult DeleteCategoryItem(CategoryItemCreationDto dto)
    {
        try
        {
            if (this.CheckUserRole(dto.UserId.ToString(), out var result, out var user))
                return result;
            var item = Db.CategoryItems.FirstOrDefault(f =>
                f.TId == dto.TId
                && f.IdCategory == dto.IdCategory
                && f.UserId == dto.UserId
                && f.CategoryId == dto.CategoryId);
            if (item == null)
            {
                return ApiResponses.Error("该分类项目不存在");
            }

            Db.CategoryItems.Remove(item);
            return Db.SaveChanges() == 1
                ? ApiResponses.Success("删除分类项目成功")
                : ApiResponses.Error("删除分类项目失败");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }
}

public record CategoryCreationDto
{
    [Range(1, long.MaxValue - 2, ErrorMessage = "请求错误")]
    public long UserId { get; set; }

    [Range(0, 3, ErrorMessage = "请求错误")]
    public sbyte TypeId { get; set; }
    [Required(ErrorMessage = "目标ID是必需的")] public long TId { get; set; }

    [Required(ErrorMessage = "目标类型是必需的")] public sbyte IdCategory { get; set; }

    [Required(ErrorMessage = "分类名称是必需的")]
    [StringLength(30, ErrorMessage = "名称长度不能超过{0}")]
    public string Name { get; set; }
}

public record CategoryItemCreationDto
{
    [Required(ErrorMessage = "分类ID是必需的")] public long CategoryId { get; set; }

    [Required(ErrorMessage = "目标ID是必需的")] public long TId { get; set; }

    [Required(ErrorMessage = "目标类型是必需的")]
    [Range(1, sbyte.MaxValue - 2, ErrorMessage = "请求错误")]
    public sbyte IdCategory { get; set; }

    [Range(1, long.MaxValue - 2, ErrorMessage = "请求错误")]
    public long UserId { get; set; }
}