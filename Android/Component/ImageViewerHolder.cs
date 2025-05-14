using Android.Adapter;
using Android.Attribute;
using Android.Helper;
using Android.Views;
using AndroidX.CardView.Widget;
using AndroidX.ViewPager.Widget;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;

namespace Android.Component;

[ViewClassBind(Layout.component_image_viewer)]
public class ImageViewerHolder(View view) : ViewHolder<View>(view)
{
    [ViewBind(Id.id_page_viewpager)] public ViewPager ViewPager;
    [ViewBind(Id.id_image_viewer_toolbar)] public LinearLayout ToolBar;
    [ViewBind(Id.id_image_viewer_back)] public ImageView Back;
    [ViewBind(Id.id_image_page_title)] public TextView Title;
    [ViewBind(Id.id_image_page_count)] public TextView Count;

    [ViewBind(Id.id_image_viewer_data_view)]
    public CardView DataView;

    [ViewBind(Id.id_image_viewer_data)] public TextView Data;

    protected override void Init()
    {
    }

    private List<(string, string, string)> list;

    public void Bind(App.Activity activity, List<(string, string, string)> list, int ind, Action ret)
    {
        this.list = list;
        var adapter = new ImageAdapter(activity, list.Select(l => l.Item2).ToList());
        ViewPager.Adapter = adapter;

        //预加载
        ViewPager.OffscreenPageLimit = 3;

        ViewPager.PageSelected += (_, args) => ViewPagerOnPageSelected(args.Position);

        int i = 0;
        adapter.OnItemClick += (sender, args) =>
        {
            if (sender is not ZoomImageView z) return;
            if (z.IsDeselect)
            {
                Root.SetBackgroundColor(i switch
                {
                    0 => Graphics.Color.Black,
                    1 => Graphics.Color.White,
                    2 => Graphics.Color.Gray,
                    3 => Graphics.Color.Transparent,
                    _ => Graphics.Color.Black
                });
                i = (i + 1) % 4;
            }
            else
            {
                var v = ToolBar.Visibility == ViewStates.Visible
                    ? ViewStates.Invisible
                    : ViewStates.Visible;
                ToolBar.Visibility = v;
                DataView.Visibility = string.IsNullOrEmpty(Data.Text)
                    ? ViewStates.Gone
                    : v;
            }
        };
        adapter.OnItemScaleBegin += (_, _) => { ToolBar.Visibility = ViewStates.Gone; };

        Back.CallClick(ret);

        //默认图
        ViewPager.CurrentItem = ind;
        ViewPagerOnPageSelected(ind);
    }

    private void ViewPagerOnPageSelected(int pos)
    {
        var (name, url, data) = list[pos];
        Count.Text = $"{pos + 1}/{list.Count}";
        Title.Text = name;
        Data.Text = data;
        DataView.Visibility = string.IsNullOrEmpty(data)
            ? ViewStates.Gone
            : ViewStates.Visible;
    }
}