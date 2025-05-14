using Android.Attribute;
using Android.Helper;
using Android.Models;
using Android.Views;
using AndroidX.SwipeRefreshLayout.Widget;
using Google.Android.Material.Tabs;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;
using Color = Android.Graphics.Color;

namespace Android.Holder;

[ViewClassBind(Layout.page_home_ingredient)]
public class PageHomeIngredientHolder(App.Activity activity) : ViewHolder<View>(activity)
{
    [ViewBind(Id.id_ingredient_tab)] public TabLayout Tab;

    [ViewBind(Id.id_page_home_ingredient_swipe_refresh)]
    public SwipeRefreshLayout SwipeRefreshLayout;

    [ViewBind(Id.id_page_home_ingredient_scroll_view)]
    public ScrollView ScrollView;

    [ViewBind(Id.id_page_home_ingredient_scroll_view_main)]
    public LinearLayout LinearLayout;

    public Func<int, int, List<IngredientInfo>> GetModels;
    private Func<int, long, bool> action;

    public void Bind(Func<int, int, List<IngredientInfo>> models, Func<int, long, bool> action)
    {
        GetModels = models;
        this.action = action;
        Tab.TabSelected += (sender, args) => { InitView(); };

        Tab.RemoveAllTabs();

        Tab.AddTab(Tab.NewTab().SetText("最新"));
        Tab.AddTab(Tab.NewTab().SetText("最热"));

        ScrollView.BindUp(AddView);
        //设置监听器,需要重写onRefresh()方法
        SwipeRefreshLayout.BindRefresh(InitView);

        ScrollView.FullScroll(FocusSearchDirection.Down);
        SwipeRefreshLayout.SetSize(CircularProgressDrawable.Large);
        SwipeRefreshLayout.SetColorSchemeColors(Color.Black, Color.Green, Color.Red, Color.Yellow, Color.Blue);

        AddView();
    }

    public bool isEnd;

    public void InitView()
    {
        LinearLayout.RemoveAllViews();
        isEnd = false;
        AddView();
    }

    public void AddView()
    {
        if (isEnd) return;

        var count = LinearLayout.ChildCount;
        View root = null;
        var models = GetModels(Tab.SelectedTabPosition, count);
        if (models == null || models.Count == 0)
        {
            root = activity.LayoutInflater.Inflate(Layout.no_more_text_view, null);
            isEnd = true;
        }
        else
        {
            var h = new LinearItemIngredientHolder(activity);
            h.Bind(models, this.action);
            root = h.Root;
        }

        LinearLayout.AddView(root);
    }

    protected override void Init()
    {
    }
}

public class IngredientInfo
{
    public IngredientModel Ingredient { set; get; }
    public UserInfo User { set; get; }

    public class UserInfo
    {
        public int UserId { set; get; }
        public string AuthorUName { set; get; }
        public string AuthorFileUrl { set; get; }

        public DateTime ModifyDate { set; get; }
    }
}