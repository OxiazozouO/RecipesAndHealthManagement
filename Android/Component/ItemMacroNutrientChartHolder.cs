using Android.Attribute;
using Android.Helper;
using Android.ViewModel;
using Android.Views;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;

namespace Android.Component;

[ViewClassBind(Layout.component_macro_nutrient_chart_item)]
public class ItemMacroNutrientChartHolder(Android.App.Activity activity) : ViewHolder<View>(activity)
{
    [ViewBind(Id.id_nutrient_chart_img)] public ImageView NutrientChartImg;

    [ViewHolderBind(Id.id_nutrient_chart_progress_view)]
    public ProgressBarHolder Progress;


    [ViewBind(Id.id_nutrient_chart_text)] public TextView NutrientChartText;

    public NutrientContentViewModel Model;

    public void Bind(NutrientContentViewModel model)
    {
        this.Model = model;
        NutrientChartImg.SetImageResource(model.Icon);
        Progress.SetColors(model.Colors.GetAndroidColor());
        Progress.SetProgress((decimal)model.Rate);
        NutrientChartText.Text = model.Str1;
    }

    protected override void Init()
    {
    }
}