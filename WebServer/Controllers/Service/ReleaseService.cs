using AnyLibrary.Constants;
using AnyLibrary.Helper;
using Microsoft.AspNetCore.Mvc;
using WebServer.DatabaseModel;
using WebServer.DTOs;
using WebServer.Helper;
using WebServer.Http;
using static AnyLibrary.Constants.IdCategory;

namespace WebServer.Controllers.Service;

public static class ReleaseService
{
    public static bool TestIngredient(this IUrlHelper url, AddIngredientModel model, out IActionResult result)
    {
        bool all = url.TryReplaceFile(FileUrlHelper.Ingredients, model.FileUrl, s => model.FileUrl = s);
        result = all ? null : ApiResponses.Error("文件上传失败");
        return !all;
    }

    public static bool TestRecipe(this IUrlHelper url, AddRecipeModel recipe, out IActionResult result)
    {
        bool all = url.TryReplaceFile(FileUrlHelper.Recipes, recipe.FileUrl, s => recipe.FileUrl = s)
                   && recipe.Steps.All(step =>
                       url.TryReplaceFile(FileUrlHelper.Recipes, step.FileUrl, s => step.FileUrl = s));

        result = all ? null : ApiResponses.Error("文件上传失败");
        return !all;
    }

    public static bool TestCollection(this IUrlHelper url, AddCollectionModel collection, out IActionResult result)
    {
        result = null;
        bool all = url.TryReplaceFile(FileUrlHelper.Collections, collection.FileUrl, s => collection.FileUrl = s);
        for (var i = 0; i < collection.Content.Images.Count; i++)
        {
            var images = collection.Content.Images;
            var ind = i;
            if (url.TryReplaceFile(FileUrlHelper.Collections, images[ind], s => images[ind] = s))
                continue;
            all = false;
            break;
        }

        result = all ? null : ApiResponses.Error("文件上传失败");
        return false;
    }

    public static ReleaseFlowHistory? GetNowHistory(this RecipeAndHealthSystemContext db, long? releaseId)
    {
        if (releaseId is null) return null;
        var history = (from f in db.ReleaseFlowHistories
            where f.ReleaseId == releaseId
            orderby f.CreateDate descending
            select f).FirstOrDefault();
        return history;
    }

