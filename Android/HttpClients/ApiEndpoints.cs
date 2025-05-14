using static RestSharp.Method;

namespace Android.HttpClients;

public static class ApiEndpoints
{
    private static ApiRequest Put(string url, object parameters, bool isJwt = true)
    {
        return new ApiRequest
        {
            Method = POST,
            Url = url,
            Parameters = parameters,
            IsJwt = isJwt
        };
    }

    // 用户相关请求构建
    public static ApiRequest Login(object parameters) =>
        Put("/Login", parameters, false);

    public static ApiRequest LoginImg(object parameters) =>
        Put("/LoginImg", parameters, false);

    public static ApiRequest Register(object parameters) =>
        Put("/Register", parameters, false);

    public static ApiRequest Logout(object parameters) =>
        Put("/Logout", parameters);

    public static ApiRequest ChangeInfo(object parameters) =>
        Put("/ChangeInfo", parameters);

    public static ApiRequest ChangePassword(object parameters) =>
        Put("/ChangePassword", parameters);

    public static ApiRequest GetInfo(object parameters) =>
        Put("/GetInfo", parameters);

    public static ApiRequest GetFavoriteItems(object parameters) =>
        Put("FavoriteItem/GetFavoriteItems", parameters);

    // 用户评论相关请求构建
    public static ApiRequest AddComment(object parameters) =>
        Put("Comment/AddComment", parameters);

    public static ApiRequest GetComments(object parameters) =>
        Put("Comment/GetComments", parameters);

    // 用户配置相关请求构建
    public static ApiRequest GetUserConfigs(object parameters) =>
        Put("Config/GetConfigs", parameters);

    // 用户饮食记录相关请求构建
    public static ApiRequest AddEatingDiary(object parameters) =>
        Put("EatingDiary/AddEatingDiary", parameters);

    public static ApiRequest DeleteUserEatingDiary(object parameters) =>
        Put("EatingDiary/DeleteEatingDiary", parameters);

    public static ApiRequest GetEatingDiaries(object parameters) =>
        Put("EatingDiary/GetEatingDiaries", parameters);

    // 用户收藏相关请求构建
    public static ApiRequest AddFavorite(object parameters) =>
        Put("Favorite/AddFavorite", parameters);

    public static ApiRequest EditFavorite(object parameters) =>
        Put("Favorite/EditFavorite", parameters);

    public static ApiRequest RemoveFavorites(object parameters) =>
        Put("Favorite/RemoveFavorites", parameters);

    public static ApiRequest Favorites(object parameters) =>
        Put("Favorite/UserFavorites", parameters);

    // 用户收藏项目相关请求构建
    public static ApiRequest AddFavoriteItems(object parameters) =>
        Put("FavoriteItem/AddFavoriteItems", parameters);

    public static ApiRequest RemoveFavoriteItemsById(object parameters) =>
        Put("FavoriteItem/RemoveFavoriteItemsById", parameters);

    public static ApiRequest RemoveFavoriteItems(object parameters) =>
        Put("FavoriteItem/RemoveFavoriteItems", parameters);

    // 用户信息相关请求构建
    public static ApiRequest MyAllInfo(object parameters) =>
        Put("Info/MyAllInfo", parameters);

    public static ApiRequest AddMyInfo(object parameters) =>
        Put("Info/AddMyInfo", parameters);

    // 用户报告相关请求构建
    public static ApiRequest AddReport(object parameters) =>
        Put("Report/AddReport", parameters);

    public static ApiRequest IndexRecipeList(object parameters) =>
        Put("IndexSearch/IndexRecipeList", parameters);

    public static ApiRequest IndexCollectionList(object parameters) =>
        Put("IndexSearch/IndexCollectionList", parameters);

    public static ApiRequest IndexSearchCollectionList(object parameters) =>
        Put("IndexSearch/IndexSearchCollectionList", parameters);

    public static ApiRequest IndexSearchRecipeList(object parameters) =>
        Put("IndexSearch/IndexSearchRecipeList", parameters);
    
    public static ApiRequest IndexSearchBaseInfo(object parameters) =>
        Put("IndexSearch/IndexSearchBaseInfo", parameters);

    public static ApiRequest IndexIngredientInfoList(object parameters) =>
        Put("IndexSearch/IndexIngredientList", parameters);

    public static ApiRequest IndexSearchIngredientInfoList(object parameters) =>
        Put("IndexSearch/IndexSearchIngredientList", parameters);

    public static ApiRequest GetIngredient(object parameters) =>
        Put("Ingredient/GetIngredient", parameters);

    public static ApiRequest GetCollection(object parameters) =>
        Put("Collection/GetCollection", parameters);

    public static ApiRequest GetIngredientInfo(object parameters) =>
        Put("Ingredient/GetIngredientInfo", parameters);

    public static ApiRequest AddCategoryItem(object parameters) =>
        Put("CategoryItem/AddCategoryItem", parameters);

    public static ApiRequest DeleteCategoryItem(object parameters) =>
        Put("CategoryItem/DeleteCategoryItem", parameters);

    public static ApiRequest CreatCategoryItem(object parameters) =>
        Put("CategoryItem/CreatCategoryItem", parameters);

    public static ApiRequest GetRecipe(object parameters) =>
        Put("Recipe/GetRecipe", parameters);

    public static ApiRequest ReleaseRecipe(object parameters) =>
        Put("Release/ReleaseRecipe", parameters);

    public static ApiRequest ReleaseCollection(object parameters) =>
        Put("Release/ReleaseCollection", parameters);

    public static ApiRequest GetReleaseRecipes(object parameters) =>
        Put("Release/GetReleaseRecipes", parameters);

    public static ApiRequest GetReleaseCollections(object parameters) =>
        Put("Release/GetReleaseCollections", parameters);

    public static ApiRequest GetReleaseRecipe(object parameters) =>
        Put("Release/GetReleaseRecipe", parameters);
    
    public static ApiRequest GetReleaseCollection(object parameters) =>
        Put("Release/GetReleaseCollection", parameters);

    public static ApiRequest DeleteRelease(object parameters) =>
        Put("Release/DeleteRelease", parameters);

    public static ApiRequest GetReleaseStatus(object parameters) =>
        Put("Release/GetReleaseStatus", parameters);

    public static ApiRequest ReverseRecipeStatus(object parameters) =>
        Put("Release/ReverseRecipeStatus", parameters);

    public static ApiRequest ReverseCollectionStatus(object parameters) =>
        Put("Release/ReverseCollectionStatus", parameters);
}