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

namespace WebServer.Controllers.User;

[Route("user/[controller]/[action]")]
[ApiController]
public class UserReleaseController(RecipeAndHealthSystemContext db, JwtHelper jwt) : MyControllerBase(db, jwt)
{
    [HttpPost]
    [Authorize(Roles = nameof(JwtType.User))]
    public IActionResult DeleteRelease(ReleaseDto dto)
    {
        try
        {
            if (this.CheckUserRole(dto.Id.ToString(), out var result, out var user))
                return result;

            var history = db.GetNowHistory(dto?.ReleaseId);

            if (history == null || history.Status == Status.Deleted)
            {
                return ApiResponses.Success("该审核请求操作无效");
            }

            Db.ReleaseFlowHistories.Add(new ReleaseFlowHistory
            {
                ReleaseId = dto.ReleaseId,
                OpId = dto.Id,
                OpFlag = UserTypes.User,
                Status = Status.Cancel,
                Info = "已被用户取消"
            });
            return Db.SaveChanges() == 1
                ? ApiResponses.Success("删除审核成功")
                : ApiResponses.Error("删除审核失败");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.User))]
    public IActionResult ReleaseRecipe(ReleaseRecipeDto dto)
    {
        try
        {
            if (this.CheckUserRole(dto.Id.ToString(), out var res, out var user))
                return res;

            var editor = new EditorContext
            {
                Db = Db,
                Url = Url,
                ReleaseId = dto.ReleaseId < 0 ? -1 : dto.ReleaseId,
                OpId = dto.Id,
                OpFlag = UserTypes.User,
                TargetType = IdCategory.Recipe,
                Info = dto.ReleaseInfo,
                R = dto.Recipe,
            };

            if (editor.EditorEnt(out res, out var fun))
                return res;

            return Db.TransactionScope(fun, "添加成功，请等待管理员审核", "添加失败");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.User))]
    public IActionResult GetReleaseRecipes(GetReleasesDto dto)
    {
        try
        {
            if (this.CheckUserRole(dto.Id.ToString(), out var result, out var user))
                return result;

            var obj = (from rl in Db.Releases
                    where rl.AuthorId == dto.Id
                          && rl.IdCategory == IdCategory.Recipe
                          && rl.OpFlag == UserTypes.User
                    select new
                    {
                        rl.ReleaseId,
                        ReleaseFlowHistories = (from rfh in Db.ReleaseFlowHistories
                                where rfh.ReleaseId == rl.ReleaseId
                                orderby rfh.CreateDate descending
                                select new
                                {
                                    User = rfh.OpFlag == UserTypes.User
                                        ? Db.Users.Where(a => a.UserId == rfh.OpId)
                                            .Select(a => new
                                            {
                                                Id = a.UserId,
                                                Name = a.UName,
                                                FileUrl = Url.GetUserUrl(Request, a.FileUrl)
                                            }).FirstOrDefault()
                                        : rfh.OpFlag == UserTypes.Admin
                                            ? Db.Admins
                                                .Where(a => a.Id == rfh.OpId)
                                                .Select(a => new
                                                {
                                                    Id = (long)a.Id,
                                                    Name = a.Name,
                                                    FileUrl = Url.GetAdminUrl(Request, a.FileUrl)
                                                }).FirstOrDefault()
                                            : null,
                                    rfh.Status,
                                    rfh.Info,
                                    rfh.CreateDate
                                })
                            .ToList(),
                        rl.FileUrl,
                        rl.Title,
                        rl.TId,
                        rl.CreateDate,
                        rl.IdCategory
                    })
                .AsEnumerable()
                .Where(r =>
                    r.ReleaseFlowHistories.Count > 0 && r.ReleaseFlowHistories.First().Status != Status.Locked)
                .Select(r =>
                    r with
                    {
                        FileUrl = Url.GetIdCategoryUrl(Request, r.IdCategory, r.FileUrl)
                    })
                .ToList();

            if (dto.Flag != Status.All)
            {
                bool b = true;
                Func<int, bool> func = null;
                if (dto.Flag == Status.Pending)
                {
                    func = Status.IsPending;
                }
                else if (dto.Flag == Status.Approve)
                {
                    func = Status.IsApprove;
                }
                else if (dto.Flag == Status.Other)
                {
                    func = Status.IsOther;
                }
                else
                {
                    b = false;
                }

                if (b)
                    obj = obj.Where(x1 =>
                    {
                        if (x1.ReleaseFlowHistories.Count < 1) return false;
                        return func.Invoke(x1.ReleaseFlowHistories.First().Status);
                    }).ToList();
            }

            return ApiResponses.Success("请求成功", obj);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.User))]
    public IActionResult GetReleaseRecipe(ReleaseDto dto)
    {
        try
        {
            if (this.CheckUserRole(dto.Id.ToString(), out var res, out var user))
                return res;

            var release = Db.Releases.FirstOrDefault(r =>
                r.ReleaseId == dto.ReleaseId
                && r.AuthorId == dto.Id
                && r.IdCategory == IdCategory.Recipe
                && r.OpFlag == UserTypes.User
            );
            if (release == null) return ApiResponses.Error("没有找到此发布记录");

            var recipeModel = release.Content.ToEntity<AddRecipeModel>();
            recipeModel.FileUrl = Url.GetRecipeUrl(Request, recipeModel.FileUrl);
            foreach (var step in recipeModel.Steps)
                step.FileUrl = Url.GetRecipeUrl(Request, step.FileUrl);

            var dir = recipeModel.Ingredients.ToDictionary(i => i.IngredientId, i => i.Dosage);

            var ingredients = (from i in Db.Ingredients
                    where dir.Keys.Contains(i.IngredientId)
                    select new
                    {
                        i.IngredientId,
                        i.IName,
                        i.Refer,
                        i.Unit,
                        i.Quantity,
                        Nutritional = (from ii in db.IngredientNutritionals
                            where ii.IngredientId == i.IngredientId
                            join n in db.Nutrients on ii.NutritionalId equals n.Id
                            select new
                            {
                                n.Name,
                                ii.Value
                            }).ToList(),
                        i.Allergy,
                        i.Content,
                        i.FileUrl,
                        Dosage = dir.GetValueOrDefault(i.IngredientId, 0)
                    })
                .AsEnumerable()
                .Select(i => new
                {
                    i.IngredientId,
                    i.IName,
                    i.Refer,
                    i.Unit,
                    Quantity = i.Quantity.ToEntity<Dictionary<string, decimal>>(),
                    Nutritional = i.Nutritional.ToDictionary(n => n.Name, n => (decimal)n.Value),
                    i.Allergy,
                    i.Content,
                    FileUrl = Url.GetIngredientUrl(Request, i.FileUrl),
                    i.Dosage
                }).ToList();

            return ApiResponses.Success("获取食谱详细信息成功", new
            {
                ReleaseInfo = release.ReleaseInfo,
                Recipe = new
                {
                    recipeModel.Title,
                    recipeModel.RName,
                    recipeModel.FileUrl,
                    recipeModel.Summary,
                    Category = recipeModel.Categories?.Select(c => new { Name = c }).ToList(),
                    Steps = recipeModel.Steps,
                    Ingredients = ingredients
                }
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.User))]
    public IActionResult ReleaseCollection(ReleaseCollectionDto dto)
    {
        try
        {
            if (this.CheckUserRole(dto.Id.ToString(), out var res, out var user))
                return res;

            var editor = new EditorContext
            {
                Db = Db,
                Url = Url,
                ReleaseId = dto.ReleaseId < 0 ? -1 : dto.ReleaseId,
                OpId = dto.Id,
                OpFlag = UserTypes.User,
                TargetType = IdCategory.Collection,
                Info = dto.ReleaseInfo,
                C = dto.Collection,
            };

            if (editor.EditorEnt(out res, out var fun))
                return res;

            return Db.TransactionScope(fun, "添加成功，请等待管理员审核", "添加失败");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.User))]
    public IActionResult GetReleaseCollections(GetReleasesDto dto)
    {
        try
        {
            if (this.CheckUserRole(dto.Id.ToString(), out var result, out var user))
                return result;

            var obj = (from rl in Db.Releases
                    where rl.AuthorId == dto.Id
                          && rl.IdCategory == IdCategory.Collection
                          && rl.OpFlag == UserTypes.User
                    select new
                    {
                        rl.ReleaseId,
                        ReleaseFlowHistories = (from rfh in Db.ReleaseFlowHistories
                                where rfh.ReleaseId == rl.ReleaseId
                                orderby rfh.CreateDate descending
                                select new
                                {
                                    User = rfh.OpFlag == UserTypes.User
                                        ? Db.Users.Where(a => a.UserId == rfh.OpId)
                                            .Select(a => new
                                            {
                                                Id = a.UserId,
                                                Name = a.UName,
                                                FileUrl = Url.GetUserUrl(Request, a.FileUrl)
                                            }).FirstOrDefault()
                                        : rfh.OpFlag == UserTypes.Admin
                                            ? Db.Admins
                                                .Where(a => a.Id == rfh.OpId)
                                                .Select(a => new
                                                {
                                                    Id = (long)a.Id,
                                                    Name = a.Name,
                                                    FileUrl = Url.GetAdminUrl(Request, a.FileUrl)
                                                }).FirstOrDefault()
                                            : null,
                                    rfh.Status,
                                    rfh.Info,
                                    rfh.CreateDate
                                })
                            .ToList(),
                        rl.FileUrl,
                        rl.Title,
                        rl.TId,
                        rl.CreateDate,
                        rl.IdCategory
                    })
                .AsEnumerable()
                .Where(r =>
                    r.ReleaseFlowHistories.Count > 0 && r.ReleaseFlowHistories.First().Status != Status.Locked)
                .Select(r =>
                    r with
                    {
                        FileUrl = Url.GetIdCategoryUrl(Request, r.IdCategory, r.FileUrl)
                    })
                .ToList();


            if (dto.Flag != Status.All)
            {
                bool b = true;
                Func<int, bool> func = null;
                if (dto.Flag == Status.Pending)
                {
                    func = Status.IsPending;
                }
                else if (dto.Flag == Status.Approve)
                {
                    func = Status.IsApprove;
                }
                else if (dto.Flag == Status.Other)
                {
                    func = Status.IsOther;
                }
                else
                {
                    b = false;
                }

                if (b)
                    obj = obj.Where(x1 =>
                    {
                        if (x1.ReleaseFlowHistories.Count < 1) return false;
                        return func.Invoke(x1.ReleaseFlowHistories.First().Status);
                    }).ToList();
            }

            return ApiResponses.Success("请求成功", obj);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.User))]
    public IActionResult GetReleaseCollection(ReleaseDto dto)
    {
        try
        {
            if (this.CheckUserRole(dto.Id.ToString(), out var res, out var user))
                return res;

            var release = Db.Releases.FirstOrDefault(r =>
                r.ReleaseId == dto.ReleaseId
                && r.AuthorId == dto.Id
                && r.IdCategory == IdCategory.Collection
                && r.OpFlag == UserTypes.User
            );
            if (release == null) return ApiResponses.Error("没有找到此发布记录");

            var collection = release.Content.ToEntity<AddCollectionModel>();
            collection.FileUrl = Url.GetCollectionUrl(Request, collection.FileUrl);
            var images = CollectionService.GetImages(Url, Request, collection.Content.Images, false);
            var result = CollectionService.GetTabs(Db, Url, Request, collection.Content.Dirs, false);

            return ApiResponses.Success("获取合集详细信息成功", new
            {
                release.ReleaseInfo,
                collection.FileUrl,
                Refer = collection.Summary,
                collection.Title,
                collection.Content.Html,
                Tabs = result,
                Images = images
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.User))]
    public IActionResult GetReleaseStatus(GetReleaseStatusDto dto)
    {
        try
        {
            if (this.CheckUserRole(dto.Id.ToString(), out var res, out var user))
                return res;

            var result = new Dictionary<long, int>();

            switch (dto.IdCategory)
            {
                case IdCategory.Ingredient:
                    result = Db.Ingredients.Where(i => dto.Ids.Contains(i.IngredientId))
                        .Select(i => new { i.IngredientId, i.Status })
                        .ToDictionary(i => i.IngredientId, i => i.Status);
                    break;
                case IdCategory.Recipe:
                    result = (from r in Db.Recipes
                            where r.AuthorId == dto.Id && dto.Ids.Contains(r.RecipeId)
                            select new { r.RecipeId, r.Status })
                        .ToDictionary(i => i.RecipeId, i => i.Status);
                    break;
                case IdCategory.Collection:
                    result = (from r in Db.Collections
                            where r.UserId == dto.Id && dto.Ids.Contains(r.CollectionId)
                            select new { r.CollectionId, r.Status })
                        .ToDictionary(i => i.CollectionId, i => i.Status);
                    break;
            }

            return ApiResponses.Success("获取合集对应项目状态成功", result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }


    [HttpPost]
    [Authorize(Roles = nameof(JwtType.User))]
    public IActionResult ReverseRecipeStatus(SetRecipeStatusDto dto)
    {
        try
        {
            if (this.CheckUserRole(dto.Id.ToString(), out var responses, out var user))
                return responses;
            return Db.ReverseRecipeStatus(dto.RecipeId, dto.Status, UserTypes.User);
        }
        catch (Exception ex)
        {
            return ApiResponses.ErrorResult;
        }
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.User))]
    public IActionResult ReverseCollectionStatus(SetCollectionStatusDto dto)
    {
        try
        {
            if (this.CheckUserRole(dto.Id.ToString(), out var responses, out var user))
                return responses;
            return Db.ReverseCollectionStatus(dto.CollectionId, dto.Status, UserTypes.User);
        }
        catch (Exception ex)
        {
            return ApiResponses.ErrorResult;
        }
    }
}

