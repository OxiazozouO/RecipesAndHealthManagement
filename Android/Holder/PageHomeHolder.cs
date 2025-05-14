using Android.Adapter;
using Android.Attribute;
using Android.Helper;
using Android.Views;
using AndroidX.ViewPager.Widget;
using Google.Android.Material.Tabs;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;

namespace Android.Holder;

[ViewClassBind(Layout.page_home)]
public class PageHomeHolder(Android.App.Activity activity) : ViewHolder<View>(activity)
{
    [ViewHolderBind(Id.id_page_home_search_toolbar)]
    public SearchToolbarHolder SearchToolbarHolder;

    [ViewBind(Id.id_home_tab)] public TabLayout Tab;

    [ViewBind(Id.id_home_content)] public ViewPager ViewPager;

    protected override void Init()
    {
        var items = new List<HomeMenuItem>
        {
            new(HomeMenuFlags.Ingredients, "食材"),
            new(HomeMenuFlags.Recipe, "食谱"),
            new(HomeMenuFlags.Collection, "合集")
        };
        ViewPager.Adapter = new PageHomeAdapter(activity, items);
        Tab.SetupWithViewPager(ViewPager);
        Tab.RemoveAllTabs();
        items.ForEach(item => Tab.AddTab(Tab.NewTab().SetId((int)item.Id).SetText(item.Text)));

        SearchToolbarHolder.Root.CallClick(() => { ActivityHelper.GotoSearch(); });
        SearchToolbarHolder.SearchEditText.CallClick(() => { ActivityHelper.GotoSearch(); });
    }
}

public record HomeMenuItem(HomeMenuFlags Id, string Text)
{
    public readonly HomeMenuFlags Id = Id;
    public readonly string Text = Text;
}

public enum HomeMenuFlags : int
{
    Ingredients,
    Recipe,
    Collection
}