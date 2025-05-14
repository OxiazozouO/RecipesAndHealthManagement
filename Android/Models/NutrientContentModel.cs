using Android.Helper;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Android.Models;

public partial class NutrientContentModel : ObservableObject
{
    [ObservableProperty] private int[] color;
    [ObservableProperty] private string name;
    [ObservableProperty] private string unit;
    [ObservableProperty] private decimal value;
    [ObservableProperty] private double rate;
}

public partial class NutrientContentRatioViewModel : ObservableObject
{
    [ObservableProperty] private int[] color;
    [ObservableProperty] private decimal ear;

    [ObservableProperty] private decimal maxValue;
    [ObservableProperty] private string name;

    [ObservableProperty] private decimal nrv;
    [ObservableProperty] private string outUnit;

    [ObservableProperty] private decimal outValue;
    [ObservableProperty] private decimal rni;
    [ObservableProperty] private decimal ul;
    [ObservableProperty] private string unit;
    [ObservableProperty] private decimal value;

    public void InitMaxValue()
    {
        var config = AppConfigHelper.ModelConfig.NutrientConfig;
        if (config.EAR.TryGetValue(Name, out var v1))
        {
            MaxValue = Math.Max(Value, (decimal)v1);
            Ear = (decimal)v1;
        }

        if (config.RNI.TryGetValue(Name, out var v2))
        {
            MaxValue = Math.Max(Value, (decimal)v2);
            Rni = (decimal)v2;
        }

        if (config.UL.TryGetValue(Name, out var v3))
        {
            MaxValue = Math.Max(Value, (decimal)v3);
            Ul = (decimal)v3;
        }

        if (MaxValue == 0) MaxValue = 10;

        Color = ColorHelper.GetColor(Value, Ear, Rni, Ul);

        if (Rni > 0)
            Nrv = Value * 100m / Rni;
    }


    partial void OnMaxValueChanged(decimal oldValue, decimal newValue)
    {
        Unit = AppConfigHelper.ModelConfig.TryGetNutritional(Name, out var v) ? v : "";
        UnitHelper.ConvertToClosestUnit(Value, Unit, out var result, out var output);
        OutUnit = output;
        OutValue = result;
    }
}

public class NutrientContentRatioModel
{
    public int[] Color;
    public decimal Ear;
    public decimal MaxValue;
    public string Name;
    public decimal Nrv;
    public decimal Rni;
    public decimal Ul;
    public string Unit;
    public decimal Value;
}