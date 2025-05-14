using Android.Activity;
using Android.Adapter;
using Android.Attribute;
using Android.Content;
using Android.Helper;
using Android.HttpClients;
using Android.Views;
using AnyLibrary.Constants;
using Google.Android.Material.Tabs;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;

namespace Android.Holder;

[ViewClassBind(Layout.activity_favorite_list)]
public class ActivityFavoriteListHolder(App.Activity activity) : ViewHolder<View>(activity)
{
    [ViewBind(Id.id_favorite_list_tab)] public TabLayout Tab;

    [ViewBind(Id.id_favorite_list_close)] public TextView ListClose;

    [ViewBind(Id.id_favorite_list_del)] public TextView Del;
    [ViewBind(Id.id_favorite_list_sel)] public CheckBox Sel;

    [ViewBind(Id.id_favorite_list_multiple_layout)]
    public LinearLayout MultipleLayout;

    [ViewBind(Id.id_favorite_list_layout)] public LinearLayout ListLayout;

    [ViewBind(Id.id_favorite_list_add)] public ImageView Add;

    [ViewBind(Id.id_favorite_list_open_list)]
    public ImageView OpenList;

    [ViewBind(Id.id_favorite_list_main)] public ListView List;

    public FavoriteListAdapter Adapter;

    protected override void Init()
    {
        List<MenuItem> items =
        [
            new(IdCategory.All, "全部", Drawable.ic_favorites),
            new(IdCategory.Ingredient, "食材", Drawable.ic_ingredient),
            new(IdCategory.Recipe, "食谱", Drawable.ic_recipe),
            new(IdCategory.Collection, "合集", Drawable.ic_collection)
        ];
        Adapter = new FavoriteListAdapter(activity, List, IdCategory.All)
        {
            LongMultiple = true,
            IsMultiple = false
        };
        List.Adapter = Adapter;

        Tab.TabSelected += (sender, args) =>
        {
            var item = items[Tab.SelectedTabPosition];
            Adapter.IdCategory = item.Id;
            Adapter.ReInitModels();
            Add.Visibility = item.Id is IdCategory.Recipe or IdCategory.Ingredient
                ? ViewStates.Visible
                : ViewStates.Gone;
        };
        Tab.RemoveAllTabs();

        items.ForEach(item =>
            Tab.AddTab(Tab.NewTab().SetCustomView(new ItemMenuButtonHolder(activity).Bind(item).Root)));

        Add.CallClick(() =>
        {
            new Intent(activity, typeof(FavoriteOperationActivity))
                .PutExtra("idCategory", Adapter.IdCategory)
                .PutExtra("mod", (int)FavoriteMod.Add)
                .StartActivityForResult(activity);
        });

        bool b = false;

        OpenList.CallClick(() =>
        {
            if (b)
            {
                Adapter.IsMultiple = false;
                MultipleLayout.Visibility = ViewStates.Gone;
                ListLayout.Visibility = ViewStates.Visible;

                Sel.Checked = false;
            }
            else
            {
                Adapter.IsMultiple = true;
                ListLayout.Visibility = ViewStates.Gone;
                MultipleLayout.Visibility = ViewStates.Visible;
            }

            b = !b;
        });

        ListClose.CallClick(() => { OpenList.CallOnClick(); });

        Del.CallClick(() =>
        {
            var models = Adapter.SelectedList;
            if (models.Count == 0) return;
            MsgBoxHelper.Builder($"确认删除这{models.Count}条收藏记录？").OkCancel(() =>
            {
                ApiEndpoints.RemoveFavorites(new
                    {
                        Id = AppConfigHelper.AppConfig.Id,
                        Flag = models[0].Flag,
                        FavoriteIds = models.Select(x => x.FavoriteId).ToList(),
                    })
                    .Execute(out var res);
                MsgBoxHelper.Builder(res.Message).ShowDialog();
                OpenList.CallOnClick();
                Adapter.ReInitModels();
            });
        });

        bool flag = false;
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

        Sel.CheckedChange += (sender, args) =>
        {
            if (flag) return;
            if (Sel.Checked) Adapter.SelectAll();
            else Adapter.RemoveAll();
        };

        Adapter.OpenMultiple = () => OpenList.CallOnClick();

        Adapter.OnItemClick = model =>
        {
            new Intent(activity, typeof(FavoriteInfoActivity))
                .PutExtra("favoriteId", model.FavoriteId)
                .StartActivityForResult(activity);
        };
    }
}

public class MenuItem(int id, string text, int icon)
{
    public int Id { get; set; } = id;
    public string Text { get; set; } = text;
    public int Icon { get; set; } = icon;
}