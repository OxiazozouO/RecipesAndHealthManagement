using System.ComponentModel.DataAnnotations;
using AnyLibrary.Constants;
using AnyLibrary.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using WebServer.Controllers.Any;
using WebServer.Controllers.Service;
using WebServer.DatabaseModel;
using WebServer.DTOs;
using WebServer.Helper;
using WebServer.Http;

namespace WebServer.Controllers.Admin;

[Route("admin/[controller]/[action]")]
[ApiController]
public class AdminReleaseController(RecipeAndHealthSystemContext db, JwtHelper jwt) : MyControllerBase(db, jwt)
{
    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult GetReleases(AdminDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;

            var obj = (from rl in Db.Releases
                    select new
                    {
                        Id = rl.ReleaseId,
                        Author = rl.OpFlag == UserTypes.User
                            ? (from u in Db.Users
                                where u.UserId == rl.AuthorId
                                select new
                                {
                                    AIsUser = true,
                                    AName = u.UName,
                                    AFileUrl = u.FileUrl
                                }).FirstOrDefault()
                            : (from a in Db.Admins
                                where a.Id == rl.AuthorId
                                select new
                                {
                                    AIsUser = false,
                                    AName = a.Name,
                                    AFileUrl = a.FileUrl
                                }).FirstOrDefault(),
                        rl.FileUrl,
                        rl.Title,
                        rl.TId,
                        CreateDate = rl.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                        rl.IdCategory,
                        ReleaseFlowHistory = (from rfh in Db.ReleaseFlowHistories
                                where rfh.ReleaseId == rl.ReleaseId
                                orderby rfh.CreateDate descending
                                select new
                                {
                                    rfh.Status,
                                    rfh.Info,
                                    CreateDate = rfh.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")
                                })
                            .FirstOrDefault()
                    })
                .AsEnumerable()
                .Select(r => new
                {
                    r.Id,
                    r.Author.AIsUser,
                    r.Author.AName,
                    AFileUrl = Url.AdminGetUsersUrl(Request, r.Author.AIsUser, r.Author.AFileUrl),
                    r.Title,
                    r.TId,
                    r.CreateDate,
                    r.IdCategory,
                    FileUrl = Url.AdminGetUrl(Request, r.IdCategory, r.FileUrl),
                    RStatus = r.ReleaseFlowHistory?.Status,
                    RInfo = r.ReleaseFlowHistory?.Info,
                    RCreateDate = r.ReleaseFlowHistory?.CreateDate,
                }).ToList();

            return ApiResponses.Success("请求成功", obj);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult RemoveRelease(AdminUserDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;

            var release = (from r in Db.Releases
                where r.ReleaseId == dto.Id
                select r).FirstOrDefault();
            if (release is null)
            {
                return ApiResponses.Error("没有找到此发布记录");
            }

            var histories = (from rf in Db.ReleaseFlowHistories
                where rf.ReleaseId == dto.Id
                orderby rf.CreateDate descending
                select rf).ToList();
            if (histories.Count == 0)
            {
                return ApiResponses.Error("没有找到此发布记录");
            }

            var f = histories.First();
            if (f is null)
            {
                return ApiResponses.Error("没有找到此发布记录");
            }

            if (f.Status is not (Status.Cancel or Status.Deleted or Status.Reject))
            {
                return ApiResponses.Error("该发布记录状态不允许删除");
            }

            return Db.TransactionScope(() =>
            {
                Db.ReleaseFlowHistories.RemoveRange(histories);
                Db.Releases.Remove(release);
            }, "删除成功", "删除失败");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult GetRelease(AdminUserDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;

            var release = (from r in Db.Releases where r.ReleaseId == dto.Id select r).FirstOrDefault();
            if (release is null)
            {
                return ApiResponses.Error("没有找到此发布记录");
            }

            var ingredientPath = Url.Action("GetImageFile", "File",
                new { path = FileUrlHelper.Ingredients, fileName = "qwq" },
                Request.Scheme,
                Request.Host.Value)?.Replace("qwq", "");
            var recipePath = Url.Action("GetImageFile", "File",
                new { path = FileUrlHelper.Recipes, fileName = "qwq" },
                Request.Scheme,
                Request.Host.Value)?.Replace("qwq", "");
            var collectionPath = Url.Action("GetImageFile", "File",
                new { path = FileUrlHelper.Collections, fileName = "qwq" },
                Request.Scheme,
                Request.Host.Value)?.Replace("qwq", "");

            var obj = (from rl in Db.Releases
                    where rl.TId == release.TId && rl.IdCategory == release.IdCategory
                    orderby rl.CreateDate ascending
                    select new
                    {
                        Id = rl.ReleaseId,
                        rl.FileUrl,
                        rl.Title,
                        rl.TId,
                        CreateDate = rl.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                        rl.IdCategory,
                        rl.Content,
                        rl.ReleaseInfo,
                        Author = rl.OpFlag == UserTypes.User
                            ? (from u in Db.Users
                                where u.UserId == rl.AuthorId
                                select new
                                {
                                    AIsUser = true,
                                    AName = u.UName,
                                    AFileUrl = u.FileUrl
                                }).FirstOrDefault()
                            : (from a in Db.Admins
                                where a.Id == rl.AuthorId
                                select new
                                {
                                    AIsUser = false,
                                    AName = a.Name,
                                    AFileUrl = a.FileUrl
                                }).FirstOrDefault(),
                        ReleaseFlowHistories = (from rfh in Db.ReleaseFlowHistories
                                where rfh.ReleaseId == rl.ReleaseId
                                orderby rfh.CreateDate ascending
                                select new
                                {
                                    User = rfh.OpFlag == UserTypes.User
                                        ? Db.Users.Where(a => a.UserId == rfh.OpId)
                                            .Select(a => new
                                            {
                                                IsUser = true,
                                                Id = a.UserId,
                                                Name = a.UName,
                                                FileUrl = Url.AdminGetUserUrl(Request, a.FileUrl)
                                            }).FirstOrDefault()
                                        : rfh.OpFlag == UserTypes.Admin
                                            ? Db.Admins
                                                .Where(a => a.Id == rfh.OpId)
                                                .Select(a => new
                                                {
                                                    IsUser = false,
                                                    Id = (long)a.Id,
                                                    Name = a.Name,
                                                    FileUrl = Url.AdminGetAdminUrl(Request, a.FileUrl)
                                                }).FirstOrDefault()
                                            : null,
                                    rfh.Status,
                                    rfh.Info,
                                    CreateDate = rfh.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")
                                })
                            .ToList()
                    })
                .AsEnumerable()
                .Select(r => new
                {
                    r.Id,
                    r.Author.AIsUser,
                    r.Author.AName,
                    AFileUrl = Url.AdminGetUsersUrl(Request, r.Author.AIsUser, r.Author.AFileUrl),
                    r.Title,
                    r.TId,
                    r.CreateDate,
                    r.IdCategory,
                    Content = r.Content.Replace("\"fileUrl\":\"", "\"fileUrl\":\"" + r.IdCategory switch
                    {
                        IdCategory.Ingredient => ingredientPath,
                        IdCategory.Recipe => recipePath,
                        IdCategory.Collection => collectionPath,
                        _ => ""
                    }),
                    r.ReleaseInfo,
                    FileUrl = Url.AdminGetUrl(Request, r.IdCategory, r.FileUrl),
                    r.ReleaseFlowHistories
                }).ToList();

            if (obj.Count == 0)
            {
                return ApiResponses.Error("没有找到此发布记录");
            }

            if (release.TId == -1)
            {
                return ApiResponses.Success("请求成功", new { Now = (object)null, List = obj });
            }

            switch (release.IdCategory)
            {
                case IdCategory.Recipe:
                {
                    var recipe = (from r in Db.Recipes
                        where r.RecipeId == release.TId
                        select new
                        {
                            r.RecipeId,
                            r.Title,
                            r.RName,
                            FileUrl = Url.AdminGetRecipeUrl(Request, r.FileUrl),
                            r.Summary,
                            r.CreateDate,
                        }).FirstOrDefault();

                    if (recipe == null)
                        return ApiResponses.Error("没有找到此食谱");

                    var steps = Db.PreparationSteps
                        .Where(step => step.RecipeId == release.TId)
                        .OrderBy(step => step.SequenceNumber)
                        .Select(step => new
                        {
                            Id = step.PreparationStepId,
                            step.Title,
                            FileUrl = Url.AdminGetRecipeUrl(Request, step.FileUrl),
                            step.Refer,
                            step.RequiredTime,
                            step.RequiredIngredient,
                            step.Summary
                        }).ToList();

                    var ingredients = (from item in Db.RecipeItems
                        where item.RecipeId == release.TId
                        select new
                        {
                            item.IngredientId,
                            item.Dosage,
                        }).ToList();

                    var categories = (from ci in db.CategoryItems
                        join c in db.Categories on ci.CategoryId equals c.CategoryId
                        where ci.UserId == UserRoles.CategoryAdmin
                              && ci.IdCategory == IdCategory.Recipe
                              && ci.TId == dto.Id
                        select c.CName).ToList();

                    return ApiResponses.Success("获取发布详细信息成功", new
                    {
                        CreateDate = recipe.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                        Now = new
                        {
                            recipe.RecipeId,
                            recipe.Title,
                            recipe.RName,
                            recipe.FileUrl,
                            recipe.Summary,
                            Steps = steps,
                            Ingredients = ingredients,
                            Categories = categories
                        },
                        List = obj
                    });
                }
                case IdCategory.Ingredient:
                {
                    var i = Db.Ingredients.FirstOrDefault(i => i.IngredientId == release.TId);
                    if (i is null) return ApiResponses.Error("食材不存在");

                    var list = (from inu in Db.IngredientNutritionals
                        where inu.IngredientId == i.IngredientId
                        join n in Db.Nutrients on inu.NutritionalId equals n.Id
                        select new
                        {
                            n.Id,
                            n.Name,
                            n.Unit,
                            Value = inu.Value
                        }).ToDictionary(i => i.Id, i => i.Value);

                    var result = new
                    {
                        CreateDate = i.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                        Now = new
                        {
                            i.IngredientId,
                            FileUrl = Url.AdminGetIngredientUrl(Request, i.FileUrl),
                            i.IName,
                            i.Refer,
                            i.Unit,
                            Quantity = i.Quantity.ToEntity<Dictionary<string, decimal>>(),
                            i.Allergy,
                            i.Content,
                            Nutrients = list
                        },
                        List = obj
                    };

                    return ApiResponses.Success("获取食材详细信息成功", result);
                }
                case IdCategory.Collection:
                {
                    var collection = Db.Collections.FirstOrDefault(c => c.CollectionId == release.TId);
                    if (collection is null) return ApiResponses.Error("合集不存在");
                    var result2 = new
                    {
                        CreateDate = collection.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                        Now = new
                        {
                            collection.CollectionId,
                            FileUrl = Url.AdminGetCollectionUrl(Request, collection.FileUrl),
                            collection.Title,
                            collection.Summary,
                            Content = collection.Content.ToEntity<HtmlData>()
                        },
                        List = obj
                    };
                    return ApiResponses.Success("获取食材详细信息成功", result2);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult NeedEditRelease(ReleaseStatusDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;

            var release = Db.Releases.FirstOrDefault(r => r.ReleaseId == dto.Id);
            if (release is null) return ApiResponses.Error("没有找到此发布记录");

            var history = Db.GetNowHistory(release.ReleaseId);

            switch (history?.Status)
            {
                case Status.Approve:
                    return Db.TransactionScope(() =>
                    {
                        Db.ReleaseFlowHistories.Add(new ReleaseFlowHistory
                        {
                            ReleaseId = dto.Id,
                            OpId = dto.AdminId,
                            CreateDate = DateTime.Now,
                            OpFlag = UserTypes.Admin,
                            Info = "已锁定 后续有其他操作",
                            Status = Status.Locked
                        });
                        var release2 = new Release
                        {
                            AuthorId = release.AuthorId,
                            OpFlag = release.OpFlag,
                            TId = release.TId,
                            IdCategory = release.IdCategory,
                            Content = release.Content,
                            ReleaseInfo = "",
                            Title = release.Title,
                            FileUrl = release.FileUrl,
                            CreateDate = DateTime.Now
                        };
                        Db.Releases.Add(release2);
                        Db.SaveChanges();
                        Db.ReleaseFlowHistories.Add(new ReleaseFlowHistory
                        {
                            ReleaseId = release2.ReleaseId,
                            OpId = dto.AdminId,
                            CreateDate = DateTime.Now,
                            OpFlag = UserTypes.Admin,
                            Info = "需修改，" + dto.Info,
                            Status = Status.NeedEdit
                        });
                    }, "新建审核成功", "新建审核失败");
                case Status.Pending or Status.Confirm:
                {
                    var flowHistory = new ReleaseFlowHistory
                    {
                        ReleaseId = dto.Id,
                        OpId = dto.AdminId,
                        CreateDate = DateTime.Now,
                        OpFlag = UserTypes.Admin,
                        Info = "需修改，" + dto.Info,
                        Status = Status.NeedEdit
                    };

                    Db.ReleaseFlowHistories.Add(flowHistory);

                    return Db.SaveChanges() == 1
                        ? ApiResponses.Success("提交成功")
                        : ApiResponses.Error("提交失败");
                }
                default:
                    return ApiResponses.Success("该审核请求操作无效");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult ConfirmRelease(AdminUserDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;

            var release = (from r in Db.Releases where r.ReleaseId == dto.Id select r).FirstOrDefault();
            if (release is null)
            {
                return ApiResponses.Error("没有找到此发布记录");
            }

            var history = Db.GetNowHistory(release.ReleaseId);

            if (history is null || history.Status is not Status.Pending)
            {
                return ApiResponses.Success("该审核请求操作无效");
            }

            var flowHistory = new ReleaseFlowHistory
            {
                ReleaseId = dto.Id,
                OpId = dto.AdminId,
                CreateDate = DateTime.Now,
                OpFlag = UserTypes.Admin,
                Info = "待终审通过",
                Status = Status.Confirm
            };

            Db.ReleaseFlowHistories.Add(flowHistory);

            return Db.SaveChanges() == 1
                ? ApiResponses.Success("提交成功")
                : ApiResponses.Error("提交失败");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult RejectRelease(ReleaseStatusDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;

            var release = Db.Releases.FirstOrDefault(r => r.ReleaseId == dto.Id);
            if (release is null)
            {
                return ApiResponses.Error("没有找到此发布记录");
            }

            var history = Db.GetNowHistory(release.ReleaseId);

            if (history?.Status is not (Status.Pending or Status.Approve or Status.Confirm or Status.NeedEdit))
            {
                return ApiResponses.Success("该审核请求操作无效");
            }

            var flowHistory = new ReleaseFlowHistory
            {
                ReleaseId = dto.Id,
                OpId = dto.AdminId,
                CreateDate = DateTime.Now,
                OpFlag = UserTypes.Admin,
                Info = "已被驳回，" + dto.Info,
                Status = Status.Reject
            };

            object obj = null;

            if (release.TId > 0)
            {
                switch (release.IdCategory)
                {
                    case IdCategory.Recipe:
                        var recipe = Db.Recipes.FirstOrDefault(r => r.RecipeId == release.TId);
                        if (recipe is null) return ApiResponses.Error("原食谱不存在");
                        recipe.Status = Status.ForceOff;
                        obj = recipe;
                        break;
                    case IdCategory.Ingredient:
                        var ingredient = Db.Ingredients.FirstOrDefault(i => i.IngredientId == release.TId);
                        if (ingredient is null) return ApiResponses.Error("原食材不存在");
                        ingredient.Status = Status.ForceOff;
                        obj = ingredient;
                        break;
                    case IdCategory.Collection:
                        var collection = Db.Collections.FirstOrDefault(i => i.CollectionId == release.TId);
                        if (collection is null) return ApiResponses.Error("原合集不存在");
                        collection.Status = Status.ForceOff;
                        obj = collection;
                        break;
                }
            }

            return Db.TransactionScope(() =>
            {
                Db.ReleaseFlowHistories.Add(flowHistory);
                if (release.TId <= 0) return;
                switch (release.IdCategory)
                {
                    case IdCategory.Recipe:
                        Db.Recipes.Update((Recipe)obj);
                        break;
                    case IdCategory.Ingredient:
                        Db.Ingredients.Update((Ingredient)obj);
                        break;
                    case IdCategory.Collection:
                        Db.Collections.Update((Collection)obj);
                        break;
                }
            }, "提交成功", "提交失败");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }


    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult ApproveRelease(AdminUserDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;

            var submit = new ApprovedContext
            {
                Db = Db,
                Url = Url,
                ReleaseId = dto.Id,
                OpId = dto.AdminId,
            };
            if (submit.SubmitApproved(out responses, out var fun))
                return responses;

            return Db.TransactionScope(() => { fun?.Invoke(); }, "通过", "不通过");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult GetReleaseRecipe(AdminUserDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;

            var release = (from r in Db.Releases
                where r.ReleaseId == dto.Id && r.IdCategory == IdCategory.Recipe
                select r).FirstOrDefault();
            if (release == null)
            {
                return ApiResponses.Error("没有找到此发布记录");
            }

            var recipeModel = release.Content.ToEntity<AddRecipeModel>();
            recipeModel.FileUrl = Url.AdminGetRecipeUrl(Request, recipeModel.FileUrl);
            foreach (var step in recipeModel.Steps)
                step.FileUrl = Url.AdminGetRecipeUrl(Request, step.FileUrl);

            var ingredientIds = recipeModel.Ingredients
                .ToDictionary(i => i.IngredientId, i => i.Dosage);

            var ingredients = (from i in Db.Ingredients
                    where ingredientIds.Keys.Contains(i.IngredientId)
                    select new
                    {
                        i.IngredientId,
                        i.IName,
                        i.Unit,
                        i.Quantity,
                        i.FileUrl,
                        Dosage = ingredientIds[i.IngredientId],
                        Nutritional = (from ii in db.IngredientNutritionals
                            where ii.IngredientId == i.IngredientId
                            join n in db.Nutrients on ii.NutritionalId equals n.Id
                            select new
                            {
                                n.Id,
                                n.Name,
                                Value = (decimal)ii.Value,
                                n.Unit
                            }).ToList(),
                    })
                .AsEnumerable()
                .Select(i => new
                {
                    i.IngredientId,
                    i.IName,
                    i.Unit,
                    Nutritional = i.Nutritional.ToDictionary(n => n.Id, n => n),
                    FileUrl = Url.AdminGetIngredientUrl(Request, i.FileUrl),
                    i.Dosage
                }).ToList();

            var categories = (from ci in db.CategoryItems
                join c in db.Categories on ci.CategoryId equals c.CategoryId
                where ci.UserId == UserRoles.CategoryAdmin
                      && ci.IdCategory == IdCategory.Ingredient
                      && ci.TId == dto.Id
                select c.CName).ToList();

            return ApiResponses.Success("获取食谱详细信息成功", new
            {
                recipeModel.RecipeId,
                recipeModel.FileUrl,
                recipeModel.Title,
                recipeModel.RName,
                recipeModel.Summary,
                recipeModel.Steps,
                Ingredients = ingredients,
                Categories = categories
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
    public IActionResult GetReleaseIngredient(AdminUserDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;

            var release = (from r in Db.Releases
                where r.ReleaseId == dto.Id && r.IdCategory == IdCategory.Ingredient
                select r).FirstOrDefault();
            if (release == null)
            {
                return ApiResponses.Error("没有找到此发布记录");
            }

            var ingredient = release.Content.ToEntity<AddIngredientModel>();
            ingredient.FileUrl = Url.AdminGetIngredientUrl(Request, ingredient.FileUrl);

            var list = (from n in Db.Nutrients
                where ingredient.Nutrients.Keys.Contains(n.Id)
                select new
                {
                    n.Id,
                    n.Name,
                    n.Unit,
                    Value = ingredient.Nutrients.GetValueOrDefault(n.Id, 0)
                }).ToList();

            var result = new
            {
                ingredient.IngredientId,
                ingredient.FileUrl,
                ingredient.IName,
                ingredient.Refer,
                ingredient.Unit,
                ingredient.Quantity,
                ingredient.Allergy,
                ingredient.Content,
                Nutritional = list,
            };
            return ApiResponses.Success("请求成功", result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult GetReleaseCollection(AdminUserDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;

            var release = (from r in Db.Releases
                where r.ReleaseId == dto.Id && r.IdCategory == IdCategory.Collection
                select r).FirstOrDefault();
            if (release == null)
            {
                return ApiResponses.Error("没有找到此发布记录");
            }

            var collection = release.Content.ToEntity<AddCollectionModel>();
            collection.FileUrl = Url.AdminGetCollectionUrl(Request, collection.FileUrl);

            var images = collection.Content.Images
                .Select(i => new
                {
                    Id = i,
                    Url = Url.AdminGetCollectionUrl(Request, i)
                }).ToList();

            var result = CollectionService.GetTabs(Db, Url, Request, collection.Content.Dirs);

            return ApiResponses.Success("获取合集详细信息成功", new
            {
                collection.FileUrl,
                collection.Summary,
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
}

public record ReleaseStatusDto : AdminUserDto
{
    [MaxLength(90, ErrorMessage = "请求错误,信息不得超过{0}")]
    public string? Info { get; set; }
}