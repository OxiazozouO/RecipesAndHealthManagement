using System.ComponentModel.DataAnnotations;
using Android.Helper;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Android.Models;

public partial class PhysicalModel : BaseObservableValidator
{
    [ObservableProperty] private long upiId;

    [Range(30, 255, ErrorMessage = "猜你的体重比{1}kg 重, 比{2}kg 轻")] [NotifyDataErrorInfo] [ObservableProperty]
    private double weight;

    [Range(120, 255, ErrorMessage = "猜你的身高应该在{1}cm和{2}cm之间")] [NotifyDataErrorInfo] [ObservableProperty]
    private double height;

    [ObservableProperty] private ActivityLevelModel activityLevel;

    /// <summary>
    /// 每日蛋白质摄入量
    /// </summary>
    [Range(1, 255, ErrorMessage = "蛋白质摄入量必须在{1}g~{2}g之间")] [NotifyDataErrorInfo] [ObservableProperty]
    private double proteinRequirement;

    /// <summary>
    /// 脂肪供能占比 脂肪20%-30%
    /// </summary>
    [Range(0.2, 0.3, ErrorMessage = "脂肪摄入量必须在{1}%~{2}%之间")] [NotifyDataErrorInfo] [ObservableProperty]
    private double fatPercentage;

    /// <summary>
    /// 碳水供能占比  其实只要确定碳水占比就行 碳水化合物50%-65%
    /// </summary>
    [ObservableProperty] private double carbohydratePercentage;

    /// <summary>
    /// 每日碳水化合物摄入量 g
    /// </summary>
    [ObservableProperty] private double carbohydrateRequirement;

    /// <summary>
    /// 每日脂肪摄入量 g
    /// </summary>
    [ObservableProperty] private double fatRequirement;


    /// <summary>
    /// 蛋白质供能占比 直接按推荐量固定  标准+-5g  占比10%-20%
    /// </summary>
    [ObservableProperty] private double proteinPercentage;

    /// <summary>
    /// 蛋白质推荐量  来自2023中国居民膳食营养素参考摄入量 附表3-2 膳食蛋白质参考摄入量
    /// </summary>
    [ObservableProperty] private double proteinRni;

    [ObservableProperty] private DateTime createDate;

    public void Bind(MyInfoModel model)
    {
        try
        {
            TdeeGroup ??= new TdeeGroupModel();
            TdeeGroup.Bmi = NutritionalHelper.GetBmi(Weight, Height);

            TdeeGroup.Ree = NutritionalHelper.GetMifflinStJeorRee(Weight, Height, model.BirthDate, model.Gender);
            TdeeGroup.Tdee = NutritionalHelper.TDEE(TdeeGroup.Ree, ActivityLevel.Value);
            TdeeGroup.MaxTdee = TdeeGroup.Tdee * AppConfigHelper.TdeeCorrect;

            ProteinRni = AppConfigHelper.ModelConfig
                .ProteinRequirement[model.Gender][model.BirthDate.GetAge()];

            ProteinPercentage = Math.Min(ProteinPercentage == 0
                ? Math.Max(0.10, ProteinRni * 4 / TdeeGroup.MaxTdee)
                : Math.Max(0.10, ProteinPercentage), 0.20);

            ProteinRequirement = (double)((decimal)ProteinPercentage * (decimal)TdeeGroup.MaxTdee / 4);

            // a + b = c;
            // 0.5<a<0.65
            // 0.2<b<0.3
            //控制 a 和 b 在合理范围内
            var a = FatPercentage;
            var c = 1 - ProteinPercentage;

            a = c switch
            {
                >= 0.8 and <= 0.85 => Math.Min(Math.Max(0.2, a), 0.3),
                > 0.85 and <= 0.90 => Math.Min(Math.Max(c - 0.65, a), 0.3),
                _ => a
            };

            CarbohydratePercentage = c - a;
            FatPercentage = a;

            CarbohydrateRequirement = CarbohydratePercentage * TdeeGroup.MaxTdee / 4;
            FatRequirement = FatPercentage * TdeeGroup.MaxTdee / 9;
        }
        catch
        {
            // ignored
        }
    }

    [ObservableProperty] private int dayNumber;

    partial void OnCreateDateChanged(DateTime oldValue, DateTime newValue)
    {
        DayNumber = DateOnly.FromDateTime(CreateDate).DayNumber;
    }

    [ObservableProperty] private TdeeGroupModel tdeeGroup;
}