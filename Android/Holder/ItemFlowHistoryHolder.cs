using Android.Attribute;
using Android.Models;
using Android.Views;
using AnyLibrary.Helper;
using Bumptech.Glide;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;

namespace Android.Holder;

[ViewClassBind(Layout.item_flow_history)]
public class ItemFlowHistoryHolder(App.Activity activity) : ViewHolder<View>(activity)
{
    [ViewBind(Id.id_item_flow_history_status_img)]
    public View StatusImg;

    [ViewBind(Id.id_item_flow_history_time)]
    public TextView Time;

    [ViewBind(Id.id_item_flow_history_info)]
    public TextView Info;

    [ViewBind(Id.id_item_flow_history_admin_img)]
    public ImageView AdminImg;

    [ViewBind(Id.id_item_flow_history_admin_name)]
    public TextView AdminName;

    protected override void Init()
    {
    }

    public void Bind(ReleaseFlowHistory r)
    {
        if (PageReleaseListItemHolder.Items.TryGetValue(r.Status, out var flag))
            StatusImg.SetBackgroundResource(flag.Bg);

        Time.Text = r.CreateDate.TimeStr1();
        Info.Text = r.Info;
        AdminName.Text = r.User.Name;
        Glide.With(Root).Load(r.User.FileUrl).Into(AdminImg);
    }
}