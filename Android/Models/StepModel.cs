using Android.Helper;
using Android.JsonConverter;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace Android.Models;

public partial class StepModel : ObservableObject
{
    [ObservableProperty] private long id;
    [ObservableProperty] private string fileUrl;
    [ObservableProperty] private string refer;

    [ObservableProperty] private TimeSpan? requiredTime = TimeSpan.Zero;

    [ObservableProperty] private string summary;
    [ObservableProperty] private string title;

    [JsonConverter(typeof(StringToRequiredIngredientJsonConverter))]
    public Tuple<List<int>, List<int>, List<int>>? RequiredIngredient { get; set; }

    [ObservableProperty] private List<TimeRateModel> timeRateModels;
    [ObservableProperty] private Android.Net.Uri? uri;


    public void InitTimeRateModels()
    {
        if (RecipeRoot is null) return;
        TimeRateModels ??= [];
        var models = TimeRateModels;

        foreach (var ingredient in RecipeRoot.Ingredients.Where(ingredient =>
                     models.Find(m => m.Ingredient == ingredient) is null))
            models.Add(new TimeRateModel
            {
                Ingredient = ingredient,
                Root = this,
                Display = 0,
                Rate = 0,
            });

        TimeRateModel.ConvertTime(models, RequiredTime);
    }

    public void InitOutputRequiredTime()
    {
        if (stepDosageAns == 0m)
        {
            Ans += RequiredIngredient.Item3.Sum();
            decimal sum = 0m;
            if (Ans > 0.00001m)
            {
                sum = IngredientRoots.Select((e, i) =>
                        UnitHelper.ConvertToBaseUnit(e.Dosage, e.Unit) * RequiredIngredient.Item3[i] / Ans)
                    .Sum();
            }

            sum = IngredientRoots.Select((e, i) =>
                    UnitHelper.ConvertToBaseUnit(e.Dosage, e.Unit) * RequiredIngredient.Item3[i] / Ans)
                .Sum();

            stepDosageAns = sum;
        }

        var now = IngredientRoots.Select((e, i) =>
                UnitHelper.ConvertToBaseUnit(e.InputDosage, e.InputUnit) * RequiredIngredient.Item3[i] / Ans)
            .Sum();

        var time = RequiredTime * (double)(now / stepDosageAns);
        if (time is { Ticks: 0 }) time = null;

        OutputRequiredTime = time;
    }

    partial void OnRequiredTimeChanged(TimeSpan? oldValue, TimeSpan? newValue)
    {
        OutputRequiredTime = RequiredTime;
    }

    partial void OnOutputRequiredTimeChanged(TimeSpan? oldValue, TimeSpan? newValue)
    {
        newValue ??= TimeSpan.Zero;
        oldValue ??= TimeSpan.Zero;
        if (RecipeRoot != null) RecipeRoot.SpendTime += newValue.Value - oldValue.Value;
    }

    #region Root

    [ObservableProperty] private List<IngredientModel> ingredientRoots = [];
    public RecipeModel? RecipeRoot;

    #endregion


    #region 计算出来的变量

    [ObservableProperty] private TimeSpan? outputRequiredTime =
        TimeSpan.Zero;

    private decimal stepDosageAns;
    private decimal Ans;

    #endregion
}