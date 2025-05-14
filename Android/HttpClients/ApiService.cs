using Android.Configurations;
using Android.Helper;
using Android.Holder;
using Android.Models;
using Android.ViewModel;
using AnyLibrary.Constants;
using AnyLibrary.Helper;

namespace Android.HttpClients;

public static class ApiService
{
    public static List<PhysicalModel> GetMyAllInfo()
    {
        var req = ApiEndpoints.MyAllInfo(new { Id = AppConfigHelper.AppConfig.Id });
        List<PhysicalModel> result = [];
        if (req.Execute(out var res))
        {
            result = res.Data.ToEntity<List<PhysicalModel>>();
        }

        return result;
    }

    public static EatingDiaryViewModel GetEatingDiaries(DateTime time)
    {
        List<EatingDiaryModel> result = [];
        var req = ApiEndpoints.GetEatingDiaries(new
        {
            Id = AppConfigHelper.AppConfig.Id,
            Time = time
        });

        if (req.Execute(out var res))
        {
            result = res.Data.ToEntity<List<EatingDiaryModel>>();
        }

        return new EatingDiaryViewModel
        {
            StartTime = time,
            EatingDiaries = result.Select(r => new EatingDiaryAtViewModel { EatingDiary = r }).ToList(),
        };
    }

    public static List<FavoriteModel> GetFavorites(int flag)
    {
        var req = ApiEndpoints.Favorites(new
        {
            Id = AppConfigHelper.AppConfig.Id,
            Flag = flag
        });

        if (!req.Execute(out var res)) return null;
        return res.Data.ToEntity<List<FavoriteModel>>();
    }

    public static List<RecipeInfoViewModel> GetRecipeList(int page)
    {
        var req = ApiEndpoints.IndexRecipeList(new
        {
            Id = AppConfigHelper.AppConfig.Id,
            PageIndex = page
        });

        if (!req.Execute(out var res)) return [];

        var list = res.Data.ToEntity<List<RecipeInfoViewModel>>();
        return list;
    }

    public static List<CollectionInfo> GetCollectionList(int page)
    {
        var req = ApiEndpoints.IndexCollectionList(new
        {
            Id = AppConfigHelper.AppConfig.Id,
            PageIndex = page
        });

        if (!req.Execute(out var res)) return [];

        var list = res.Data.ToEntity<List<CollectionInfo>>();
        return list;
    }

    public static List<IngredientInfo> GetIngredientInfoList(int flag, int page)
    {
        var req = ApiEndpoints.IndexIngredientInfoList(new
        {
            Flag = flag,
            Id = AppConfigHelper.AppConfig.Id,
            PageIndex = page
        });

        if (!req.Execute(out var res)) return [];

        var list = res.Data.ToEntity<List<IngredientInfo>>();
        return list;
    }

    public static List<IngredientInfo> SearchIngredientInfoList(string search, int flag, int page)
    {
        var req = ApiEndpoints.IndexSearchIngredientInfoList(new
        {
            Flag = flag,
            Id = AppConfigHelper.AppConfig.Id,
            PageIndex = page,
            Search = search
        });

        if (!req.Execute(out var res)) return [];

        var list = res.Data.ToEntity<List<IngredientInfo>>();
        return list;
    }

    public static List<RecipeInfoViewModel> SearchRecipeList(string search, int page)
    {
        var req = ApiEndpoints.IndexSearchRecipeList(new
        {
            Search = search,
            Id = AppConfigHelper.AppConfig.Id,
            PageIndex = page
        });

        if (!req.Execute(out var res)) return [];

        var list = res.Data.ToEntity<List<RecipeInfoViewModel>>();
        return list;
    }

    public static List<CollectionInfo> SearchCollectionList(string search, int page)
    {
        var req = ApiEndpoints.IndexSearchCollectionList(new
        {
            Search = search,
            Id = AppConfigHelper.AppConfig.Id,
            PageIndex = page
        });

        if (!req.Execute(out var res)) return [];

        var list = res.Data.ToEntity<List<CollectionInfo>>();
        return list;
    }

    // user.BirthDate,
    // user.FileUrl,
    // user.Gender,
    // user.UName
    public static MyInfoModel GetInfo(long id)
    {
        var req = ApiEndpoints.GetInfo(new { Id = id });

        if (!req.Execute(out var res))
            return new MyInfoModel();

        var list = res.Data.ToEntity<MyInfoModel>();
        return list;
    }