public record GetReleasesDto
{
    [Required(ErrorMessage = "用户id是必需的")]
    [Range(1, long.MaxValue - 2, ErrorMessage = "请求错误")]
    public long Id { get; set; }

    [Required(ErrorMessage = "请求错误")]
    [Range(1, int.MaxValue - 2, ErrorMessage = "请求错误")]
    public int Flag { get; set; }
}

public record ReleaseDto
{
    [Required(ErrorMessage = "用户id是必需的")]
    [Range(1, long.MaxValue - 2, ErrorMessage = "请求错误")]
    public long Id { get; set; }

    [Required(ErrorMessage = "发布id是必需的")]
    [Range(1, long.MaxValue - 2, ErrorMessage = "请求错误")]
    public long ReleaseId { get; set; }
}

public record ReleaseRecipeDto
{
    [Required(ErrorMessage = "用户id是必需的")]
    [Range(1, long.MaxValue - 2, ErrorMessage = "请求错误")]
    public long Id { get; set; }

    [Required(ErrorMessage = "申请id是必需的")]
    [Range(-1, long.MaxValue - 2, ErrorMessage = "请求错误")]
    public long ReleaseId { get; set; } = -1;

    [Required(ErrorMessage = "参考资料来源是必需的")]
    [StringLength(200, ErrorMessage = "参考资料来源长度不能超过{1}")]

