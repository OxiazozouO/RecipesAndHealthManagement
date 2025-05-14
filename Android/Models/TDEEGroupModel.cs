using Android.Helper;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Android.Models;

public partial class TdeeGroupModel : ObservableObject
{
    [ObservableProperty] private double bmi;
    [ObservableProperty] private string bmiStr;

    /// <summary>
    /// 静息代谢
    /// </summary>
    [ObservableProperty] private double ree;

    /// <summary>
    /// 基础代谢
    /// </summary>
    [ObservableProperty] private double tdee;

    [ObservableProperty] private double maxTdee;

    partial void OnBmiChanged(double oldValue, double newValue) =>
        BmiStr = NutritionalHelper.GetBmiString(Bmi);
}