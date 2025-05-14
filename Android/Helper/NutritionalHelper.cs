using Android.Models;
using Android.ViewModel;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;

namespace Android.Helper;

public static class NutritionalHelper
{
    /// <summary>
    /// 静息代谢
    /// </summary>
    public static double GetMifflinStJeorRee(double weight, double height, DateTime birthday, bool sex)
    {
        double age = birthday.GetAge();
        var ret = sex switch
        {
            true => 9.99 * weight + 6.25 * height - 4.92 * age + 5, //男
            false => 9.99 * weight + 6.25 * height - 4.92 * age - 161 //女
        };
        return Math.Max(0, ret);
    }

    /// <summary>
    /// 每日总消耗热量
    /// </summary>
    public static double TDEE(double ree, double activityFactor)
    {
        return ree * activityFactor; //静息代谢 * 活动水平
    }

    /// <summary>
    /// 计算BMI  来自《中国肥胖预防和控制蓝皮书》
    /// </summary>
    /// <param name="weight">体重 kg</param>
    /// <param name="height">身高 cm</param>
    public static double GetBmi(double weight, double height)
    {
        if (weight == 0 || height == 0) return 0;
        return weight * 10000 / (height * height);
    }

    /// <summary>
    /// 《中国肥胖预防和控制蓝皮书》
    /// </summary>
    public static string GetBmiString(double bmp) =>
        bmp switch
        {
            < 0 => "",
            < 18.5 => "消瘦",
            < 24 => "标准",
            < 28 => "肥胖前期",
            < 30 => "一级肥胖",
            < 40 => "二级肥胖",
            _ => "严重肥胖"
        };

    /// <summary>
    /// 《中国居民膳食营养素参考摄入量（2023版）》 中提供的个体膳食评价方法
    /// 原书说 无法确定个体实际需要量，也几乎不可能测定个体真正的日常摄入量。理论上只能运用统计学方法评估
    /// </summary>
    /// <returns>概率和相应判断</returns>
    public static string GetSd(decimal ear, decimal rni, decimal ul, int day, decimal ear0, string name)
    {
        string result = "";
        if (!AppConfigHelper.ModelConfig.NutrientConfig.SD.TryGetValue(name, out var a))
            return result;
        decimal sd3 = (decimal)a; //个体内标准差
        //对于有UL的情况，需要单独判断
        if (ul >= 0.0001m)
        {
            //Z分值 = (调查时期平均摄入量 - 每日摄入最高量) * 平均需要量标准差 / 个体内标准差
            var zUl = (ear0 - ul) * (decimal)Math.Sqrt(day) / sd3;
            var sdUl = AppConfigHelper.ModelConfig.GetSd(zUl); //通过数据库进行Z分值对应的判断
            result += sdUl != null ? $"{sdUl.A2 * 100:#}%概率 超出每日最高摄入量\n" : "";
        }

        if (ear > 0.001m)
        {
            var dEar = ear0 - ear; //差异 = 调查时期平均摄入量 - 平均需要量
            decimal sd2 = ear * 0.1m; //需要量标准差 = 平均需要量 * CV  CV在10%~15% 假设CV为10%
            var tmp = sd3 * sd3 / day + sd2 * sd2; //个体内标准差 ^ 2 / 调查的天数 + 需要量标准差 ^ 2
            var sd4 = (decimal)Math.Sqrt((double)tmp); //得到差异的标准差
            var zEar = dEar / sd4; //Z分值 = 差异 / 差异的标准差
            var sdEar = AppConfigHelper.ModelConfig.GetSd(zEar); //通过数据库进行Z分值对应的判断
            return sdEar != null ? $"{sdEar.A2 * 100:#}%概率 {sdEar.A1} " : "";
        }

        //对于无UL、无EAR的，直接放弃判断
        return result;
    }

