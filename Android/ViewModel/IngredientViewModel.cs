using System.ComponentModel;
using Android.Helper;
using Android.HttpClients;
using Android.Models;
using AnyLibrary.Constants;
using AnyLibrary.Helper;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Android.ViewModel;

public partial class IngredientViewModel : ObservableObject
{
    [ObservableProperty] private List<CategoryModel> category;
    [ObservableProperty] private IngredientModel ingredient;

    partial void OnIngredientChanged(IngredientModel? oldValue, IngredientModel newValue)
    {
        Ingredient.PropertyChanged += IngredientModelOnPropertyChanged;
        if (Ingredient.EstimatedDosage > 0)
            InitAllNutritional();

        var ans = UnitHelper.ConvertToBaseUnit(Ingredient.Dosage, Ingredient.Unit) *
                  (decimal)Ingredient.Content;
        var list = NutritionalHelper.GetEvaluateTag(Ingredient.Nutritional, ans);
        if (Ingredient is { Allergy: not null, InputDosage: > 0 })
            list.Add(new Tuple<EvaluateTag, string>(EvaluateTag.Allergy, Ingredient.Allergy));

        if (Category is not null && Category.Any(model => model.Name.Contains("酒")))
            list.Add(new Tuple<EvaluateTag, string>(EvaluateTag.Negative, "含酒精"));

        Evaluate = NutritionalHelper.ToDictionary(list);
    }

    private void IngredientModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(Ingredient.EstimatedDosage)) return;
        if (Ingredient.EstimatedDosage > 0)
            InitAllNutritional();
    }

    public void AddCategory(string text, sbyte flag, Action action)
    {
        var chars = text.ToCharArray();
        if (flag != CategoryType.Category && (flag != CategoryType.Emoji || chars.Length != 2)) return;
        var req = ApiEndpoints.CreatCategoryItem(new
        {
            UserId = AppConfigHelper.AppConfig.Id,
            TypeId = flag,
            TId = Ingredient.IngredientId,
            IdCategory = IdCategory.Ingredient,
            Name = text
        });

        if (!req.Execute(out var res)) return;
        var entity = res.Data.ToEntity<CategoryModel>();
        Category.Add(entity);
        action.Invoke();
    }

    public void AddEatingDiary(DateTime time)
    {
        var n = Ingredient.GetUpdatedNutritional();
        var dos = new Dictionary<long, decimal>
        {
            [Ingredient.IngredientId] = UnitHelper.ConvertToBaseUnit(Ingredient.InputDosage, Ingredient.InputUnit)
        };

        var req = ApiEndpoints.AddEatingDiary(new
        {
            Id = AppConfigHelper.AppConfig.Id,
            Flag = IdCategory.Ingredient,
            Dosages = dos,
            Nutrients = n,
            UpdateTime = time,
            Tid = Ingredient.IngredientId
        });

        req.Execute();
    }

    public void InitAllNutritional()
    {
        var dic = Ingredient.GetUpdatedNutritional();
        NutritionalHelper.InitAllNutritional(dic, out var energy, out var protein, out var other);
        EnergyNutrient = energy;
        ProteinNutrient = protein;
        OtherNutrient = other;
    }

    #region 计算出来的变量

    [ObservableProperty] private Dictionary<EvaluateTag, List<string>> evaluate; //评价
    [ObservableProperty] private List<Tuple<string, decimal>> energyNutrient; //能量模型
    [ObservableProperty] private List<NutrientContentViewModel> proteinNutrient; //三大营养素模型
    [ObservableProperty] private List<NutrientContentModel> otherNutrient; //其他营养素

    #endregion
}