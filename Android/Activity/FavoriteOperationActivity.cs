using Android.Content;
using Android.Helper;
using Android.Holder;
using Android.Models;
using AnyLibrary.Constants;
using AnyLibrary.Helper;
using Result = Android.App.Result;

namespace Android.Activity;

[Activity]
public class FavoriteOperationActivity : App.Activity
{
    private FavoriteOperationActivityHolder holder;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        try
        {
            var b = Intent?.Extras;
            int idCategory = b.GetInt("idCategory");
            var mod = (FavoriteMod)b.GetInt("mod");

            if (idCategory is not (IdCategory.Recipe or IdCategory.Ingredient))
                throw new NotImplementedException();

            holder = new FavoriteOperationActivityHolder(this);
            holder.Bind(idCategory);
            SetContentView(holder.Root);
            if (mod == FavoriteMod.Edit)
            {
                string? favoriteDate = b.GetString("favoriteDate");
                var model = favoriteDate.ToEntity<FavoriteModel>();
                holder.Sub.Text = "修改";
                holder.Bind(model);
            }


            holder.Sub.CallClick(() =>
            {
                int favorite = mod switch
                {
                    FavoriteMod.Add => holder.model.AddFavorite(),
                    FavoriteMod.Edit => holder.model.EditFavorite(),
                    _ => -1
                };

                if (favorite > 0)
                {
                    SetResult(Result.Ok, null);
                    Finish();
                }
            });

            return;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        Finish();
    }

    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        base.OnActivityResult(requestCode, resultCode, data);
        if (requestCode != 1 || resultCode != Result.Ok || data == null) return;
        holder.SetFileUrl(data.Data);
    }
}

public enum FavoriteMod : int
{
    Add,
    Edit
}