    public static bool EditorEnt(this EditorContext cfg, out IActionResult result, out Action fun)
    {
        fun = null;
        result = null;
        ReleaseFlowHistory? history = null;
        if (cfg is { OpFlag: UserTypes.User, TargetType: IdCategory.Ingredient })
        {
            result = ApiResponses.Error("非法修改");
            return true;
        }

        if (cfg.Test(out result))
            return true;

        Release? release = cfg.GetRelease();
        cfg.InitModel();
        var model = cfg.GetDbModel();

        // 1 √食材 √发布 -> 修改
        // 2 √食材 ×发布 -> 修改
        // 3 ×食材 √发布 -> 修改
        // 4 ×食材 ×发布 -> 添加
        if (release == null && model == null)
        {
            release = cfg.MapTo(() => new Release { ReleaseInfo = "后台自动添加" });

            history = new ReleaseFlowHistory
            {
                // ReleaseId = release.ReleaseId,
                OpId = cfg.OpId,
                OpFlag = cfg.OpFlag,
                Info = $"新增{GetName(cfg.TargetType)}",
                CreateDate = DateTime.Now,
                Status = Status.Pending
            };

            fun = () =>
            {
                cfg.Db.Releases.Add(release);
                cfg.Db.SaveChanges();
                history.ReleaseId = release.ReleaseId;
                cfg.Db.ReleaseFlowHistories.Add(history);
            };
        }
        else if (release != null && model == null)
        {
            history = cfg.Db.GetNowHistory(release.ReleaseId);

            if (IsValidHistoryStatus(cfg.OpFlag, history))
            {
                result = ApiResponses.Success("该审核请求操作无效");
                return true;
            }

            if (cfg.CompareObjects(release.Content))
            {
                result = ApiResponses.Error("两次提交的内容相同");
                return true;
            }

            //修改release
            release.Content = cfg.GetModel().ToJson();
            if (!string.IsNullOrEmpty(cfg.Info))
                release.ReleaseInfo = cfg.Info;

            history = new ReleaseFlowHistory
            {
                // ReleaseId = release.ReleaseId,
                OpId = cfg.OpId,
                OpFlag = cfg.OpFlag,
                Status = Status.Pending,
                Info = "修改审核"
            };

            fun = () =>
            {
                cfg.Db.Releases.Update(release);
                history.ReleaseId = release.ReleaseId;
                cfg.Db.ReleaseFlowHistories.Add(history);
            };
        }
        else if (release == null && model != null)
        {
            if (IsValidEntStatus(cfg.OpFlag, cfg.GetStatus()))
            {
                result = ApiResponses.Error("该食材状态不允许修改");
                return true;
            }

            if (cfg.CompareObjects()) //@ 食材 食谱 合集
            {
                result = ApiResponses.Error("与原内容相同");
                return true;
            }

            //新增release 和 修改ingredient
            release = cfg.MapTo(() => new Release
            {
                ReleaseInfo = "重新修改审核",
            });
            history = new ReleaseFlowHistory
            {
                // ReleaseId = release.ReleaseId,
                OpId = cfg.OpId,
                OpFlag = cfg.OpFlag,
                Status = Status.Pending,
                Info = "修改审核"
            };

            var up = cfg.SetStatus(Status.Pending);

            fun = () =>
            {
                cfg.Db.Releases.Add(release);
                cfg.Db.SaveChanges();
                history.ReleaseId = release.ReleaseId;
                cfg.Db.ReleaseFlowHistories.Add(history);
                up();
            };
        }
        else if (release != null && model != null)
        {
            if (cfg.OpFlag == UserTypes.User && release.AuthorId != cfg.OpId)
            {
                result = ApiResponses.Error("无权修改");
                return true;
            }

            history = cfg.Db.GetNowHistory(release.ReleaseId);
            if (IsValidHistoryStatus(cfg.OpFlag, history))
            {
                result = ApiResponses.Success("该审核请求操作无效");
                return true;
            }

            if (IsValidEntStatus(cfg.OpFlag, cfg.GetStatus()))
            {
                result = ApiResponses.Error("该食材状态不允许修改");
                return true;
            }

            if (cfg.CompareObjects())
            {
                result = ApiResponses.Error("与原内容相同");
                return true;
            }

            if (cfg.CompareObjects(release.Content))
            {
                result = ApiResponses.Error("两次提交的内容相同");
                return true;
            }

            //修改release 和 ingredient
            var up = cfg.SetStatus(Status.Pending);
            release.Content = cfg.GetModel().ToJson();
            if (!string.IsNullOrEmpty(cfg.Info))
                release.ReleaseInfo = cfg.Info;

            history = new ReleaseFlowHistory
            {
                ReleaseId = release.ReleaseId,
                OpId = cfg.OpId,
                OpFlag = cfg.OpFlag,
                Status = Status.Pending,
                Info = "修改审核"
            };

            fun = () =>
            {
                cfg.Db.Releases.Update(release);
                cfg.Db.ReleaseFlowHistories.Add(history);
                up();
            };
        }

        return false;
    }

    public static bool SubmitApproved(this ApprovedContext cfg, out IActionResult result, out Action fun)
    {
        result = null;
        Action f = null;
        fun = null;
        var release = cfg.Db.Releases.FirstOrDefault(r => r.ReleaseId == cfg.ReleaseId);
        cfg.release = release;
        if (release is null)
        {
            result = ApiResponses.Error("没有找到此发布记录");
            return true;
        }

        var history = cfg.Db.GetNowHistory(release.ReleaseId);

        if (history?.Status is not Status.Confirm)
        {
            result = ApiResponses.Success("该审核请求操作无效");
            return true;
        }

        var flowHistory = new ReleaseFlowHistory
        {
            ReleaseId = cfg.ReleaseId,
            OpId = cfg.OpId,
            CreateDate = DateTime.Now,
            OpFlag = UserTypes.Admin,
            Info = "已通过",
            Status = Status.Approve
        };
        cfg.release = release;
        cfg.TargetType = release.IdCategory;
        cfg.InitEnt();//根据类别初始化JSON为模型数据
        if (cfg.Test(out result))//验证上传文件是否有效
            return true;
        if (release.TId == -1)
        {
            cfg.MapTo();//新建并把模型数据映射到 食材 食谱 合集 数据库实体里
            f = cfg.GetFun();//多表插入和修改数据
            goto res;
        }
        if (cfg.GetDbEnt())//获取已有 食材 食谱 合集 数据库实体
        {
            result = ApiResponses.Error($"原{GetName(cfg.TargetType)}已存在");
            return true;
        }
        cfg.UpDate();//把模型数据映射到 食材 食谱 合集 数据库实体里
        f = cfg.BuildSql();//对已有 食材 食谱 合集 数据库实体进行综合的删除、添加、修改
        res:
        fun = () => { f(); cfg.Db.ReleaseFlowHistories.Add(flowHistory); };
        return false;
    }

