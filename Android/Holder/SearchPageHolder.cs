using Android.Activity;
using Android.Adapter;
using Android.Attribute;
using Android.Content;
using Android.Helper;
using Android.HttpClients;
using Android.Views;
using AndroidX.ViewPager.Widget;
using AnyLibrary.Constants;
using Google.Android.Material.Tabs;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;
using Object = Java.Lang.Object;
using TextView = Android.Widget.TextView;

namespace Android.Holder;

[ViewClassBind(Layout.activity_search)]
public class SearchPageHolder(App.Activity activity, SearchFlag flag) : ViewHolder<View>(activity)
{
    [ViewHolderBind(Id.id_search_page_toolbar)]
    public SearchToolbar2Holder SearchToolbar2Holder;

    [ViewBind(Id.id_search_list)] public readonly ListView List;

    [ViewBind(Id.id_search_layout)] public readonly LinearLayout SearchLayout;

    [ViewBind(Id.id_search_tab)] public TabLayout Tab;
    [ViewBind(Id.id_search_content)] public ViewPager Page;

    public SearchPageAdapter PageAdapter;
    private SearchHistoryAdapter adapter;

    protected override void Init()
    {
        adapter = new SearchHistoryAdapter(activity);
        List.Adapter = adapter;
        if ((flag & SearchFlag.All) > 0 || (flag & SearchFlag.Colletion) > 0 || (flag & SearchFlag.Recipe) > 0)
        {
            adapter.Models = AppConfigHelper.SearchHistory;
        }
        else if ((flag & SearchFlag.Colletion) > 0)
        {
            adapter.Models = AppConfigHelper.FavoriteSearchHistory;
        }

        List.ItemClick += (sender, args) =>
        {
            if (args?.View?.Tag is not SearchPageItemHolder holder) return;
            SearchToolbar2Holder.SearchEditText.Text = holder.Name.Text;
            SearchToolbar2Holder.SearchButton.CallOnClick();
        };

        SearchToolbar2Holder.SearchButton.CallClick(() =>
        {
            var editText = SearchToolbar2Holder.SearchEditText;
            var query = editText.Text;
            Update(true);
            Search = query;
        });

        SearchToolbar2Holder.CloseButton.CallClick(() => { Update(false); });

        SearchToolbar2Holder.ImageViewerBack.CallClick(activity.Finish);

        List<SearchMenuItem> items = null;
        if ((flag & SearchFlag.All) > 0)
        {
            items = new List<SearchMenuItem>
            {
                new(MenuFlags.Ingredients, "食材"),
                new(MenuFlags.Recipe, "食谱"),
                new(MenuFlags.Collection, "合集")
            };
        }
        else if ((flag & SearchFlag.Recipe) > 0)
        {
            items = new List<SearchMenuItem>
            {
                new(MenuFlags.Recipe, "食谱")
            };
        }
        else if ((flag & SearchFlag.Ingredient) > 0)
        {
            items = new List<SearchMenuItem>
            {
                new(MenuFlags.Ingredients, "食材")
            };
        }

        PageAdapter = new SearchPageAdapter(activity, items, this);
        PageAdapter.OnItemClick = (idCategory, id) =>
        {
            if ((flag & SearchFlag.Open) > 0)
            {
                if ((idCategory & IdCategory.Ingredient) > 0)
                {
                    ActivityHelper.GotoIngredient(id);
                }
                else if ((idCategory & IdCategory.Recipe) > 0)
                {
                    ActivityHelper.GotoRecipe(id);
                }
            }
            else
            {
                activity.SetResult(Result.Ok, new Intent()
                    .PutExtra("idCategory", idCategory)
                    .PutExtra("id", id)
                );
                activity.Finish();
            }

            return true;
        };
        Page.Adapter = PageAdapter;
        Tab.SetupWithViewPager(Page);
        Tab.RemoveAllTabs();
        items.ForEach(item => Tab.AddTab(Tab.NewTab().SetId((int)item.Id).SetText(item.Text)));
    }


    public void Update(bool b)
    {
        if (b)
        {
            SearchLayout.Visibility = ViewStates.Visible;
            List.Visibility = ViewStates.Gone;
        }
        else
        {
            List.Visibility = ViewStates.Visible;
            SearchLayout.Visibility = ViewStates.Gone;
        }
    }

    private string? _search;

    public string? Search
    {
        get => _search;
        set
        {
            if (string.IsNullOrEmpty(value)) return;
            if (adapter.Models.Contains(value))
            {
                var ind = adapter.Models.IndexOf(value);
                adapter.Models.RemoveAt(ind);
                adapter.Models.Insert(0, value);
            }
            else
            {
                adapter.Models.Add(value);
            }

            _search = value;

            if ((flag & SearchFlag.All) > 0 || (flag & SearchFlag.Colletion) > 0 || (flag & SearchFlag.Recipe) > 0)
            {
                AppConfigHelper.SaveSearchHistory();
            }
            else if ((flag & SearchFlag.Colletion) > 0)
            {
                AppConfigHelper.SaveFavoriteSearchHistory();
            }

            adapter.NotifyDataSetChanged();
        }
    }
}

[ViewClassBind(Layout.search_page_item)]
public class SearchPageItemHolder(App.Activity activity) : ViewHolder<View>(activity)
{
    [ViewBind(Id.tv_search_item_name)] public TextView Name;

    protected override void Init()
    {
    }
}

public class SearchHistoryAdapter(App.Activity activity)
    : SimpleAdapter<string>(activity)
{
    public override bool CreateItem(App.Activity activity, int pos, out Object holder, out View root)
    {
        var searchPageItemHolder = new SearchPageItemHolder(activity);
        holder = searchPageItemHolder;
        root = searchPageItemHolder.Root;
        root.Tag = holder;
        return true;
    }

    public override void Bind(string item, Object holder)
    {
        var searchPageItemHolder = (SearchPageItemHolder)holder;
        searchPageItemHolder.Name.Text = item;
    }
}

public record SearchMenuItem(MenuFlags Id, string Text)
{
    public readonly MenuFlags Id = Id;
    public readonly string Text = Text;
}

public enum MenuFlags
{
    Ingredients,
    Recipe,
    Collection
}

public class SearchPageAdapter(Android.App.Activity activity, List<SearchMenuItem> menuItems, SearchPageHolder sholder)
    : SimplePagerAdapter<SearchMenuItem>(menuItems)
{
    public Func<int, long, bool> OnItemClick;

    public override Object InstantiateItem(ViewGroup container, int position)
    {
        var item = menuItems[position];
        if (string.IsNullOrEmpty(sholder.Search)) return null;
        View v = null;
        switch (item.Id)
        {
            case MenuFlags.Ingredients:
                var iHolder = new PageHomeIngredientHolder(activity);
                iHolder.Bind((flag, count) => ApiService.SearchIngredientInfoList(sholder.Search, flag, count + 1),
                    OnItemClick);
                v = iHolder.Root;
                break;
            case MenuFlags.Recipe:
                var rholder = new PageHomeRecipeHolder(activity);
                rholder.Bind(count => ApiService.SearchRecipeList(sholder.Search, count + 1), OnItemClick);
                v = rholder.Root;
                break;
            case MenuFlags.Collection:
                var cholder = new PageHomeCollectionHolder(activity);
                cholder.Bind(count => ApiService.SearchCollectionList(sholder.Search, count + 1), OnItemClick);
                v = cholder.Root;
                break;
        }

        if (v is not null)
            container.AddView(v); //添加到父控件
        return v;
    }
}