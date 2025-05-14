using CommunityToolkit.Mvvm.ComponentModel;

namespace Android.Models;

public partial class ActivityLevelModel : ObservableObject
{
    [ObservableProperty] private int id;
    [ObservableProperty] private string name;
    [ObservableProperty] private double value;
}