using Android.Helper;
using Android.HttpClients;
using Android.Models;
using AnyLibrary.Constants;
using AnyLibrary.Helper;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Android.ViewModel;

public partial class RecipeViewModel : ObservableObject
{
    [ObservableProperty] private RecipeModel recipe;

    private bool tag;

    [ObservableProperty] private string releaseInfo;

    partial void OnRecipeChanged(RecipeModel? oldValue, RecipeModel newValue)
    {
        Recipe.Init();
        InitNutritional();
    }

    public void InitNutritional()
    {
        Recipe.Ingredients.ForEach(i => i.NutrientContent = NutritionalHelper
            .InitNutritional(i.Nutritional)
        );
    }

    public void Reset()
    {
        if (tag) return;
        tag = true;
        Recipe.InitDosage();
        Recipe.InitAllNutritional();
        tag = false;
    }

    public void AddCategory(string text, int flag, Action action)
    {
        var chars = text.ToCharArray();
        if (flag != CategoryType.Category && (flag != CategoryType.Emoji || chars.Length != 2)) return;
        var req = ApiEndpoints.CreatCategoryItem(new
        {
            UserId = AppConfigHelper.AppConfig.Id,
            TypeId = flag,
            TId = Recipe.RecipeId,
            IdCategory = IdCategory.Recipe,
            Name = text
        });

        if (!req.Execute(out var res)) return;
        var entity = res.Data.ToEntity<CategoryModel>();
        Recipe.Category.Add(entity);
        action.Invoke();
    }

    public void AddEatingDiary(DateTime time)
    {
        var dic = Recipe.Ingredients.ToDictionary(i => i.IngredientId, i =>
            UnitHelper.ConvertToBaseUnit(i.InputDosage, i.InputUnit));
        ApiEndpoints.AddEatingDiary(new
        {
            Id = AppConfigHelper.AppConfig.Id,
            Flag = ModelFlags.Recipe,
            Dosages = dic,
            Nutrients = Recipe.AllNutritional,
            UpdateTime = time,
            Tid = Recipe.RecipeId
        }).Execute();
    }
}