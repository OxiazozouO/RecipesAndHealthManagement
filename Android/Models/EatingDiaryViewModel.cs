using Android.Helper;
using Android.ViewModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Generators;

namespace Android.Models;

public partial class EatingDiaryViewModel : ObservableObject
{
    [ObservableProperty] private DateTime startTime;

    [ObservableProperty] private List<EatingDiaryAtViewModel> eatingDiaries;

    [ObservableProperty] private Dictionary<DateTime, EatingDiaryBarViewModel> eatingDiaryBar;


    partial void OnEatingDiariesChanged(List<EatingDiaryAtViewModel>? oldValue, List<EatingDiaryAtViewModel> newValue)
    {
        if (oldValue is not null) return;
        EatingDiaries = (from d in EatingDiaries orderby d.EatingDiary.TieUpDate select d)
            .ToList();


        var dir = AppConfigHelper
            .MyInfo
            .GetAllPhysicalModels(StartTime);
        EatingDiaryBar = dir
            .ToDictionary(d => d.Key, d => new EatingDiaryBarViewModel
            {
                TdeeGroup = d.Value,
                Energy = 0
            });
        //按日期分组 同日期的Energy累加
        foreach (var model in EatingDiaries)
        {
            var date = model.EatingDiary.TieUpDate.Date;
            if (EatingDiaryBar.TryGetValue(date, out var bar))
                bar.Energy += model.Energy;
        }

        var list = EatingDiaryBar.Values.Select(d => d.Energy).ToList();
        MaxEnergy = list.Count == 0 ? 0 : list.Max();
    }

    partial void OnMaxEnergyChanged(decimal oldValue, decimal newValue)
    {
        if (IsOnlyUpdate(nameof(MaxEnergy))) return;
        if (EatingDiaryBar.TryGetValue(StartTime, out var bar2))
        {
            OnlyUpdateMaxEnergy = Math.Max(MaxEnergy, (decimal)bar2.TdeeGroup.MaxTdee);
        }

        OnlyUpdateMaxEnergy = MaxEnergy == 0 ? 1 : MaxEnergy;
    }

    #region 计算出来的变量

    [ObservableProperty] [AddOnlyUpdate] private decimal maxEnergy;

    [ObservableProperty] private double maxWidth;

    #endregion
}

public partial class EatingDiaryBarViewModel : ObservableObject
{
    [ObservableProperty] private TdeeGroupModel tdeeGroup;
    [ObservableProperty] private decimal energy;
}