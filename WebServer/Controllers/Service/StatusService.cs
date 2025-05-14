using AnyLibrary.Constants;
using Microsoft.AspNetCore.Mvc;
using WebServer.DatabaseModel;
using WebServer.Http;

namespace WebServer.Controllers.Service;

public static class StatusService
{
    public static bool ReverseStatus(int status, int rStatus, out int nextStatus, out IActionResult result)
    {
        result = null;
        switch (status)
        {
            case Status.Off:
            case Status.ForceOff:
            case Status.ReportOff:
                nextStatus = Status.On;
                return false;
            case Status.On:
                switch (rStatus)
                {
                    case UserTypes.Admin:
                        nextStatus = Status.ForceOff;
                        return false;
                    case UserTypes.User:
                        nextStatus = Status.Off;
                        return false;
                    default:
                        nextStatus = -1;
                        result = ApiResponses.Error("请求错误");
                        return true;
                }

                break;
            default:
                nextStatus = -1;
                result = ApiResponses.Error("请求错误");
                return true;
        }
    }

    public static IActionResult ReverseRecipeStatus(this RecipeAndHealthSystemContext db,
        long id, int status, int releaseFlowHistoryStatus)
    {
        var recipe = db.Recipes.FirstOrDefault(i => i.RecipeId == id);
        if (recipe is null) return ApiResponses.Error("食谱不存在");
        if (recipe.Status != status)
            return ApiResponses.Error("请求错误");
        if (ReverseStatus(status, releaseFlowHistoryStatus, out var nextStatus, out var res))
            return res;
        recipe.Status = nextStatus;

        db.Recipes.Update(recipe);
        return db.SaveChanges() == 1
            ? ApiResponses.Success("修改成功")
            : ApiResponses.Error("修改失败");
    }

    public static IActionResult ReverseIngredientStatus(this RecipeAndHealthSystemContext db,
        long id, int status, int releaseFlowHistoryStatus)
    {
        var ingredient = db.Ingredients.FirstOrDefault(i => i.IngredientId == id);
        if (ingredient is null) return ApiResponses.Error("食材不存在");
        if (ingredient.Status != status)
            return ApiResponses.Error("请求错误");
        if (ReverseStatus(status, releaseFlowHistoryStatus, out var nextStatus, out var res))
            return res;
        ingredient.Status = nextStatus;

        db.Ingredients.Update(ingredient);
        return db.SaveChanges() == 1
            ? ApiResponses.Success("修改成功")
            : ApiResponses.Error("修改失败");
    }

    public static IActionResult ReverseCollectionStatus(this RecipeAndHealthSystemContext db,
        long id, int status, int releaseFlowHistoryStatus)
    {
        var collection = db.Collections.FirstOrDefault(i => i.CollectionId == id);
        if (collection is null) return ApiResponses.Error("合集不存在");
        if (collection.Status != status)
            return ApiResponses.Error("请求错误");
        if (ReverseStatus(status, releaseFlowHistoryStatus, out var nextStatus, out var res))
            return res;
        collection.Status = nextStatus;

        db.Collections.Update(collection);
        return db.SaveChanges() == 1
            ? ApiResponses.Success("修改成功")
            : ApiResponses.Error("修改失败");
    }
}