    public static object GetReleaseRecipeContent(this RecipeAndHealthSystemContext db, Recipe recipe)
    {
        var steps = (from s in db.PreparationSteps
            where s.RecipeId == recipe.RecipeId
            orderby s.SequenceNumber
            select new
            {
                Id = s.PreparationStepId,
                s.Title,
                s.FileUrl,
                s.Refer,
                s.RequiredTime,
                s.RequiredIngredient,
                s.Summary
            }).ToList();

        var ingredients = (from item in db.RecipeItems
            join i in db.Ingredients on item.IngredientId equals i.IngredientId
            where item.RecipeId == recipe.RecipeId
            select new
            {
                i.IngredientId,
                item.Dosage,
            }).ToList();

        return new
        {
            recipe.RecipeId,
            recipe.Title,
            recipe.RName,
            recipe.FileUrl,
            recipe.Summary,
            Steps = steps,
            Ingredients = ingredients
        };
    }

    public static object GetReleaseIngredientContent(this RecipeAndHealthSystemContext db, Ingredient ingredient)
    {
        var dir = (from inu in db.IngredientNutritionals
            where inu.IngredientId == ingredient.IngredientId
            select new
            {
                inu.Id,
                Value = inu.Value
            }).ToDictionary(i => i.Id, i => i.Value);

        return new
        {
            ingredient.IngredientId,
            ingredient.FileUrl,
            ingredient.IName,
            ingredient.Unit,
            ingredient.Content,
            ingredient.Quantity,
            ingredient.Refer,
            ingredient.Allergy,
            Nutrients = dir
        };
    }

    public static object GetReleaseCollectionContent(this RecipeAndHealthSystemContext db, Collection c)
    {
        return new { c.CollectionId, c.FileUrl, c.Title, c.Summary, c.Content };
    }

    public static bool IsValidEntStatus(int opFlag, int? status)
    {
        if (status == null) return true;
        if (opFlag == UserTypes.User)
        {
            if (status is not (Status.Pending or Status.NeedEdit or Status.Approve))
            {
                return true;
            }
        }

        if (status is Status.Deleted or Status.Locked or Status.Cancel)
        {
            return true;
        }

        return false;
    }

    public static bool IsValidHistoryStatus(int opFlag, ReleaseFlowHistory? history)
    {
        if (history is null) return true;
        var status = history.Status;
        if (opFlag == UserTypes.User)
        {
            if (status is not (Status.Pending or Status.NeedEdit or Status.Approve))
            {
                return true;
            }
        }
        else if (opFlag == UserTypes.Admin)
        {
            if (status is not (Status.Pending or Status.NeedEdit or Status.Approve or Status.ForceOff))
                return true;
        }

        return false;
    }
}

public class EditorContext
{
    // 基础参数
    public RecipeAndHealthSystemContext Db;
    public IUrlHelper Url;
    public long ReleaseId = -1;
    public long OpId;
    public sbyte OpFlag;
    public string? Info;

    public AddIngredientModel I = null;
    public AddRecipeModel R = null;
    public AddCollectionModel C = null;
    private Ingredient? _ii = null;
    private Recipe? _rr = null;
    private Collection? _cc = null;

    private Release release;

    public sbyte TargetType = -1;


    public bool Test(out IActionResult result)
    {
        switch (TargetType)
        {
            case IdCategory.Ingredient:
                return Url.TestIngredient(I, out result);
            case IdCategory.Recipe:
                return Url.TestRecipe(R, out result);
            case IdCategory.Collection:
                return Url.TestCollection(C, out result);
            default:
                result = ApiResponses.Error("未知类型");
                return true;
        }
    }

    public Release? GetRelease()
    {
        release = ReleaseId != -1 ? Db.Releases.FirstOrDefault(r => r.ReleaseId == ReleaseId) : null;
        return release;
    }

    public object? GetModel()
    {
        return TargetType switch
        {
            IdCategory.Recipe => R,
            IdCategory.Ingredient => I,
            IdCategory.Collection => C,
            _ => throw new NotImplementedException()
        };
    }

