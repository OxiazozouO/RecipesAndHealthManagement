using Android.Helper;
using Android.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using Generators;

namespace Android.ViewModel;

public partial class EatingDiaryAtViewModel : ObservableObject
{
    [ObservableProperty] private EatingDiaryModel eatingDiary;

    [ObservableProperty] [AddOnlyUpdate] private decimal dosage;
    [ObservableProperty] private decimal energy;
    [ObservableProperty] private string outUnit;

    partial void OnEatingDiaryChanged(EatingDiaryModel? oldValue, EatingDiaryModel newValue)
    {
        Dosage = EatingDiary.Dosages.Values.Sum();
        EatingDiary.Nutrients.TryGetValue("热量 kcal", out var e);
        Energy = e;
    }

    partial void OnDosageChanged(decimal oldValue, decimal newValue)
    {
        if (IsOnlyUpdate(nameof(Dosage))) return;

        UnitHelper.ConvertToClosestUnit(Dosage, "g", out var n, out var output);
        OnlyUpdateDosage = n;
        OutUnit = output;
    }
}