    public static List<ReleaseModel> GetReleases(ReleaseCategory id, int id2)
    {
        var req = id == ReleaseCategory.Collect
            ? ApiEndpoints.GetReleaseCollections(new
            {
                AppConfigHelper.AppConfig.Id,
                Flag = id2
            })
            : ApiEndpoints.GetReleaseRecipes(new
            {
                AppConfigHelper.AppConfig.Id,
                Flag = id2
            });

        if (!req.Execute(out var res)) return [];
        var models = res.Data.ToEntity<List<ReleaseModel>>();
        models.Reverse();
        return models;
    }

    public static Dictionary<long, int> GetReleaseStatus(ReleaseCategory id, HashSet<long> ids)
    {
        var req = ApiEndpoints.GetReleaseStatus(new
        {
            AppConfigHelper.AppConfig.Id,
            IdCategory = id switch
            {
                ReleaseCategory.Collect => IdCategory.Collection,
                ReleaseCategory.Recipe => IdCategory.Recipe,
                _ => -1
            },
            Ids = ids
        });

        if (!req.Execute(out var res)) return [];
        var dir = res.Data.ToEntity<Dictionary<long, int>>();
        return dir;
    }

    public static IngredientViewModel? GetIngredientAt(long id)
    {
        var req = ApiEndpoints.GetIngredient(new { UserId = AppConfigHelper.AppConfig.Id, Id = id });
        IngredientViewModel? ingredient = null;
        if (req.Execute(out var res))
            ingredient = res.Data.ToEntity<IngredientViewModel>();
        else
            MsgBoxHelper.Builder().TryError(res.Message);

        return ingredient;
    }

    public static CollectionModel? GetCollectionAt(long id)
    {
        var req = ApiEndpoints.GetCollection(new { UserId = AppConfigHelper.AppConfig.Id, Id = id });
        CollectionModel? collection = null;
        if (req.Execute(out var res))
            collection = res.Data.ToEntity<CollectionModel>();
        else
            MsgBoxHelper.Builder().TryError(res.Message);

        return collection;
    }

    public static ModelConfig GetConfig()
    {
        // @formatter:off
        var reqStr = string.Join(',',[
            nameof(ModelConfig.Nutrients                 ),
            nameof(ModelConfig.Units                     ),
            nameof(ModelConfig.BaseUnit                  ),
            nameof(ModelConfig.UnitNext                  ),
            nameof(ModelConfig.UnitPre                   ),
            nameof(ModelConfig.UnitLocal                 ),
            nameof(ModelConfig.Cals                      ),
            nameof(ModelConfig.ProteinRequirement        ),
            nameof(ModelConfig.ReferenceIntakeOfNutrients),
            nameof(ModelConfig.SDs                       )
        ]);
        // @formatter:on

        var req = ApiEndpoints.GetUserConfigs(new { Id = AppConfigHelper.AppConfig.Id, Key = reqStr });

        if (!req.Execute(out var res))
            return null;
        var model = res.Data.ToEntity<ModelConfig>();
        model.Init();
        return model;
    }

    public static RecipeModel GetRecipe(long id)
    {
        var req = ApiEndpoints.GetRecipe(new { Id = id, UserId = AppConfigHelper.AppConfig.Id });
        if (!req.Execute(out var res))
            return null;
        var recipe = res.Data.ToEntity<RecipeModel>();
        return recipe;
    }

    public static List<CommentModel> GetComments(long tid, sbyte idCategory, int pos)
    {
        var req = ApiEndpoints.GetComments(new
        {
            UserId = AppConfigHelper.AppConfig.Id,
            TId = tid,
            TypeId = idCategory,
            PageIndex = pos
        });
        if (!req.Execute(out var res))
            return null;
        var comment = res.Data.ToEntity<List<CommentModel>>();
        return comment;
    }

    public static FavoriteAtModel GetFavoriteItems(long favoriteId)
    {
        var req = ApiEndpoints.GetFavoriteItems(new
        {
            Id = AppConfigHelper.AppConfig.Id,
            FavoriteId = favoriteId
        });
        if (!req.Execute(out var res))
            return null;
        var favoriteItems = res.Data.ToEntity<FavoriteAtModel>();
        return favoriteItems;
    }


    public static bool ReverseStatus(int idCategory, long id, int status)
    {
        ApiRequest req;
        switch (idCategory)
        {
            case IdCategory.Recipe:
                req = ApiEndpoints.ReverseRecipeStatus(new
                {
                    Id = AppConfigHelper.AppConfig.Id,
                    RecipeId = id,
                    Status = status
                });
                break;
            case IdCategory.Collection:
                req = ApiEndpoints.ReverseCollectionStatus(new
                {
                    Id = AppConfigHelper.AppConfig.Id,
                    CollectionId = id,
                    Status = status
                });
                break;
            default:
                return false;
        }

        if (!req.Execute(out var res))
            return false;
        return true;
    }
}