    public long GetId()
    {
        return TargetType switch
        {
            IdCategory.Recipe => R.RecipeId,
            IdCategory.Ingredient => I.IngredientId,
            IdCategory.Collection => C.CollectionId,
            _ => -1
        };
    }

    public object? GetDbModel()
    {
        return TargetType switch
        {
            IdCategory.Recipe => _rr,
            IdCategory.Ingredient => _ii,
            IdCategory.Collection => _cc,
            _ => throw new NotImplementedException()
        };
    }

    public bool CompareObjects(object? obj = null)
    {
        obj ??= TargetType switch
        {
            IdCategory.Recipe => Db.GetReleaseRecipeContent(_rr),
            IdCategory.Ingredient => Db.GetReleaseIngredientContent(_ii),
            IdCategory.Collection => Db.GetReleaseCollectionContent(_cc),
            _ => throw new NotImplementedException()
        };
        return GetModel().CompareObjects(obj, TargetType switch
        {
            IdCategory.Recipe => "Steps:false",
            IdCategory.Collection => "Items:false",
            _ => "Nutrients:false"
        });
    }

    public int? GetStatus()
    {
        return TargetType switch
        {
            IdCategory.Recipe => _rr?.Status,
            IdCategory.Ingredient => _ii?.Status,
            IdCategory.Collection => _cc?.Status,
            _ => throw new NotImplementedException()
        };
    }

    public void InitModel()
    {
        switch (TargetType)
        {
            case IdCategory.Recipe when R.RecipeId != -1:
                _rr = Db.Recipes.FirstOrDefault(a => a.RecipeId == R.RecipeId);
                break;
            case IdCategory.Ingredient when I.IngredientId != -1:
                _ii = Db.Ingredients.FirstOrDefault(a => a.IngredientId == I.IngredientId);
                break;
            case IdCategory.Collection when C.CollectionId != -1:
                _cc = Db.Collections.FirstOrDefault(a => a.CollectionId == C.CollectionId);
                break;
        }
    }

    public Release MapTo(Func<Release> fun)
    {
        var re = fun();
        re.AuthorId = OpId;
        re.OpFlag = OpFlag;
        re.TId = GetId();
        re.IdCategory = TargetType;
        re.CreateDate = DateTime.Now;
        re.ReleaseInfo = string.IsNullOrEmpty(Info) ? re.ReleaseInfo : Info;
        switch (TargetType)
        {
            case IdCategory.Ingredient:
                I.MapTo(() => re);
                break;
            case IdCategory.Collection:
                C.MapTo(() => re);
                break;
            case IdCategory.Recipe:
                R.MapTo(() => re);
                break;
            default: throw new NotImplementedException();
        }

        return re;
    }

    public Action SetStatus(sbyte status)
    {
        switch (TargetType)
        {
            case IdCategory.Ingredient:
                _ii.Status = status;
                return () => Db.Ingredients.Update(_ii);
            case IdCategory.Collection:
                _cc.Status = status;
                return () => Db.Collections.Update(_cc);
            case IdCategory.Recipe:
                _rr.Status = status;
                return () => Db.Recipes.Update(_rr);
            default:
                throw new NotImplementedException();
        }

        return () => { };
    }

    public string GetEntityName() => TargetType switch
    {
        IdCategory.Ingredient => "食材",
        IdCategory.Recipe => "食谱",
        IdCategory.Collection => "合集",
        _ => throw new NotImplementedException()
    };
}

public class ApprovedContext
{
    public RecipeAndHealthSystemContext Db;
    public IUrlHelper Url;
    public long ReleaseId;
    public long OpId;
    public int TargetType;
    public Release release;

    private AddRecipeModel r;
    private AddIngredientModel i;
    private AddCollectionModel c;
    public Recipe? Rr;
    public Ingredient? Ii;
    public Collection? Cc;

    public void InitEnt()
    {
        switch (TargetType)
        {
            case IdCategory.Collection:
                c = release.Content.ToEntity<AddCollectionModel>();
                break;
            case IdCategory.Recipe:
                r = release.Content.ToEntity<AddRecipeModel>();
                break;
            case IdCategory.Ingredient:
                i = release.Content.ToEntity<AddIngredientModel>();
                break;
            default:
                throw new NotImplementedException();
        }
    }

