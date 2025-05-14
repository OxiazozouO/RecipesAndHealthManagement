using Android.Attribute;
using Android.ViewModel;
using Android.Views;
using AndroidX.SwipeRefreshLayout.Widget;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;
using Color = Android.Graphics.Color;

namespace Android.Holder;

[ViewClassBind(Layout.page_home_recipe)]
public class PageHomeRecipeHolder(App.Activity activity) : ViewHolder<View>(activity)
{
    [ViewBind(Id.id_page_home_recipe_swipe_refresh)]
    public SwipeRefreshLayout SwipeRefreshLayout;

    [ViewBind(Id.id_page_home_recipe_scroll_view)]
    public ScrollView ScrollView;

    [ViewBind(Id.id_page_home_recipe_scroll_view_main)]
    public LinearLayout LinearLayout;

    public Func<int, List<RecipeInfoViewModel>> GetModels;
    public Func<int, long, bool> action;

    public void Bind(Func<int, List<RecipeInfoViewModel>> models, Func<int, long, bool> action)
    {
        GetModels = models;
        this.action = action;

        bool isEnd = false;
        ScrollView.ScrollChange += (sender, e) =>
        {
            if (ScrollView.MeasuredHeight <= ScrollView.ScrollY + ScrollView.Height)
            {
                addView();
            }
        };
        ScrollView.FullScroll(FocusSearchDirection.Down);
        SwipeRefreshLayout.SetSize(CircularProgressDrawable.Large);
        SwipeRefreshLayout.SetColorSchemeColors(Color.Black, Color.Green, Color.Red, Color.Yellow, Color.Blue);

        addView();

        //设置监听器,需要重写onRefresh()方法
        SwipeRefreshLayout.Refresh += (_, _) =>
        {
            LinearLayout.RemoveAllViews();
            isEnd = false;
            addView();
            SwipeRefreshLayout.Refreshing = false;
        };

        void addView()
        {
            if (isEnd) return;
            var count = LinearLayout.ChildCount;
            View root = null;
            var models = GetModels(count);
            if (models == null || models.Count == 0)
            {
                root = activity.LayoutInflater.Inflate(Layout.no_more_text_view, null);
                isEnd = true;
            }
            else
            {
                var h = new LinearRecipeHolder(activity);
                h.Bind(models, this.action);
                root = h.Root;
            }

            LinearLayout.AddView(root);
        }
    }

    protected override void Init()
    {
    }
}