using Android.Content;
using Android.Helper;
using Android.Holder;
using Android.HttpClients;
using Android.Models;
using Android.ViewModel;
using Android.Views;
using AnyLibrary.Constants;

namespace Android.Activity;

[Activity]
public class RecipeActivity : BaseActivity
{
    public ActivityRecipeHolder holder;

    private RecipeModel model;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        try
        {
            long? id = Intent?.GetLongExtra("id", 0);
            int? flag = Intent?.Extras?.GetInt("flag") ?? 0;
            if (id == 0) return;
            holder = new ActivityRecipeHolder(this);
            model = ApiService.GetRecipe(id.Value);
            if (model is null) return;
            holder.Bind(new RecipeViewModel { Recipe = model });
            if (flag == 1)
            {
                holder.CommentHolder.Root.Visibility = ViewStates.Gone;
                holder.Islike.Visibility = ViewStates.Gone;
                holder.LikeNum.Visibility = ViewStates.Gone;
                holder.Share.Visibility = ViewStates.Gone;
                holder.Report.Visibility = ViewStates.Gone;
            }
            SetContentView(holder.Root);

            holder.Islike.CallClick(() =>
            {
                if (model.IsLike)
                {
                    var req = ApiEndpoints.RemoveFavoriteItemsById(new
                    {
                        Id = AppConfigHelper.AppConfig.Id,
                        Flag = IdCategory.Recipe,
                        TId = model.RecipeId
                    });
                    if (req.Execute(out var res))
                    {
                        model.IsLike = false;
                        model.FavoriteCount -= 1;
                        holder.Update2(model);
                    }
                }
                else
                {
                    new Intent(this, typeof(FavoriteActivity))
                        .PutExtra("tid", model.RecipeId)
                        .PutExtra("idCategory", (int)IdCategory.Recipe)
                        .PutExtra("name", model.RName)
                        .PutExtra("fileUrl", model.FileUrl)
                        .StartActivityForResult(this);
                }
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        
    }

    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        base.OnActivityResult(requestCode, resultCode, data);
        if (requestCode == 1 && resultCode == Result.Ok)
        {
            int value = data.Extras.GetInt("count", 0);
            model.IsLike = value > 0;
            model.FavoriteCount += value;
            holder.Update2(model);
        }
    }

    public override void OnBackPressed()
    {
        if (holder.ImageViewer.Root.Visibility == ViewStates.Visible)
        {
            holder.ImageViewer.Back.CallOnClick();
        }
        else
        {
            base.OnBackPressed();
        }
    }
}