    public bool Test(out IActionResult result)
    {
        switch (TargetType)
        {
            case IdCategory.Ingredient:
                return Url.TestIngredient(i, out result);
            case IdCategory.Recipe:
                return Url.TestRecipe(r, out result);
            case IdCategory.Collection:
                return Url.TestCollection(c, out result);
            default:
                result = ApiResponses.Error("未知类型");
                return true;
        }
    }

    public void MapTo()
    {
        switch (TargetType)
        {
            case IdCategory.Ingredient:
                Ii = i.MapTo(() => new Ingredient
                {
                    UserId = release.AuthorId,
                    Status = Status.Off,
                    CreateDate = DateTime.Now
                });
                break;
            case IdCategory.Recipe:
                Rr = r.MapTo(() => new Recipe
                {
                    AuthorId = release.AuthorId,
                    Status = Status.Off,
                    CreateDate = DateTime.Now
                });
                break;
            case IdCategory.Collection:
                Cc = c.MapTo(() => new Collection
                {
                    UserId = release.AuthorId,
                    CreateDate = DateTime.Now,
                    Status = Status.Off
                });
                break;
            default:
                throw new NotImplementedException();
        }
    }

    public void UpDate()
    {
        switch (TargetType)
        {
            case IdCategory.Ingredient:
                i.MapTo(() =>
                {
                    Ii.Status = Status.Off;
                    Ii.CreateDate = DateTime.Now;
                    return Ii;
                });
                break;
            case IdCategory.Recipe:
                r.MapTo(() =>
                {
                    Rr.Status = Status.Off;
                    Rr.CreateDate = DateTime.Now;
                    return Rr;
                });
                break;
            case IdCategory.Collection:
                c.MapTo(() =>
                {
                    Cc.Status = Status.Off;
                    Cc.CreateDate = DateTime.Now;
                    return Cc;
                });
                break;
            default:
                throw new NotImplementedException();
        }
    }

    public Action GetFun()
    {
        switch (TargetType)
        {
            case IdCategory.Ingredient:
                return () =>
                {
                    Db.Ingredients.Add(Ii);
                    Db.SaveChanges();
                    release.TId = Ii.IngredientId;
                    i.IngredientId = Ii.IngredientId;
                    release.Content = i.ToJson();
                    Db.Releases.Update(release);
                    var nutritional = i.Nutrients.Select(n => new IngredientNutritional
                    {
                        IngredientId = Ii.IngredientId,
                        NutritionalId = n.Key,
                        Value = n.Value
                    }).ToList();

                    Db.IngredientNutritionals.AddRange(nutritional);
                };
                break;
            case IdCategory.Recipe:
                var ints = Db.Categories.GetCategoryIds(r.Categories);
                return () =>
                {
                    Db.Recipes.Add(Rr);
                    Db.SaveChanges();
                    release.TId = Rr.RecipeId;
                    r.RecipeId = Rr.RecipeId;
                    var recipeItems = r.Ingredients
                        .Select(n => n.MapTo(() => new RecipeItem { RecipeId = Rr.RecipeId, }))
                        .ToList();

                    var steps = r.Steps
                        .Select((s, n) => (s, s.MapTo(() => new PreparationStep
                        {
                            RecipeId = Rr.RecipeId,
                            SequenceNumber = n + 1,
                        }))).ToDictionary(s => s.Item1, s => s.Item2);


                    var categoryItems = ints.Select(n => new CategoryItem
                    {
                        CategoryId = n,
                        UserId = release.AuthorId,
                        TId = Rr.RecipeId,
                        IdCategory = IdCategory.Recipe
                    }).ToList();

                    Db.PreparationSteps.AddRange(steps.Values);
                    Db.SaveChanges();
                    foreach (var (key, value) in steps)
                        key.Id = value.PreparationStepId;
                    Db.RecipeItems.AddRange(recipeItems);
                    release.Content = r.ToJson();
                    Db.Releases.Update(release);
                    Db.CategoryItems.AddRange(categoryItems);
                };
                break;
            case IdCategory.Collection:
                return () =>
                {
                    Db.Collections.Add(Cc);
                    Db.SaveChanges();
                    release.TId = Cc.CollectionId;
                    c.CollectionId = Cc.CollectionId;
                    release.Content = c.ToJson();
                    Db.Releases.Update(release);
                };
            default:
                throw new NotImplementedException();
        }
    }

