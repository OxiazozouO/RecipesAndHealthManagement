using Android.Adapter;
using Android.Attribute;
using Android.Helper;
using Android.Views;
using AndroidX.ViewPager.Widget;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;

namespace Android.Component;

[ViewClassBind(Layout.component_data_paked_content)]
public class DataPackedContentHolder(View view) : ViewHolder<View>(view)
{
    [ViewBind(Id.id_data_pake_content)] public ViewPager MainContent;

    public DataPakedAdapter Adapter;

    protected override void Init()
    {
    }

    public void Init(App.Activity activity, DataPackedBarHolder holder)
    {
        Adapter = new DataPakedAdapter(activity, Root);
        Adapter.Init(MainContent);
        Adapter.OnItemClick += (seder, pos) =>
        {
            var time = Adapter.GetDateTime(pos, out _, out _);
            holder.Bind(time);
        };

        holder.todayIcon.CallClick(() =>
        {
            MsgBoxHelper
                .Builder("请选择日期", "日期选择")
                .AddDatePicker(Adapter.GetDateTime(Adapter.SelectedPos, out _, out _))
                .Show(list =>
                {
                    var time = (DateTime)list[0];
                    holder.Bind(time);
                    Adapter.Goto(time);
                });
        });
    }
}