    public string ReleaseInfo { get; set; }

    [Required(ErrorMessage = "食谱是必需的")] public AddRecipeModel Recipe { get; set; }
}

public record ReleaseCollectionDto
{
    [Required(ErrorMessage = "用户id是必需的")]
    [Range(1, long.MaxValue - 2, ErrorMessage = "请求错误")]
    public long Id { get; set; }

    [Required(ErrorMessage = "申请id是必需的")]
    [Range(-1, long.MaxValue - 2, ErrorMessage = "请求错误")]
    public long ReleaseId { get; set; } = -1;

    [Required(ErrorMessage = "参考资料来源是必需的")]
    [StringLength(200, ErrorMessage = "参考资料来源长度不能超过{1}")]
    public string ReleaseInfo { get; set; }

    [Required(ErrorMessage = "合集是必需的")] public AddCollectionModel Collection { get; set; }
}

public record GetReleaseStatusDto
{
    [Required(ErrorMessage = "用户id是必需的")]
    [Range(1, long.MaxValue - 2, ErrorMessage = "请求错误")]
    public long Id { get; set; }

    [Required(ErrorMessage = "分类是必需的")]
    [Range(1, sbyte.MaxValue - 2, ErrorMessage = "请求错误")]
    public sbyte IdCategory { get; set; }

