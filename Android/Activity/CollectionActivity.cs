using Android.Content;
using Android.Helper;
using Android.Holder;
using Android.HttpClients;
using Android.ViewModel;
using AnyLibrary.Constants;

namespace Android.Activity;

[Activity]
public class CollectionActivity : App.Activity
{
    private ActivityCollectionHolder holder;

    private CollectionModel _model;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        long? id = Intent?.GetLongExtra("id", 0);
        if (id == 0) return;
        holder = new ActivityCollectionHolder(this);
        _model = ApiService.GetCollectionAt(id.Value);
        if (_model is null)
        {
            Finish();
            return;
        }

        holder.Bind(_model);
        SetContentView(holder.Root);

        holder.Like.CallClick(() =>
        {
            if (_model.IsLike)
            {
                var req = ApiEndpoints.RemoveFavoriteItemsById(new
                {
                    Id = AppConfigHelper.AppConfig.Id,
                    Flag = IdCategory.Collection,
                    TId = _model.CollectionId
                });
                req.Execute(res =>
                {
                    _model.IsLike = false;
                    _model.FavoriteCount -= 1;
                    holder.BindCategory();
                });
            }
            else
            {
                new Intent(this, typeof(FavoriteActivity))
                    .PutExtra("tid", _model.CollectionId)
                    .PutExtra("idCategory", (int)IdCategory.Collection)
                    .PutExtra("name", _model.Title)
                    .PutExtra("fileUrl", _model.FileUrl)
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
            _model.IsLike = value != "0";
            _model.FavoriteCount += value == "0" ? 0 : 1;
            holder.BindCategory();
        }
    }
}