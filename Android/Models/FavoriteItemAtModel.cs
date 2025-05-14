using Android.Helper;
using Android.ViewModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Android.Models;

public partial class FavoriteItemModel : ObservableObject
{
    [ObservableProperty] private string fileUrl;
    [ObservableProperty] private long id;
    [ObservableProperty] private string name;
    [ObservableProperty] private string refer;
    [ObservableProperty] private List<List<NutrientContentDto>> nutrients;
    [ObservableProperty] private List<NutrientContentViewModel> nutrientContent;
    [ObservableProperty] private int favoriteCount;

    partial void OnNutrientsChanged(List<List<NutrientContentDto>> value)
    {
        Dictionary<string, decimal> dir = new Dictionary<string, decimal>();
        foreach (var dto in value.SelectMany(decimals => decimals.Where(dto => !dir.TryAdd(dto.Name, dto.Value))))
        {
            dir[dto.Name] += dto.Value;
        }

        NutrientContent =
            NutritionalHelper.InitNutritional(dir);
    }

    public record NutrientContentDto
    {
        public string Name { get; set; }
        public decimal Value { get; set; }
    }
}