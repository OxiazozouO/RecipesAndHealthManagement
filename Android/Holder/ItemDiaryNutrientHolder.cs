using Android.Attribute;
using Android.Component;
using Android.Helper;
using Android.Models;
using Android.Views;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;

namespace Android.Holder;

[ViewClassBind(Layout.item_diary_nutrient)]
public class ItemDiaryNutrientHolder(App.Activity activity) : ViewHolder<View>(activity)
{
    [ViewBind(Id.id_diary_nutrient_dotted_line_1)]
    public View DottedLine1;

    [ViewBind(Id.id_diary_nutrient_dotted_line_2)]
    public View DottedLine2;

    [ViewBind(Id.id_diary_nutrient_dotted_line_3)]
    public View DottedLine3;

    [ViewHolderBind(Id.id_diary_nutrient_progress)]
    public ProgressBarHolder Progress;
    [ViewBind(Id.id_diary_nutrient_nrv)] public TextView Nrv;

    [ViewBind(Id.id_diary_nutrient_name)] public TextView Name;
    [ViewBind(Id.id_diary_nutrient_chart_layout)]
    public FrameLayout ChartLayout;

    protected override void Init()
    {
    }

    public void Bind(NutrientContentRatioViewModel model)
    {
        Name.Text = model.Name;
        Nrv.Text = model.Nrv > 0.001m
            ? $"{model.Nrv:0.00} NRV%"
            : $"{model.OutValue:0.000} {model.OutUnit}";
        // Dosage.Text = $"{model.OutValue:0.000} {model.OutUnit}";

        Progress.SetColors(model.Color.GetAndroidColor());
        Progress.SetProgress(model.Value, model.MaxValue);

        var width = Progress.Width;

        if (model.Ear > 0.001m)
        {
            // Ear.Text = $"{model.Ear:0.00} {model.Unit}";
            Update(DottedLine1, p => p.LeftMargin = -width + Progress.CalculationProgress(model.Ear, model.MaxValue));
        }
        else
        {
            // Ear.Text = "";
            // EarText.Text = "";
            DottedLine1.Visibility = ViewStates.Gone;
        }

        if (model.Rni > 0.001m)
        {
            // Rni.Text = $"{model.Rni:0.00} {model.Unit}";
            Update(DottedLine2, p => p.LeftMargin = -width + Progress.CalculationProgress(model.Rni, model.MaxValue));
        }
        else
        {
            // Rni.Text = "";
            // RniText.Text = "";
            DottedLine2.Visibility = ViewStates.Gone;
        }

        if (model.Ul > 0.001m)
        {
            // Ul.Text = $"{model.Ul:0.00} {model.Unit}";
            Update(DottedLine3, p => p.LeftMargin = -width + Progress.CalculationProgress(model.Ul, model.MaxValue));
        }
        else
        {
            // Ul.Text = "";
            // UlText.Text = "";
            DottedLine3.Visibility = ViewStates.Gone;
        }

        if (model is { Ear: < 0.001m, Rni: < 0.001m, Ul: < 0.001m })
        {
            ChartLayout.Visibility = ViewStates.Invisible;
        }
    }

    private void Update(View view, Action<FrameLayout.LayoutParams> action)
    {
        var parameters = (FrameLayout.LayoutParams)view.LayoutParameters;
        action.Invoke(parameters);
        view.LayoutParameters = parameters;
    }
}