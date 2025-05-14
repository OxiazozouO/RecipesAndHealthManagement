using Android.Activity;
using Android.Content;

namespace Android.Helper;

public static class ActivityHelper
{
    public static void StartActivity(this Intent intent, Context activity)
    {
        activity.StartActivity(intent);
    }

    public static void StartActivityForResult(this Intent intent, App.Activity activity, int code = 1)
    {
        activity.StartActivityForResult(intent, code);
    }

    public static void GotoLogin()
    {
        new Intent(Application.Context, typeof(LoginActivity))
            .AddFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask)
            .StartActivity(Application.Context);
    }

    public static void GotoRegister() => SimpleGoTo<RegisterActivity>();

    public static void GotoSearch(SearchFlag flag = SearchFlag.All | SearchFlag.Open) =>
        new Intent(CurrentActivity, typeof(SearchActivity))
            .PutExtra("flag", (int)flag)
            .StartActivityForResult(CurrentActivity, 0);

    public static void GotoChangePassword() => SimpleGoTo<ChangePasswordActivity>();
    public static void GotoLogout() => SimpleGoTo<LogoutActivity>();
    public static void GotoFavoriteList() => SimpleGoTo<FavoriteListActivity>();

    public static void GotoAddRecipe(long releaseId = -1, long tid = 0) =>
        new Intent(Application.Context, typeof(AddRecipeActivity))
            .AddFlags(ActivityFlags.NewTask)
            .PutExtra("releaseId", releaseId)
            .PutExtra("tid", tid)
            .StartActivity(Application.Context);

    public static void GotoAddCollection(long releaseId = -1, long tid = 0) =>
        new Intent(Application.Context, typeof(AddCollectionActivity))
            .AddFlags(ActivityFlags.NewTask)
            .PutExtra("releaseId", releaseId)
            .PutExtra("tid", tid)
            .StartActivity(Application.Context);

    public static void GotoIngredient(long id) => new Intent(Application.Context, typeof(IngredientActivity))
        .AddFlags(ActivityFlags.NewTask)
        .PutExtra("id", id)
        .StartActivity(Application.Context);

    public static void GotoDiaryNutrient(string json) => new Intent(Application.Context, typeof(DiaryNutrientActivity))
        .AddFlags(ActivityFlags.NewTask)
        .PutExtra("json", json)
        .StartActivity(Application.Context);

    public static void GotoRecipe(long id, int flag = 0) => new Intent(Application.Context, typeof(RecipeActivity))
        .AddFlags(ActivityFlags.NewTask)
        .PutExtra("id", id)
        .PutExtra("flag", flag)
        .StartActivity(Application.Context);

    public static void GotoCollection(long id, int flag = 0) => new Intent(Application.Context, typeof(CollectionActivity))
        .AddFlags(ActivityFlags.NewTask)
        .PutExtra("id", id)
        .PutExtra("flag", flag)
        .StartActivity(Application.Context);

    public static void GotoReport(long tid, sbyte category, string name) =>
        new Intent(Application.Context, typeof(ReportActivity))
            .AddFlags(ActivityFlags.NewTask)
            .PutExtra("tid", tid)
            .PutExtra("category", (int)category)
            .PutExtra("name", name)
            .StartActivity(Application.Context);

    public static void GoHome() => SimpleGoTo<MainActivity>(ActivityFlags.NewTask | ActivityFlags.ClearTask);

    public static void SimpleGoTo<T>(ActivityFlags flags = ActivityFlags.NewTask) where T : Android.App.Activity
    {
        new Intent(Application.Context, typeof(T))
            .AddFlags(flags)
            .StartActivity(Application.Context);
    }

    public static void GotoUserInfo() => SimpleGoTo<EditUserInfoActivity>();


    private static WeakReference<App.Activity>? _currentActivityWeakRef;

    public static App.Activity? CurrentActivity
    {
        set => _currentActivityWeakRef = new WeakReference<App.Activity>(value);
        get
        {
            App.Activity activity = null;
            _currentActivityWeakRef?.TryGetTarget(out activity);
            return activity;
        }
    }

    public static void SelectImage(this App.Activity activity, int code = 1)
    {
        activity.StartActivityForResult(new Intent(Intent.ActionGetContent)
            .SetType("image/*"), code);
    }
}