    public bool GetDbEnt()
    {
        switch (TargetType)
        {
            case IdCategory.Ingredient:
                Ii = Db.Ingredients.FirstOrDefault(n => n.IngredientId == release.TId);
                return Ii is null;
            case IdCategory.Recipe:
                Rr = Db.Recipes.FirstOrDefault(n => n.RecipeId == release.TId);
                return Rr is null;
            case IdCategory.Collection:
                Cc = Db.Collections.FirstOrDefault(i => i.CollectionId == release.TId);
                return Cc is null;
            default:
                throw new NotImplementedException();
        }
    }

    public Action BuildSql()
    {
        switch (TargetType)
        {
            case IdCategory.Ingredient:
                return BuildIngredientSql();
            case IdCategory.Recipe:
                return BuildRecipeSql();
            case IdCategory.Collection:
                return BuildCollectionSql();
            default:
                throw new NotImplementedException();
        }
    }

    private Action BuildCollectionSql()
    {
        c.CollectionId = Cc.CollectionId;
        return () => Db.Collections.Update(Cc);
    }

    private Action BuildIngredientSql()
    {
        var ins = new DataUpdate<IngredientNutritional, int>
        {
            Tab = Db.IngredientNutritionals,
            Filter = n => n.IngredientId == Ii.IngredientId,
            Maper = n => n.NutritionalId,
        }.Build();
        foreach (var (key, value) in i.Nutrients)
        {
            if (ins.IsUpdate(key, out var item))
            {
                item.Value = value;
            }
            else
            {
                ins.Append(new IngredientNutritional
                {
                    IngredientId = Ii.IngredientId,
                    NutritionalId = key,
                    Value = value
                });
            }
        }

        i.IngredientId = Ii.IngredientId;
        return () =>
        {
            Db.Ingredients.Update(Ii);
            Db.SaveChanges();
            i.IngredientId = Ii.IngredientId;
            release.Content = i.ToJson();
            Db.Releases.Update(release);
            ins.RunSql();
        };
    }

    public Action BuildRecipeSql()
    {
        var ris = new DataUpdate<RecipeItem, long>
        {
            Tab = Db.RecipeItems,
            Filter = r => r.RecipeId == Rr.RecipeId,
            Maper = n => n.IngredientId,
        }.Build();
        foreach (var item in r.Ingredients)
        {
            if (ris.IsUpdate(item.IngredientId, out var value))
            {
                value.Dosage = item.Dosage;
            }
            else
            {
                ris.Append(new RecipeItem
                {
                    RecipeId = Rr.RecipeId,
                    IngredientId = item.IngredientId,
                    Dosage = item.Dosage
                });
            }
        }

        var pss = new DataUpdate<PreparationStep, int>
        {
            Tab = Db.PreparationSteps,
            Filter = r => r.RecipeId == Rr.RecipeId,
            Maper = n => n.SequenceNumber,
        }.Build();

        for (var i = 0; i < r.Steps.Count; i++)
        {
            var item = r.Steps[i];
            int ind = i + 1;
            if (pss.IsUpdate(ind, out var step))
            {
                pss.Append(() => item.Id = step.PreparationStepId);
                item.MapTo(() => step);
            }
            else
            {
                var s = item.MapTo(() => new PreparationStep
                {
                    RecipeId = Rr.RecipeId,
                    SequenceNumber = ind,
                });
                pss.Append(s);
                pss.Append(() => item.Id = s.PreparationStepId);
            }
        }


        var cs = new DataUpdate<CategoryItem, long>
        {
            Tab = Db.CategoryItems,
            Filter = r => r.TId == Rr.RecipeId
                          && r.UserId == Rr.AuthorId
                          && r.IdCategory == IdCategory.Recipe,
            Maper = n => n.CategoryId,
        }.Build();
        var ints = Db.Categories.GetCategoryIds(r.Categories);

        foreach (var t in ints.Where(t => !cs.IsUpdate(t, out _)))
        {
            cs.Append(new CategoryItem
            {
                CategoryId = t,
                UserId = release.AuthorId,
                TId = Rr.RecipeId,
                IdCategory = IdCategory.Recipe
            });
        }

        r.RecipeId = Rr.AuthorId;
        return () =>
        {
            pss.RunSql();
            Db.SaveChanges();
            pss.UpdateId();
            release.Content = r.ToJson();
            Db.Releases.Update(release);
            cs.RunSql();
            ris.RunSql();
            Db.Recipes.Update(Rr);
        };
    }
}