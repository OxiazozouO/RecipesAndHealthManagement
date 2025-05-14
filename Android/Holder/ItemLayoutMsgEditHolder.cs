using Android.Attribute;
using Android.Views;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;

namespace Android.Holder;

[ViewClassBind(Layout.item_layout_msg_edit)]
public class ItemLayoutMsgEditHolder(App.Activity activity): ViewHolder<View>(activity)
{
    [ViewBind(Id.id_msg_edit_text)] public TextView MsgText;
    [ViewBind(Id.id_msg_edit)] public EditText MsgEdit;
    protected override void Init()
    {
    }
}

[ViewClassBind(Layout.msg_time)]
public class MsgTimeHolder(App.Activity activity) : ViewHolder<TimePicker>(activity)
{
    protected override void Init()
    {
    }
}