    [Required(ErrorMessage = "实体id是必需的")]
    [CollectionLength(1, 1000, ErrorMessage = "实体最多只能选择{1}个")]
    public HashSet<long> Ids { get; set; }
}

public record SetRecipeStatusDto
{
    [Required(ErrorMessage = "用户id是必需的")]
    [Range(1, long.MaxValue - 2, ErrorMessage = "请求错误")]
    public long Id { get; set; }

    [Required(ErrorMessage = "合集id是必需的")]
    [Range(1, long.MaxValue - 2, ErrorMessage = "请求错误")]
    public long RecipeId { get; set; }


    [Required(ErrorMessage = "请求错误")]
    [Range(1, int.MaxValue - 2, ErrorMessage = "请求错误")]
    public int Status { get; set; }
}

public record SetCollectionStatusDto
{
    [Required(ErrorMessage = "用户id是必需的")]
    [Range(1, long.MaxValue - 2, ErrorMessage = "请求错误")]
    public long Id { get; set; }

    [Required(ErrorMessage = "合集id是必需的")]
    [Range(1, long.MaxValue - 2, ErrorMessage = "请求错误")]
    public long CollectionId { get; set; }


    [Required(ErrorMessage = "请求错误")]
    [Range(1, int.MaxValue - 2, ErrorMessage = "请求错误")]
    public int Status { get; set; }
}