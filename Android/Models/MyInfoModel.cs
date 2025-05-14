using System.ComponentModel.DataAnnotations;
using Android.Helper;
using CommunityToolkit.Mvvm.ComponentModel;
using Generators;

namespace Android.Models;

public partial class MyInfoModel : BaseObservableValidator
{
    [Required(ErrorMessage = "生日为必填项")] [NotifyDataErrorInfo] [ObservableProperty]
    private DateTime birthDate;

    [Required(ErrorMessage = "性别为必填项")] [NotifyDataErrorInfo] [ObservableProperty]
    private bool gender;

    [StringLength(200, ErrorMessage = "长度必须比{1}短")] [NotifyDataErrorInfo] [ObservableProperty]
    private string fileUrl;

    [ObservableProperty] [AddOnlyUpdate] private List<PhysicalModel> physicalList;

    [Required(ErrorMessage = "昵称为必填项")]
    [StringLength(20, ErrorMessage = "长度必须比{1}短")]
    [Display(Name = "用户名")]
    [NotifyDataErrorInfo]
    [ObservableProperty]
    private string userName;

    [ObservableProperty] private PhysicalModel physical;

    public void InitPhysical()
    {
        if (PhysicalList.Count == 0)
        {
            Physical = new PhysicalModel
            {
                Weight = 30,
                Height = 120,
                ActivityLevel = AppConfigHelper.ModelConfig.Cals[0],
                CreateDate = DateTime.Now
            };
            PhysicalList.Add(Physical);
        }
        else
        {
            var model = PhysicalList.Last();
            if (model.CreateDate.Date >= DateTime.Now.Date)
            {
                Physical = model;
            }
            else
            {
                Physical = new PhysicalModel
                {
                    Weight = model.Weight,
                    Height = model.Height,
                    ActivityLevel = model.ActivityLevel,
                    ProteinPercentage = model.ProteinPercentage,
                    FatPercentage = model.FatPercentage,
                    CreateDate = DateTime.Now.Date
                };
                PhysicalList.Add(Physical);
            }
        }
    }

    partial void OnPhysicalListChanged(List<PhysicalModel>? oldValue, List<PhysicalModel> newValue)
    {
        if (IsOnlyUpdate(nameof(PhysicalList))) return;
        OnlyUpdatePhysicalList = (from p in PhysicalList orderby p.CreateDate select p)
            .ToList();
        InitPhysical();
        OnlyUpdatePhysicalList = (from p in PhysicalList orderby p.CreateDate select p)
            .ToList();

        PhysicalList.ForEach(p => p.Bind(this));
    }

    public Dictionary<DateTime, TdeeGroupModel> GetAllPhysicalModels(DateTime time)
    {
        int l = DateOnly.FromDateTime(time).DayNumber;

        int r = l + 6;
        var result = new Dictionary<DateTime, TdeeGroupModel>();

        // 先处理没有标点的情况
        if (PhysicalList.Count is 0)
        {
            for (int i = l; i <= r; i++)
            {
                var model = new TdeeGroupModel
                {
                    Ree = 0,
                    Tdee = 0,
                    MaxTdee = 0
                };
                result[i.ToDate()] = model;
            }

            return result;
        }

        if (PhysicalList.Count is 1)
        {
            var m = PhysicalList[0].TdeeGroup;
            for (int i = l; i <= r; i++)
            {
                var model = new TdeeGroupModel
                {
                    Ree = m.Ree,
                    Tdee = m.Tdee,
                    MaxTdee = m.MaxTdee
                };
                result[i.ToDate()] = model;
            }

            return result;
        }

        int pl = 0;
        var plx = PhysicalList[pl].DayNumber;
        var plt = PhysicalList[pl].TdeeGroup;
        if (l < plx)
        {
            if (r <= plx)
            {
                for (int i = l; i <= r; i++)
                {
                    var model = new TdeeGroupModel
                    {
                        Ree = plt.Ree,
                        Tdee = plt.Tdee,
                        MaxTdee = plt.MaxTdee
                    };
                    result[i.ToDate()] = model;
                }

                return result;
            }

            for (int i = l; i < plx; i++)
            {
                var model = new TdeeGroupModel
                {
                    Ree = plt.Ree,
                    Tdee = plt.Tdee,
                    MaxTdee = plt.MaxTdee
                };
                result[i.ToDate()] = model;
            }

            l = plx;
        }

        while (true)
        {
            if (l == r)
            {
                return result;
            }

            var p1 = PhysicalList[pl];
            var p2 = PhysicalList[pl + 1];
            double k1 = (p2.TdeeGroup.Ree - p1.TdeeGroup.Ree) / (p2.DayNumber - p1.DayNumber);
            double b1 = p1.TdeeGroup.Ree - k1 * p1.DayNumber;
            double k2 = (p2.TdeeGroup.Tdee - p1.TdeeGroup.Tdee) / (p2.DayNumber - p1.DayNumber);
            double b2 = p1.TdeeGroup.Tdee - k2 * p1.DayNumber;
            double k3 = (p2.TdeeGroup.MaxTdee - p1.TdeeGroup.MaxTdee) / (p2.DayNumber - p1.DayNumber);
            double b3 = p1.TdeeGroup.MaxTdee - k3 * p1.DayNumber;
            if (p2.DayNumber > r)
            {
                for (; l < r; l++)
                {
                    var model = new TdeeGroupModel
                    {
                        Ree = k1 * l + b1,
                        Tdee = k2 * l + b2,
                        MaxTdee = k3 * l + b3,
                    };
                    result[l.ToDate()] = model;
                }

                var model2 = new TdeeGroupModel
                {
                    Ree = k1 * r + b1,
                    Tdee = k2 * r + b2,
                    MaxTdee = k3 * r + b3
                };
                result[r.ToDate()] = model2;
            }
            else
            {
                for (; l < p2.DayNumber; l++)
                {
                    var model = new TdeeGroupModel
                    {
                        Ree = k1 * l + b1,
                        Tdee = k2 * l + b2,
                        MaxTdee = k3 * l + b3,
                    };
                    result[l.ToDate()] = model;
                }

                var model2 = new TdeeGroupModel
                {
                    Ree = k1 * p2.DayNumber + b1,
                    Tdee = k2 * p2.DayNumber + b2,
                    MaxTdee = k3 * p2.DayNumber + b3,
                };
                result[l.ToDate()] = model2;
                pl++;

                if (pl != PhysicalList.Count - 1) continue;

                for (int i = l; i <= r; i++)
                {
                    plt = PhysicalList[pl].TdeeGroup;
                    var model = new TdeeGroupModel
                    {
                        Ree = plt.Ree,
                        Tdee = plt.Tdee,
                        MaxTdee = plt.MaxTdee
                    };
                    result[i.ToDate()] = model;
                }

                return result;
            }
        }
    }
}