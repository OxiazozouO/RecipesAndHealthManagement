using Android.Attribute;
using Android.Views;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;

namespace Android.Holder;

[ViewClassBind(Layout.item_layout_msg)]
public class ItemLayoutMsgHolder(App.Activity activity) : ViewHolder<View>(activity)
{
    [ViewBind(Id.id_msg_img)] public ImageView MsgImg;

    [ViewBind(Id.id_msg_menu_text)]
    public TextView MsgMenuText;

    protected override void Init()
    {
    }

    public void Bind(MsgItem item)
    {
        if (item.Icon != null)
            MsgImg.SetImageResource(item.Icon);
        MsgMenuText.Text = item.Text;
    }
}

public class MsgItem
{
    public int Icon { get; set; }
    public string Text { get; set; }
}