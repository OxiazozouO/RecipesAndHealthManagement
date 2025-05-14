using Android.Helper;
using Android.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Android.ViewModel;

public partial class EatingDiaryInfoViewModel : ObservableObject
{
    public int Day = 1;
    [ObservableProperty] private List<EatingDiaryAtViewModel> select;
    [ObservableProperty] private decimal energy;
    [ObservableProperty] private Dictionary<string, decimal> allNutrients;

    [ObservableProperty] private List<NutrientContentRatioViewModel> otherNutrientContent;
    [ObservableProperty] private List<NutrientContentRatioViewModel> nutrientContent;
    [ObservableProperty] private List<NutrientContentViewModel> proteinNutrient; //三大营养素模型

    partial void OnSelectChanged(List<EatingDiaryAtViewModel>? oldValue, List<EatingDiaryAtViewModel> newValue)
    {
        var dic = new Dictionary<string, decimal>();
        foreach (var model in newValue)
        foreach (var (key, value) in model.EatingDiary.Nutrients)
        {
            dic.TryAdd(key, 0);
            dic[key] += value;
        }

        dic = dic.OrderByDescending(kv => kv.Value)
            .ToDictionary(kv => kv.Key, kv => kv.Value);

        Energy = Select.Sum(e => e.Energy) / Day;

        AllNutrients = dic;
        OtherNutrientContent = (from r in AllNutrients
            where !string.IsNullOrEmpty(r.Key)
                  && !r.Key.Contains("热量")
                  && !r.Key.Contains("碳水化合物")
                  && !r.Key.Contains("蛋白质")
                  && !r.Key.Contains("脂肪")
            select new NutrientContentRatioViewModel
            {
                Name = r.Key,
                Value = r.Value
            }).ToList();
        NutrientContent = (from r in AllNutrients
            where !string.IsNullOrEmpty(r.Key)
                  && (r.Key.Contains("热量")
                      || r.Key.Contains("碳水化合物")
                      || r.Key.Contains("蛋白质")
                      || r.Key.Contains("脂肪"))
            select new NutrientContentRatioViewModel
            {
                Name = r.Key,
                Value = r.Value
            }).ToList();
        ProteinNutrient = NutritionalHelper.InitNutritional(dic, false, true);
        foreach (var m in OtherNutrientContent)
        {
            m.Value /= Day;
        }

        foreach (var m in ProteinNutrient)
        {
            m.Pos /= Day;
            m.Str1 = $"{m.Rate:0.##} NRV%";
        }

        OtherNutrientContent.ForEach(o => o.InitMaxValue());
        NutrientContent.ForEach(o => o.InitMaxValue());
    }
}