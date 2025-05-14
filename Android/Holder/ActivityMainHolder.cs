using Android.Attribute;
using Android.Views;
using Google.Android.Material.Tabs;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;

namespace Android.Holder;

[ViewClassBind(Layout.activity_main)]
public class MainPageHolder(App.Activity activity) : ViewHolder<View>(activity)
{
    [ViewBind(Id.id_main_tab)] public TabLayout Tab;

    [ViewBind(Id.id_main_container)]
    public FrameLayout Frame;

    protected override void Init()
    {
        List<BoomMenuItem> items =
        [
            new(BoomMenuFlags.Index, "首页", Drawable.ic_home),
            new(BoomMenuFlags.Physical, "身材", Drawable.ic_physical),
            new(BoomMenuFlags.Release, "发布", Drawable.ic_release),
            new(BoomMenuFlags.Diary, "日记", Drawable.ic_diary),
            new(BoomMenuFlags.Settings, "我的", Drawable.ic_settings)
        ];

        Tab.TabSelected += (sender, args) =>
        {
            var item = items[Tab.SelectedTabPosition];
            View? v = item.Id switch
            {
                BoomMenuFlags.Index => new PageHomeHolder(activity).Root,
                BoomMenuFlags.Physical => new PagePhysicalHolder(activity).Root,
                BoomMenuFlags.Diary => new PageDiaryHolder(activity).Root,
                BoomMenuFlags.Release => new PageReleaseHolder(activity).Root,
                BoomMenuFlags.Settings => new PageSettingHolder(activity).Root,
                _ => null
            };

            if (v is null) return;
            Frame.RemoveAllViews();
            Frame.AddView(v);
        };
        Tab.RemoveAllTabs();

        items.ForEach(item => Tab.AddTab(Tab.NewTab().SetCustomView(new ItemMenuButtonHolder(activity).Bind(item).Root)));
    }
}