    /// <summary>
    /// 获得对营养素含量的评价
    /// 主要依据  2008年01月11日《食品营养标签管理规范》问答  进行评判
    /// </summary>
    /// <param name="allNutritional">每100克可食部的营养素含量</param>
    /// <param name="ans">可食部量</param>
    public static List<Tuple<EvaluateTag, string>> GetEvaluateTag(Dictionary<string, decimal> allNutritional,
        decimal ans)
    {
        //先按比例计算出营养素
        var result = new List<Tuple<EvaluateTag, string>>();
        var dic = new Dictionary<string, Tuple<decimal, string>>();
        var nrv = new Dictionary<string, decimal>();
        foreach (var (key, value) in allNutritional)
        {
            double rni = 0;
            if (!AppConfigHelper.ModelConfig.TryGetNutritional(key, out var unit)) continue;
            if (!AppConfigHelper.ModelConfig.NutrientConfig.RNI?.TryGetValue(key, out rni) ??
                false) continue;
            //   *100/ans  每100g可食部中营养素的含量
            dic[key] = new Tuple<decimal, string>(value * 100 / ans, unit);
            // 文件中提到 NRV是以DRIs为依据制定的
            // DRIs用来指导“膳食”的而不是一个食品，而且其数值较多，应用起来比较复杂
            // 但是可以用这个概念对膳食的营养素进行评价，方便用户直观的了解
            // 所以这里直接使用根据用户提供的性别、年龄、体力活动水平等信息得到的DIRs的RNI作为 该营养素的营养素参考值
            // NRV = 食品中某营养素的含量 / 该营养素的营养素参考值 *100%
            if (rni != 0) nrv[key] = dic[key].Item1 * 100 / (decimal)rni;
        }

        if (dic.TryGetValue("热量 kcal", out var e))
        {
            var eKcal = e.Item1;
            var eKj = UnitHelper.ConvertUnit(e.Item1, e.Item2, "kJ");
            // 低能量 ≤ 170 kJ/100g 固体  ≤ 80 kJ/100ml 液体
            if (eKj <= 170) result.Add(new Tuple<EvaluateTag, string>(EvaluateTag.Positive, "低能量"));

            // 无或零能量 ≤ 17 kJ/100g（固体）或100ml（液体
            if (eKj <= 17) result.Add(new Tuple<EvaluateTag, string>(EvaluateTag.Positive, "无能量"));

            //（中国居民膳食指南） 高能量 400kcal/100g 以上
            if (eKcal >= 400) result.Add(new Tuple<EvaluateTag, string>(EvaluateTag.Negative, "高能量"));

            if (dic.TryGetValue("蛋白质", out var d))
            {
                //来自蛋白质的能量 kcal
                var dd = d.Item1 * 4m;
                // 低蛋白 来自蛋白质的能量 ≤总能量的5 %
                if (dd <= eKcal * 0.05m) result.Add(new Tuple<EvaluateTag, string>(EvaluateTag.Negative, "低蛋白"));
            }

            if (nrv.TryGetValue("蛋白质", out var ddd))
                switch (ddd)
                {
                    //蛋白质来源或含有蛋白质或提供蛋白质 每100 g的含量≥10 % NRV 每100 ml的含量 ≥5 % NRV 或者 每420 kJ的含量 ≥5 % NRV
                    case >= 10m:
                        result.Add(new Tuple<EvaluateTag, string>(EvaluateTag.Positive, "富含蛋白质"));
                        break;
                    //高或富含蛋白质或蛋白质丰富“来源”的两倍以上
                    case >= 5m:
                        result.Add(new Tuple<EvaluateTag, string>(EvaluateTag.Positive, "提供蛋白质"));
                        break;
                }
        }

        if (dic.TryGetValue("脂肪", out var z))
            switch (z.Item1)
            {
                //零，无或不含脂肪 ≤0.5 g/100g（固体）或100ml（液体） 
                case <= 0.5m:
                    result.Add(new Tuple<EvaluateTag, string>(EvaluateTag.Positive, "零脂肪"));
                    break;
                //低脂肪 ≤3 g/100g固体；≤1.5 g/100ml液体
                case <= 3m:
                    result.Add(new Tuple<EvaluateTag, string>(EvaluateTag.Positive, "低脂肪"));
                    break;
                case >= 20m:
                    result.Add(new Tuple<EvaluateTag, string>(EvaluateTag.Negative, "高脂肪"));
                    break;
            }

        var ans2 = dic.Where(d => d.Key.Contains("饱和脂肪") && d.Key.Contains("脂肪酸"))
            .Sum(d => UnitHelper.ConvertToBaseUnit(d.Value.Item1, d.Value.Item2));
        switch (ans2)
        {
            //零，无或不含饱和脂肪 ≤0.1 g/ 100g（固体）或100ml（液体）
            case <= 0.1m:
                result.Add(new Tuple<EvaluateTag, string>(EvaluateTag.Positive, "不含饱和脂肪"));
                break;
            //低饱和脂肪 ≤1.5 g/100g固体 ≤0.75 g /100mL液体
            case <= 1.5m:
                result.Add(new Tuple<EvaluateTag, string>(EvaluateTag.Positive, "低饱和脂肪"));
                break;
            case >= 5m:
                result.Add(new Tuple<EvaluateTag, string>(EvaluateTag.Negative, "高饱和脂肪"));
                break;
        }

        if (dic.TryGetValue("胆固醇", out var d2))
            switch (UnitHelper.ConvertUnit(d2.Item1, d2.Item2, "mg"))
            {
                //无、或不含、零胆固醇 ≤0.005 g/100g（固体）或100ml（液体）
                case <= 0.005m:
                    result.Add(new Tuple<EvaluateTag, string>(EvaluateTag.Positive, "零胆固醇"));
                    break;
                //低胆固醇 ≤20m g /100g固体； ≤10mg /100ml液体。
                case <= 20m:
                    result.Add(new Tuple<EvaluateTag, string>(EvaluateTag.Positive, "低胆固醇"));
                    break;
                case >= 100m:
                    result.Add(new Tuple<EvaluateTag, string>(EvaluateTag.Negative, "高胆固醇"));
                    break;
            }

        if (dic.TryGetValue("碳水化合物", out var d3))
            switch (UnitHelper.ConvertUnit(d3.Item1, d3.Item2, "g"))
            {
                //无或不含碳水化合物 ≤ 0.5 g /100g（固体）或100ml（液体）
                case <= 0.5m:
                    result.Add(new Tuple<EvaluateTag, string>(EvaluateTag.Positive, "不含碳水化合物"));
                    break;
                //低碳水化合物 ≤ 5 g /100g（固体）或100ml（液体）
                case <= 5m:
                    result.Add(new Tuple<EvaluateTag, string>(EvaluateTag.Positive, "低碳水化合物"));
                    break;
                ////（中国居民膳食指南） 高糖 400kcal/100g 以上
            }

        if (dic.TryGetValue("钠", out var d4))
            switch (UnitHelper.ConvertUnit(d4.Item1, d4.Item2, "mg"))
            {
                //无、或不含、零钠 ≤5 mg /100g（固体）或100ml（液体）
                case <= 5m:
                    result.Add(new Tuple<EvaluateTag, string>(EvaluateTag.Positive, "零钠"));
                    break;
                //极低钠 ≤40 mg /100g或100ml
                case <= 40m:
                    result.Add(new Tuple<EvaluateTag, string>(EvaluateTag.Positive, "极低钠"));
                    break;
                //低钠 ≤120 mg /100g或100ml
                case <= 120m:
                    result.Add(new Tuple<EvaluateTag, string>(EvaluateTag.Positive, "低钠"));
                    break;
                ////（中国居民膳食指南） 高盐 800/100g 以上
                case >= 800m:
                    result.Add(new Tuple<EvaluateTag, string>(EvaluateTag.Negative, "高钠"));
                    break;
            }


        var list = new List<string> { "钙", "磷", "镁", "铁", "锌", "铜", "锰", "碘", "硒", "钼", "铬", "氟" };
        var list2 = nrv.Where(d => list.Contains(d.Key) && d.Value >= 15m)
            .ToList();
        foreach (var (key, value) in list2)
            switch (value)
            {
                //高或富含xx或xx 的良好来源 “来源”的两倍以上 
                case >= 30m:
                    result.Add(new Tuple<EvaluateTag, string>(EvaluateTag.Positive, $"富含 {key}"));
                    break;
                // 矿物质 xx来源 或含有xx 或提供xx 每100 g中 ≥15% NRV 每100 ml中 ≥7.5% NRV  或者 每420 kJ中 ≥5% NRV
                case >= 15m:
                    result.Add(new Tuple<EvaluateTag, string>(EvaluateTag.Positive, $"含有 {key}"));
                    break;
            }

        var list3 = nrv.Where(d => d.Key.Contains("维生素") && d.Value >= 15m)
            .ToList();
        foreach (var (key, value) in list3)
            switch (value)
            {
                case >= 30m:
                    result.Add(new Tuple<EvaluateTag, string>(EvaluateTag.Positive, $"富含 {key}"));
                    break;
                case >= 15m:
                    result.Add(new Tuple<EvaluateTag, string>(EvaluateTag.Positive, $"含有 {key}"));
                    break;
            }

        //多维 添加3种以上的维生素 含量符合上述相应来源的含量要求
        if (list3.Count > 3) result.Add(new Tuple<EvaluateTag, string>(EvaluateTag.Positive, "多维"));

        //膳食纤维来源或含有膳食纤维   ≥3 g/ 100g，  ≥1.5 g/ 100ml 
        //高或富含膳食纤维或良好来源 “来源”的两倍以上
        if (dic.TryGetValue("膳食纤维", out var b))
        {
            var value = UnitHelper.ConvertToBaseUnit(b.Item1, b.Item2);
            switch (value)
            {
                case >= 6m:
                    result.Add(new Tuple<EvaluateTag, string>(EvaluateTag.Positive, "富含膳食纤维"));
                    break;
                case >= 3m:
                    result.Add(new Tuple<EvaluateTag, string>(EvaluateTag.Positive, "含有膳食纤维"));
                    break;
            }
        }

        if (e is not null)
        {
            dic.TryGetValue("碳水化合物", out var aaa);
            dic.TryGetValue("脂肪", out var bbb);
            dic.TryGetValue("蛋白质", out var ccc);

            var ans4 = UnitHelper.ConvertUnit(aaa?.Item1 ?? 0m, aaa?.Item2 ?? "g", "g") +
                       UnitHelper.ConvertUnit(bbb?.Item1 ?? 0m, bbb?.Item2 ?? "g", "g") +
                       UnitHelper.ConvertUnit(ccc?.Item1 ?? 0m, ccc?.Item2 ?? "g", "g");
            if (ans4 <= 1) result.Add(new Tuple<EvaluateTag, string>(EvaluateTag.Negative, "纯热量食物"));
        }

        return result;
    }

