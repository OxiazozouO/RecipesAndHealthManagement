using Android.Attribute;
using Android.Component;
using Android.Helper;
using Android.Models;
using Android.Views;
using AnyLibrary.Helper;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;

namespace Android.Holder;

[ViewClassBind(Layout.item_ingredient_info)]
public class ItemIngredientInfoHolder(App.Activity activity) : ViewHolder<View>(activity)
{
    [ViewBind(Id.id_ingredient_info_author_avatar)]
    public View Avatar;

    [ViewBind(Id.id_ingredient_info_name)]
    public TextView Name;

    [ViewBind(Id.id_ingredient_info_dosage)]
    public TextView Dosage;

    [ViewBind(Id.id_ingredient_info_unit)]
    public TextView Unit;

    [ViewHolderBind(Id.id_ingredient_chart_progress_view)]
    public ProgressBarHolder ChartHolder;

    protected override void Init()
    {
    }

    public void Bind(NutrientContentModel model)
    {
        Avatar.Background = ColorHelper.CreatDrawable(model.Color);

        Name.Text = model.Name;
        Dosage.Text = model.Value.ShortStr();
        Unit.Text = model.Unit;
        ChartHolder.SetColors(model.Color.GetAndroidColor());
        ChartHolder.SetProgress((decimal)model.Rate);
    }
}