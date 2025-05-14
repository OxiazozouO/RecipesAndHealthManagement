using Android.Activity;
using Android.Adapter;
using Android.Attribute;
using Android.Content;
using Android.Helper;
using Android.HttpClients;
using Android.ViewModel;
using Android.Views;
using AnyLibrary.Constants;
using AnyLibrary.Helper;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;

namespace Android.Holder;

[ViewClassBind(Layout.activity_favorite_info)]
public class ActivityFavoriteInfoHolder(App.Activity activity) : ViewHolder<View>(activity)
{
    [ViewBind(Id.id_favorite_info_title)] public TextView Title;

    [ViewBind(Id.id_favorite_info_refer)] public TextView Refer;

    [ViewBind(Id.id_favorite_info_mod_time)]
    public TextView ModTime;

    [ViewBind(Id.id_favorite_info_flag)] public TextView Flag;

    [ViewBind(Id.id_favorite_info_count)] public TextView Count;

    [ViewBind(Id.id_favorite_info_list)] public ListView List;

    [ViewBind(Id.id_favorite_info_main)] public LinearLayout Main1;

    [ViewBind(Id.id_favorite_info_main2)] public LinearLayout Main2;

    [ViewBind(Id.id_favorite_info_sel)] public CheckBox Sel;

    [ViewBind(Id.id_favorite_info_del)] public TextView Del;

    [ViewBind(Id.id_favorite_info_edit_layout)]
    public LinearLayout EditLayout;

    [ViewBind(Id.id_favorite_info_open_edit)]
    public ImageView OpenEdit;

    [ViewBind(Id.id_favorite_info_open_sel)]
    public ImageView OpenSel;

    [ViewBind(Id.id_favorite_info_close)] public TextView InfoClose;


    public FavoriteAdapter Adapter;

    public long FavoriteId;

    protected override void Init()
    {
        Adapter = new FavoriteAdapter(activity, List)
        {
            LongMultiple = true,
            IsMultiple = false
        };

        OpenEdit.CallClick(() =>
        {
            new Intent(activity, typeof(FavoriteOperationActivity))
                .PutExtra("idCategory", (int)model.Favorite.Flag)
                .PutExtra("mod", (int)FavoriteMod.Edit)
                .PutExtra("favoriteDate", model.Favorite.ToJson())
                .StartActivityForResult(activity);
        });

        bool b = true;
        OpenSel.CallClick(() =>
        {
            if (b)
            {
                EditLayout.Visibility = ViewStates.Gone;
                Main1.Visibility = ViewStates.Gone;
                Main2.Visibility = ViewStates.Visible;

                Sel.Checked = false;
            }
            else
            {
                EditLayout.Visibility = ViewStates.Visible;
                Main2.Visibility = ViewStates.Gone;
                Main1.Visibility = ViewStates.Visible;
            }

            Adapter.IsMultiple = b;
            b = !b;
        });
        Adapter.OpenMultiple = () => OpenSel.CallOnClick();

        Adapter.OnItemClick = viewModel =>
        {
            var flag = model.Favorite.Flag;
            switch (flag)
            {
                case IdCategory.Ingredient:
                    ActivityHelper.GotoIngredient(viewModel.FavoriteItem.Id);
                    break;
                case IdCategory.Recipe:
                    ActivityHelper.GotoRecipe(viewModel.FavoriteItem.Id);
                    break;
            }
        };

        bool flag = false;

        Sel.CheckedChange += (sender, args) =>
        {
            if (flag) return;
            if (Sel.Checked) Adapter.SelectAll();
            else Adapter.RemoveAll();
        };

        Adapter.SelectAction = pos =>
        {
            if (!Adapter.IsMultiple) return;
            switch (pos)
            {
                case -1:
                    flag = true;
                    Sel.Checked = false;
                    flag = false;
                    break;
                case 1:
                    flag = true;
                    Sel.Checked = true;
                    flag = false;
                    break;
            }
        };

        Del.CallClick(() =>
        {
            MsgBoxHelper.Builder($"确认删除这{Adapter.SelectedList.Count}条收藏记录？").OkCancel(() =>
            {
                var req = ApiEndpoints.RemoveFavoriteItems(new
                {
                    AppConfigHelper.AppConfig.Id,
                    model.Favorite.Flag,
                    model.Favorite.FavoriteId,
                    TIds = Adapter.SelectedList.Select(x => x.FavoriteItem.Id).ToList()
                });
                req.Execute(res =>
                {
                    OpenSel.CallOnClick();
                    Update();
                });
            });
        });

        InfoClose.CallClick(() => OpenSel.CallOnClick());
    }

    private FavoriteAtModel model;

    public void Bind(long favoriteId)
    {
        FavoriteId = favoriteId;
        Update();
    }

    public void Update()
    {
        var model = ApiService.GetFavoriteItems(FavoriteId);
        this.model = model;
        var favorite = model.Favorite;
        Title.Text = favorite.FName;
        Refer.Text = favorite.Refer;
        ModTime.Text = favorite.ModifyDate.TimeStr1();

        var (name, sid) = Dir[favorite.Flag];
        Flag.Text = name;
        Flag.SetBackgroundResource(sid);

        Count.Text = favorite.Count.ToString();
        Adapter.ReSet(model.FavoriteItems);
    }
    
    public static Dictionary<int, (string name, int sid)> Dir = new()
    {
        [0] = ("", Drawable.shape_label9_bg),
        [IdCategory.Ingredient] = ("食材", Drawable.shape_label5_bg),
        [IdCategory.Recipe] = ("食谱", Drawable.shape_label10_bg),
        [IdCategory.Collection] = ("合集", Drawable.shape_label8_bg)
    };
}