using Android.Adapter;
using Android.Attribute;
using Android.Helper;
using Android.HttpClients;
using Android.Views;
using AndroidX.CardView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using AnyLibrary.Constants;
using Google.Android.Material.Tabs;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;

namespace Android.Holder;

[ViewClassBind(Layout.page_release)] // 将 your_layout_name 替换为实际的布局文件名
public class PageReleaseHolder(App.Activity activity) : ViewHolder<View>(activity)
{
    [ViewBind(Id.id_release_category_tab)] public TabLayout ReleaseCategoryTab;

    [ViewBind(Id.id_release_status_tab)] public TabLayout ReleaseStatusTab;

    [ViewBind(Id.id_release_list_swipe_refresh)]
    public SwipeRefreshLayout SwipeRefresh;

    [ViewBind(Id.id_release_list)] public ListView ReleaseList;

    [ViewBind(Id.id_release_fab)] public CardView ReleaseFab;

    private List<(ReleaseCategory, string)> items =
    [
        (ReleaseCategory.Recipe, "食谱"),
        (ReleaseCategory.Collect, "合集")
    ];

    public static readonly Dictionary<int, string> Items2 = new()
    {
        [Status.Approve] = "已通过",
        [Status.All] = "全部",
        [Status.Pending] = "审核中",
        [Status.Other] = "其他"
    };

    private PageReleaseListAdapter Adapter;

    protected override void Init()
    {
        items.ForEach(item => ReleaseCategoryTab
            .AddTab(ReleaseCategoryTab.NewTab().SetText(item.Item2).SetId((int)item.Item1)));
        foreach (var (key, value) in Items2)
        {
            ReleaseStatusTab.AddTab(ReleaseStatusTab.NewTab().SetText(value).SetId(key));
        }

        Adapter = new PageReleaseListAdapter(activity);
        ReleaseList.Adapter = Adapter;
        ReleaseCategoryTab.TabSelected += (_, args) =>
        {
            if (ReleaseStatusTab.SelectedTabPosition != 0)
            {
                ReleaseStatusTab.SelectTab(ReleaseStatusTab.GetTabAt(0));
            }
            else
            {
                var tab = ReleaseStatusTab.GetTabAt(ReleaseCategoryTab.SelectedTabPosition);
                Update(args.Tab.Id, tab.Id);
            }
        };

        ReleaseStatusTab.TabSelected += (_, args) =>
        {
            var tab = ReleaseCategoryTab.GetTabAt(ReleaseCategoryTab.SelectedTabPosition);
            Update(tab.Id, args.Tab.Id);
        };

        Update((int)ReleaseCategory.Recipe, Status.All);

        ReleaseFab.CallClick(() =>
        {
            var tab = ReleaseCategoryTab.GetTabAt(ReleaseCategoryTab.SelectedTabPosition);
            var id1 = (ReleaseCategory)tab.Id;
            switch (id1)
            {
                case ReleaseCategory.Recipe:
                    ActivityHelper.GotoAddRecipe();
                    break;
                case ReleaseCategory.Collect:
                    ActivityHelper.GotoAddCollection();
                    break;
            }
        });

        SwipeRefresh.BindRefresh(() => { Update(); });
    }

    private int Id1;
    private int Id2;

    private void Update(int id1 = -1, int id2 = -1)
    {
        if (id1 != -1) Id1 = id1;
        if (id2 != -1) Id2 = id2;
        SwipeRefresh.Refreshing = false;
        Adapter.Models = ApiService.GetReleases((ReleaseCategory)Id1, Id2);
        var set = Adapter.Models.Select(m => m.TId).ToHashSet();
        Adapter.Status = ApiService.GetReleaseStatus((ReleaseCategory)Id1, set);

        Adapter.NotifyDataSetChanged();
    }
}

public enum ReleaseCategory
{
    Recipe = 1,
    Collect
}