using CommunityToolkit.Mvvm.ComponentModel;

namespace Android.Models;

public partial class NutrientModel : ObservableObject
{
    [ObservableProperty] private int id;
    [ObservableProperty] private string name;
    [ObservableProperty] private string unit;
    [ObservableProperty] private string? refer;
}