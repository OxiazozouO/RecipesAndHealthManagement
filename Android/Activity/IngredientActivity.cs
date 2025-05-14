using Android.Content;
using Android.Helper;
using Android.Holder;
using Android.HttpClients;
using Android.ViewModel;
using AnyLibrary.Constants;

namespace Android.Activity;

[Activity]
public class IngredientActivity : App.Activity
{
    private ActivityIngredientHolder holder;

    private IngredientViewModel _model;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        long? id = Intent?.GetLongExtra("id",0);
        if (id == 0) return;
        holder = new ActivityIngredientHolder(this);
        _model = ApiService.GetIngredientAt(id.Value);
        if (_model is null)
        {
            Finish();
            return;
        }

        holder.Bind(_model);
        SetContentView(holder.Root);

        holder.Like.CallClick(() =>
        {
            if (_model.Ingredient.IsLike)
            {
                var req = ApiEndpoints.RemoveFavoriteItemsById(new
                {
                    Id = AppConfigHelper.AppConfig.Id,
                    Flag = IdCategory.Ingredient,
                    TId = _model.Ingredient.IngredientId
                });
                req.Execute(res =>
                {
                    _model.Ingredient.IsLike = false;
                    _model.Ingredient.FavoriteCount -= 1;
                    holder.Update2(_model);
                });
            }
            else
            {
                new Intent(this, typeof(FavoriteActivity))
                    .PutExtra("tid", _model.Ingredient.IngredientId)
                    .PutExtra("idCategory", (int)IdCategory.Ingredient)
                    .PutExtra("name", _model.Ingredient.IName)
                    .PutExtra("fileUrl", _model.Ingredient.FileUrl)
                    .StartActivityForResult(this);
            }
        });
    }

    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        base.OnActivityResult(requestCode, resultCode, data);
        if (requestCode == 1 && resultCode == Result.Ok)
        {
            string value = data.Extras.GetString("count", "0");
            _model.Ingredient.IsLike = value != "0";
            _model.Ingredient.FavoriteCount += value == "0" ? 0 : 1;
            holder.Update2(_model);
        }
    }
}