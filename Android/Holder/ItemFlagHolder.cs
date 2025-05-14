using Android.Attribute;
using Android.Helper;
using Android.Views;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;

namespace Android.Holder;

[ViewClassBind(Layout.item_flag)]
public class ItemFlagHolder(App.Activity activity) : ViewHolder<View>(activity)
{
    [ViewBind(Id.id_flag_name)] public TextView Name;

    protected override void Init()
    {
    }

    public void Bind(string name, int bg, int text)
    {
        activity.SetTextColorResource(Name, text);
        Name.SetBackgroundResource(bg);
        Name.Text = name;
    }
}