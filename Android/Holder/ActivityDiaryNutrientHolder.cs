using Android.Attribute;
using Android.Component;
using Android.Helper;
using Android.Models;
using Android.Views;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;

namespace Android.Holder;

[ViewClassBind(Layout.activity_diary_nutrient)]
public class ActivityDiaryNutrientHolder(App.Activity activity) : ViewHolder<View>(activity)
{
    [ViewBind(Id.id_diary_nutrient_name)] public TextView NutrientName;

    [ViewBind(Id.id_diary_nutrient_dotted_line_1)]
    public View DottedLine1;

    [ViewBind(Id.id_diary_nutrient_dotted_line_2)]
    public View DottedLine2;

    [ViewBind(Id.id_diary_nutrient_dotted_line_3)]
    public View DottedLine3;

    [ViewHolderBind(Id.id_diary_nutrient_progress)]
    public ProgressBarHolder Progress;

    [ViewBind(Id.id_diary_dosage)] public TextView Dosage;
    [ViewBind(Id.id_diary_nutrient_nrv)] public TextView NutrientNRV;
    [ViewBind(Id.id_diary_nutrient_ear)] public TextView EAR;
    [ViewBind(Id.id_diary_nutrient_rni)] public TextView RNI;
    [ViewBind(Id.id_diary_nutrient_ul)] public TextView UL;

    [ViewBind(Id.id_diary_nutrient_evaluate)]
    public TextView NutrientEvaluate;

    [ViewBind(Id.id_diary_nutrient_info)] public TextView NutrientInfo;

    protected override void Init()
    {
    }

    public void Bind(NutrientContentRatioModel model)
    {
        NutrientName.Text = model.Name;
        NutrientNRV.Text = model.Nrv > 0.001m
            ? $"{model.Nrv:0.00} NRV%"
            : $"{model.Value:0.000} {model.Unit}";
        Dosage.Text = $"{model.Value:0.000} {model.Unit}";

        Progress.SetColors(model.Color.GetAndroidColor());
        Progress.SetProgress(model.Value, model.MaxValue);

        var width = Progress.Width;

        if (model.Ear > 0.001m)
        {
            EAR.Text = $"{model.Ear:0.00} {model.Unit}";
            Update(DottedLine1, p => p.LeftMargin = -width + Progress.CalculationProgress(model.Ear, model.MaxValue));
        }
        else
        {
            EAR.Text = "";
            DottedLine1.Visibility = ViewStates.Gone;
        }

        if (model.Rni > 0.001m)
        {
            RNI.Text = $"{model.Rni:0.00} {model.Unit}";
            Update(DottedLine2, p => p.LeftMargin = -width + Progress.CalculationProgress(model.Rni, model.MaxValue));
        }
        else
        {
            RNI.Text = "";
            DottedLine2.Visibility = ViewStates.Gone;
        }

        if (model.Ul > 0.001m)
        {
            UL.Text = $"{model.Ul:0.00} {model.Unit}";
            Update(DottedLine3, p => p.LeftMargin = -width + Progress.CalculationProgress(model.Ul, model.MaxValue));
        }
        else
        {
            UL.Text = "";
            DottedLine3.Visibility = ViewStates.Gone;
        }

        foreach (var nutrient in AppConfigHelper.ModelConfig.Nutrients.Where(nutrient => model.Name == nutrient.Name))
        {
            NutrientInfo.Text = nutrient.Refer;
            break;
        }

        var sd = NutritionalHelper.GetSd(model.Ear, model.Rni, model.Ul, 1, model.Value, model.Name);
        NutrientEvaluate.Text = sd;
    }

    private void Update(View view, Action<FrameLayout.LayoutParams> action)
    {
        var parameters = (FrameLayout.LayoutParams)view.LayoutParameters;
        action.Invoke(parameters);
        view.LayoutParameters = parameters;
    }
}