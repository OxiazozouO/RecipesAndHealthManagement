using Android.Activity;
using Android.Adapter;
using Android.Attribute;
using Android.Content;
using Android.Helper;
using Android.HttpClients;
using Android.Views;
using AnyLibrary.Constants;
using Bumptech.Glide;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;

namespace Android.Holder;

[ViewClassBind(Layout.activity_favorite)]
public class ActivityFavoriteHolder(App.Activity activity) : ViewHolder<View>(activity)
{
    [ViewBind(Id.id_favorite_item_category)]
    public TextView Category;

    [ViewBind(Id.id_favorite_item_name)] public TextView Name;

    [ViewBind(Id.id_favorite_add_sub)] public TextView AddSub;

    [ViewBind(Id.id_favorite_list)] public ListView List;

    [ViewBind(Id.id_favorite_sub)] public TextView Sub;

    [ViewBind(Id.id_favorite_item_img)] public ImageView Img;

    private sbyte idCategory;
    private long tid;

    public FavoriteListAdapter Adapter;

    protected override void Init()
    {
        Sub.CallClick(() =>
        {
            var list = Adapter.SelectedList;
            if (list.Count == 0)
            {
                Toast.MakeText(activity, "请选择收藏夹", ToastLength.Short).Show();
                return;
            }

            var ints = list.Select(l => l.FavoriteId).ToList();
            var item = ApiEndpoints.AddFavoriteItems(new
            {
                Id = AppConfigHelper.AppConfig.Id,
                Flag = idCategory,
                FavoriteIds = ints,
                TId = tid
            });
            if (item.Execute(out var res))
            {
                var i = Convert.ToInt16(res.Data);
                activity.SetResult(Result.Ok, new Intent()
                    .PutExtra("count", i.ToString()));
                activity.Finish();
                MsgBoxHelper.Builder(res.Message).ShowDialog();
                return;
            }

            Toast.MakeText(activity, res.Message, ToastLength.Short).Show();
        });

        AddSub.CallClick(() =>
        {
            new Intent(activity, typeof(FavoriteOperationActivity))
                .PutExtra("idCategory", (int)idCategory)
                .PutExtra("mod", (int)FavoriteMod.Add)
                .StartActivityForResult(activity);
        });
    }

    public void Bind(int idCategory, string name, long tid, string file)
    {
        this.idCategory = (sbyte)idCategory;
        this.tid = tid;
        Adapter = new FavoriteListAdapter(activity, List, idCategory);
        Adapter.IsMultiple = true;

        Category.Text = IdCategory.GetName(this.idCategory);
        Name.Text = name;
        Glide.With(Root).Load(file).Into(Img);
    }
}