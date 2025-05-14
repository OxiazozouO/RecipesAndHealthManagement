using Android.Attribute;
using Android.Helper;
using Android.Models;
using Android.Views;
using AnyLibrary.Helper;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;

namespace Android.Holder;

[ViewClassBind(Layout.item_nutrient)]
public class ItemNutrientHolder(App.Activity activity) : ViewHolder<View>(activity)
{
    [ViewBind(Id.id_nutrient_author_avatar)]
    public View AuthorAvatar;

    [ViewBind(Id.id_nutrient_value)]
    public TextView Value;

    [ViewBind(Id.id_nutrient_unit)]
    public TextView Unit;

    [ViewBind(Id.id_nutrient_name)]
    public TextView Name;

    protected override void Init()
    {
    }

    public void Bind(NutrientContentModel holder)
    {
        AuthorAvatar.Background = ColorHelper.CreatDrawable(holder.Color);
        Value.Text = holder.Value.ShortStr();
        Unit.Text = holder.Unit;
        Name.Text = holder.Name;
    }
}