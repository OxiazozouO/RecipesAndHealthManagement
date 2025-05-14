using System.Security.Policy;
using AnyLibrary.Constants;
using Microsoft.AspNetCore.Mvc;
using WebServer.Controllers.Admin;
using WebServer.DatabaseModel;
using WebServer.Helper;

namespace WebServer.Controllers.Service;

public static class CollectionService
{
    public static List<object> GetTabs(RecipeAndHealthSystemContext db, IUrlHelper url, HttpRequest request,
        Dictionary<sbyte, HashSet<long>> dirs, bool isAdmin = true)
    {
        List<object> result = [];
        foreach (var (idc, ids) in dirs)
        {
            if (ids is null || ids.Count == 0) continue;
            switch (idc)
            {
                case IdCategory.Ingredient:
                    var ingredients = db.Ingredients.Where(i => ids.Contains(i.IngredientId))
                        .Select(i => new
                        {
                            IdCategory = (int)IdCategory.Ingredient,
                            Id = i.IngredientId,
                            i.FileUrl,
                            Title = i.IName,
                            Refer = i.Refer
                        }).AsEnumerable()
                        .Select(i => i with
                        {
                            FileUrl = isAdmin
                                ? url.AdminGetIngredientUrl(request, i.FileUrl)
                                : url.GetIngredientUrl(request, i.FileUrl)
                        }).ToList();
                    result.AddRange(ingredients);
                    break;
                case IdCategory.Recipe:
                    var recipes = db.Recipes.Where(i => ids.Contains(i.RecipeId))
                        .Select(i => new
                        {
                            IdCategory = (int)IdCategory.Recipe,
                            Id = i.RecipeId,
                            i.FileUrl,
                            Title = i.RName,
                            Refer = i.Summary
                        }).AsEnumerable()
                        .Select(i => i with
                        {
                            FileUrl = isAdmin
                                ? url.AdminGetRecipeUrl(request, i.FileUrl)
                                : url.GetRecipeUrl(request, i.FileUrl)
                        })
                        .ToList();
                    result.AddRange(recipes);
                    break;
                case IdCategory.Collection:
                    var collections = db.Collections.Where(i => ids.Contains(i.CollectionId))
                        .Select(i => new
                        {
                            IdCategory = (int)IdCategory.Collection,
                            Id = i.CollectionId,
                            i.FileUrl,
                            Title = i.Title,
                            Refer = i.Summary
                        }).AsEnumerable()
                        .Select(i => i with
                        {
                            FileUrl = isAdmin
                                ? url.AdminGetCollectionUrl(request, i.FileUrl)
                                : url.GetCollectionUrl(request, i.FileUrl)
                        })
                        .ToList();
                    result.AddRange(collections);
                    break;
            }
        }

        return result;
    }

    public static dynamic GetImages(IUrlHelper url, HttpRequest request, List<string> images, bool isAdmin = true)
    {
        return images.Select(i => new
        {
            Id = i,
            Url = isAdmin
                ? url.AdminGetCollectionUrl(request, i)
                : url.GetCollectionUrl(request, i)
        }).ToList();
    }
}