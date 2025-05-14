using Android.Attribute;
using Android.Views;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;

namespace Android.Holder;

[ViewClassBind(Layout.item_menu_button)]
public class ItemMenuButtonHolder(App.Activity activity) : ViewHolder<View>(activity)
{
    [ViewBind(Id.id_menu_button_text)]
    public TextView Text;

    [ViewBind(Id.id_menu_button_image)]
    public ImageView Img;

    public ItemMenuButtonHolder Bind(BoomMenuItem item)
    {
        Text.Text = item.Text;
        Img.SetImageResource(item.Icon);
        Root.Tag = (int)item.Id;
        return this;
    }

    public ItemMenuButtonHolder Bind(MenuItem item)
    {
        Text.Text = item.Text;
        Img.SetImageResource(item.Icon);
        Root.Tag = item.Id;
        return this;
    }

    protected override void Init()
    {
    }
}

public class BoomMenuItem(BoomMenuFlags id, string text, int icon)
{
    public BoomMenuFlags Id { get; set; } = id;
    public string Text { get; set; } = text;
    public int Icon { get; set; } = icon;
}

public enum BoomMenuFlags
{
    Index,
    Physical,
    Diary,
    Release,
    Settings,
}