    public static Dictionary<EvaluateTag, List<string>> ToDictionary(List<Tuple<EvaluateTag, string>> list)
    {
        var ret = new Dictionary<EvaluateTag, List<string>>();
        foreach (var (item1, item2) in list)
        {
            if (!ret.ContainsKey(item1)) ret[item1] = new List<string>();

            ret[item1].Add(item2);
        }

        return ret;
    }

    /// <summary>
    ///     生成模型
    /// </summary>
    /// <param name="dic">所有营养素</param>
    /// <param name="energyNutrient">千焦 千卡</param>
    /// <param name="proteinNutrient">三大营养素</param>
    /// <param name="otherNutrient">其他营养素</param>
    public static void InitAllNutritional(
        Dictionary<string, decimal> dic,
        out List<Tuple<string, decimal>> energyNutrient,
        out List<NutrientContentViewModel> proteinNutrient,
        out List<NutrientContentModel> otherNutrient
    )
    {
        otherNutrient = [];

        foreach (var (key, value) in dic)
        {
            if (key is "蛋白质" or "碳水化合物" or "脂肪" or "热量 kcal") continue;
            if (!AppConfigHelper.ModelConfig.TryGetNutritional(key, out var unit)) continue;
            var baseUnit = UnitHelper.GetBaseUnit(unit);
            var baseValue = UnitHelper.ConvertUnit(value, unit, baseUnit);
            var model = new NutrientContentModel
            {
                Name = key,
                Value = baseValue,
                Unit = baseUnit,
                Color = key.Contains("脂肪酸") || key.Contains("饱和脂肪")
                    ? ColorHelper.Blue
                    : ColorHelper.RandomColor
            };
            otherNutrient.Add(model);
        }

        var sum = otherNutrient.Sum(x => x.Value);
        otherNutrient.ForEach(i => i.Rate = (double)(i.Value / sum * 100));

        otherNutrient.Sort((l, r) => r.Value.CompareTo(l.Value));
        otherNutrient.ForEach(i =>
        {
            UnitHelper.ConvertToClosestUnit(i.Value, i.Unit, out var unit, out var value);
            i.Value = unit;
            i.Unit = value;
        });

        //dic 单位为g或者ml
        if (dic.TryGetValue("热量 kcal", out var kcal))
        {
            energyNutrient =
            [
                new Tuple<string, decimal>("热量 kcal", kcal),
                new Tuple<string, decimal>("热量 kJ", UnitHelper.ConvertUnit(kcal, "kcal", "kJ"))
            ];
            energyNutrient.Sort((l, r) =>
                string.Compare(r.Item1, l.Item1, StringComparison.Ordinal));
        }
        else
        {
            energyNutrient = [];
        }

        proteinNutrient = InitNutritional(dic);
    }

