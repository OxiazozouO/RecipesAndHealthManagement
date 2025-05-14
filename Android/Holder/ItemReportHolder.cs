using Android.Attribute;
using Android.Views;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;

namespace Android.Holder;

[ViewClassBind(Layout.item_report)]
public class ItemReportHolder(App.Activity activity) : ViewHolder<View>(activity)
{
    [ViewBind(Id.id_item_report_img)] public ImageView Img;
    [ViewBind(Id.id_item_report_text)] public TextView Text;

    public int id;

    protected override void Init()
    {
    }

    public void Bind(int id, string name)
    {
        Root.Tag = this;
        this.id = id;
        Text.Text = name;
    }
}