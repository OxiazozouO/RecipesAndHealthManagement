using Android.Attribute;
using Android.Helper;
using Android.Models;
using Newtonsoft.Json;

namespace Android.Configurations;

public class ModelConfig
{
    public List<NutrientModel>? Nutrients { set; get; }

    [JsonConverter(typeof(StringToObjectJsonConverter))]
    public Dictionary<string, decimal> Units { set; get; }

    [JsonConverter(typeof(StringToObjectJsonConverter))]
    public Dictionary<string, string> BaseUnit { set; get; }

    [JsonConverter(typeof(StringToObjectJsonConverter))]
    public Dictionary<string, string> UnitNext { set; get; }

    [JsonConverter(typeof(StringToObjectJsonConverter))]
    public Dictionary<string, string> UnitPre { set; get; }

    [JsonConverter(typeof(StringToObjectJsonConverter))]
    public Dictionary<string, SDClass> SDs { set; get; }

    public SDClass? GetSd(decimal value)
    {
        foreach (var (key, sdClass) in SDs)
        {
            var arr = key.Split("~");
            var l = (decimal)double.Parse(arr[0]);
            var r = (decimal)double.Parse(arr[1]);
            if (value >= l && value <= r)
            {
                return sdClass;
            }
        }

        return null;
    }

    public class SDClass
    {
        public string A1;
        public double A2;
    }

    [JsonConverter(typeof(StringToObjectJsonConverter))]
    public Dictionary<string, Dictionary<string, string>> UnitLocal { set; get; }

    public List<ActivityLevelModel> Cals { set; get; }
    public Dictionary<string, ActivityLevelModel> ActivityLevels { set; get; }

    [JsonConverter(typeof(StringToObjectJsonConverter))]
    public Dictionary<bool, byte[]> ProteinRequirement { set; get; }

    public Dictionary<int, Dictionary<string, Dictionary<string, Dictionary<string, double>>>>?
        ReferenceIntakeOfNutrients { set; get; }

    public void Init()
    {
        if (Cals is not null)
            ActivityLevels = Cals.ToDictionary(kv => kv.Name, kv => kv);
    }

    public void InitNutritional(bool gender, int age)
    {
        try
        {
            var dic = ReferenceIntakeOfNutrients;
            if (dic is null) return;
            if (!dic.TryGetValue(gender ? 1 : 0, out var dic2)) return;

            NutrientConfig = new NutrientConfig
            {
                EAR = Update(dic2, "EAR", age),
                RNI = Update(dic2, "RNI", age),
                UL = Update(dic2, "UL", age),
                CV = Update(dic2, "CV", age),
                SD = Update(dic2, "SD^h", age)
            };
        }
        catch (Exception e)
        {
            MsgBoxHelper.Builder().TryError("初始化失败！\n" + e.Message);
        }
    }

    private static Dictionary<string, double> Update(
        Dictionary<string, Dictionary<string, Dictionary<string, double>>> dic, string key,
        int age)
    {
        if (!dic.TryGetValue(key, out var a)) return [];
        foreach (var (key2, value) in a)
        {
            var arr = key2.Split("-");
            var l = int.Parse(arr[0]);
            var r = int.Parse(arr[1]);
            if (age >= l && age <= r)
            {
                return value;
            }
        }

        return [];
    }

    public NutrientConfig NutrientConfig;

    public bool TryGetNutritional(string name, out string? value)
    {
        value = null;
        var v = Nutrients?.FirstOrDefault(n => n.Name.Contains(name));
        if (v == null) return false;
        value = v.Unit;
        return true;
    }
}