    public static List<NutrientContentViewModel> InitNutritional(Dictionary<string, decimal> nutritional,
        bool isFire = false, bool isNrv = false)
    {
        nutritional.TryGetValue("热量 kcal", out var ev);
        nutritional.TryGetValue("碳水化合物", out var cv);
        nutritional.TryGetValue("蛋白质", out var pv);
        nutritional.TryGetValue("脂肪", out var fv);

        if (cv == 0 && pv == 0 && fv == 0) return [];
        var physical = AppConfigHelper.MyInfo.Physical;
        var result = new List<NutrientContentViewModel>
        {
            new()
            {
                Max = (decimal)physical.CarbohydrateRequirement,
                Pos = cv,
                Icon = Drawable.ic_carbohydrate,
                Colors = ColorHelper.Blue
            },
            new()
            {
                Max = (decimal)physical.ProteinRequirement,
                Pos = pv,
                Icon = Drawable.ic_protein,
                Colors = ColorHelper.Purple
            },
            new()
            {
                Max = (decimal)physical.FatRequirement,
                Pos = fv,
                Icon = Drawable.ic_fat,
                Colors = ColorHelper.Orange
            }
        };
        if (isFire)
            result.Insert(0, new()
            {
                Max = (decimal)physical.TdeeGroup.Tdee,
                Pos = ev,
                Icon = Drawable.ic_fire,
                Colors = ColorHelper.OrangeRed
            });

        foreach (var model in result)
            model.Str1 = $"{model.Rate:0.##} NRV%";
        UnitHelper.ConvertToClosestUnit(ev, "kcal", out var e1, out var eu);
        UnitHelper.ConvertToClosestUnit(cv, "g", out var c1, out var cu);
        UnitHelper.ConvertToClosestUnit(pv, "g", out var p1, out var pu);
        UnitHelper.ConvertToClosestUnit(fv, "g", out var f1, out var fu);

        {
            int i = 0;
            if (isFire)
                result[i++].Str2 = $"能量 {e1:0.##} {eu}";
            result[i++].Str2 = $"碳水 {c1:0.##} {cu}";
            result[i++].Str2 = $"蛋白质 {p1:0.##} {pu}";
            result[i].Str2 = $"脂肪 {f1:0.##} {fu}";
        }

        if (!isNrv)
            foreach (var model in result.Where(model => model.Pos > model.Max))
                model.Str1 = model.Str2;

        return result;
    }
}