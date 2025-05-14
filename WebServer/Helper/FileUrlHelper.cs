using System.Text;
using AnyLibrary.Constants;
using Microsoft.AspNetCore.Mvc;
using WebServer.Configurations;

namespace WebServer.Helper;

public static class FileUrlHelper
{
    public static string OldFilePath => AppSettings.FileUrlConfig.OldFilePath;
    public static string Temps => AppSettings.FileUrlConfig.Temps;
    public static string Recipes => AppSettings.FileUrlConfig.Recipes;
    public static string Ingredients => AppSettings.FileUrlConfig.Ingredients;
    public static string Collections => AppSettings.FileUrlConfig.Collections;
    public static string Users => AppSettings.FileUrlConfig.Users;
    public static string Admins => AppSettings.FileUrlConfig.Admins;
    public static string Favorites => AppSettings.FileUrlConfig.Favorites;
    public static string UserDomain => AppSettings.FileUrlConfig.UserDomain;
    public static string AdminDomain => AppSettings.FileUrlConfig.AdminDomain;
    
    
    public static string? GetTmpUrl(this IUrlHelper urlHelper, HttpRequest request, string? fileName) =>
        GetUrl(urlHelper, request, Temps, fileName, false);

    public static string? GetRecipeUrl(this IUrlHelper urlHelper, HttpRequest request, string? fileName) =>
        GetUrl(urlHelper, request, Recipes, fileName, true);

    public static string? AdminGetRecipeUrl(this IUrlHelper urlHelper, HttpRequest request, string? fileName) =>
        GetUrl(urlHelper, request, Recipes, fileName, false);

    public static string? GetAdminUrl(this IUrlHelper urlHelper, HttpRequest request, string? fileName) =>
        GetUrl(urlHelper, request, Admins, fileName, true);

    public static string? AdminGetAdminUrl(this IUrlHelper urlHelper, HttpRequest request, string? fileName) =>
        GetUrl(urlHelper, request, Admins, fileName, false);

    public static string? GetFavoriteUrl(this IUrlHelper urlHelper, HttpRequest request, string? fileName) =>
        GetUrl(urlHelper, request, Favorites, fileName, true);

    public static string? GetIngredientUrl(this IUrlHelper urlHelper, HttpRequest request, string? fileName) =>
        GetUrl(urlHelper, request, Ingredients, fileName, true);

    public static string? AdminGetIngredientUrl(this IUrlHelper urlHelper, HttpRequest request, string? fileName) =>
        GetUrl(urlHelper, request, Ingredients, fileName, false);

    public static string? GetCollectionUrl(this IUrlHelper urlHelper, HttpRequest request, string? fileName) =>
        GetUrl(urlHelper, request, Collections, fileName, true);

    public static string? AdminGetCollectionUrl(this IUrlHelper urlHelper, HttpRequest request, string? fileName) =>
        GetUrl(urlHelper, request, Collections, fileName, false);

    public static string? GetUserUrl(this IUrlHelper urlHelper, HttpRequest request, string? fileName) =>
        GetUrl(urlHelper, request, Users, fileName, true);

    public static string? AdminGetUserUrl(this IUrlHelper urlHelper, HttpRequest request, string? fileName) =>
        GetUrl(urlHelper, request, Users, fileName, false);

    public static string? GetUrl(this IUrlHelper urlHelper, HttpRequest request, string path, string? fileName,
        bool isRep)
    {
        if (string.IsNullOrEmpty(fileName)) return null;
        var result = urlHelper.Action("GetImageFile", "File", new { path, fileName }, request.Scheme,
            request.Host.Value);
        return result?.Replace("localhost", isRep ? UserDomain : AdminDomain);
    }

    public static bool TryReplaceFile(this IUrlHelper urlHelper, string toPath, string? inputName,
        Action<string>? action = null)
    {
        if (inputName is null) return false;
        inputName = urlHelper.GetFileName(inputName);
        if (inputName is null) return false;
        bool b = TryReplaceFile(inputName, toPath);
        if (b) action?.Invoke(inputName);
        return b;
    }

    public static string? GetFileName(this IUrlHelper urlHelper, string? fileName) =>
        string.IsNullOrEmpty(fileName) ? null : fileName.Split('/').Last();

    public static bool TryReplaceFile(string? inputName, string toPath) =>
        TryReplaceFile(Temps, inputName, toPath);

    public static bool TryReplaceFile(string inputPath, string? inputName, string toPath)
    {
        if (inputName is null) return false;
        var sb = new StringBuilder();
        try
        {
            if (!ExistsForm(inputPath, inputName, out var inputFilePath))
            {
                sb.Append("文件不存在缓存区 ");
                if (ExistsForm(toPath, inputName, out _))
                    return true;
                sb.Append("文件完全不存在 ");
                return false;
            }

            var toFilePath = Path.Combine(OldFilePath, toPath, inputName);
            try
            {
                File.Move(inputFilePath, toFilePath, true);
                if (ExistsForm(toPath, inputName, out _))
                {
                    sb.Append("文件移动成功 ");
                    return true;
                }
            }
            catch (Exception c)
            {
                sb.Append("文件移动失败");
                return false;
            }
        }
        catch (Exception d)
        {
            sb.Append("文件不存在");
            return false;
        }

        return false;
    }

    public static bool ExistsForm(string path, string? fileName, out string retPath)
    {
        retPath = "";
        if (string.IsNullOrEmpty(fileName)) return false;
        retPath = Path.Combine(OldFilePath, path, fileName);
        return File.Exists(retPath);
    }

    public static string? AdminGetUrl(this IUrlHelper url, HttpRequest request, int idCategory, string fileUrl)
    {
        return idCategory switch
        {
            IdCategory.Recipe => url.AdminGetRecipeUrl(request, fileUrl),
            IdCategory.Collection => url.AdminGetCollectionUrl(request, fileUrl),
            IdCategory.Ingredient => url.AdminGetIngredientUrl(request, fileUrl),
            _ => ""
        };
    }

    public static string? AdminGetUsersUrl(this IUrlHelper url, HttpRequest request, bool isUser, string? fileUrl) =>
        isUser ? url.AdminGetUserUrl(request, fileUrl) : url.AdminGetAdminUrl(request, fileUrl);

    public static string? GetIdCategoryUrl(this IUrlHelper url, HttpRequest request, int idCategory, string fileUrl)
    {
        return idCategory switch
        {
            IdCategory.Recipe => url.GetRecipeUrl(request, fileUrl),
            IdCategory.Collection => url.GetCollectionUrl(request, fileUrl),
            IdCategory.Ingredient => url.GetIngredientUrl(request, fileUrl),
            _ => ""
        };
    }

    public static string? GetUsersUrl(this IUrlHelper url, HttpRequest request, bool isUser, string? fileUrl) =>
        isUser ? url.GetUserUrl(request, fileUrl) : url.GetAdminUrl(request, fileUrl);
}