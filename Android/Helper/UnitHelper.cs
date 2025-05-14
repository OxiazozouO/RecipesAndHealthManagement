using static Android.Helper.AppConfigHelper;

namespace Android.Helper;

public static class UnitHelper
{
    public static decimal ConvertUnit(decimal value, string input, string output)
    {
        if (input == output) return value;
        if (ModelConfig.Units.TryGetValue(input, out var i) &&
            ModelConfig.Units.TryGetValue(output, out var o))
            return value * i / o;

        throw new ArgumentException();
    }

    public static decimal ConvertBaseUnitTo(decimal value, string input)
    {
        return ConvertUnit(value, GetBaseUnit(input), input);
    }

    public static decimal ConvertToBaseUnit(decimal value, string input)
    {
        return ConvertUnit(value, input, GetBaseUnit(input));
    }

    public static void ConvertToClosestUnit(decimal value, string input, out decimal result, out string output)
    {
        if (ModelConfig.Units.TryGetValue(input, out var i))
        {
            if (ModelConfig.UnitNext.TryGetValue(input, out var a) && a is not null &&
                ModelConfig.Units.TryGetValue(a, out var ma) &&
                input != a && Math.Abs(value) >= Math.Abs(ma / i))
            {
                value = value * i / ma;
                input = a;
                ConvertToClosestUnit(value, input, out result, out output);
                return;
            }

            if (ModelConfig.UnitPre.TryGetValue(input, out var b) && b is not null &&
                ModelConfig.Units.TryGetValue(b, out var mi) && input != b && Math.Abs(value) < 1)
            {
                value = value * i / mi;
                input = b;
                ConvertToClosestUnit(value, input, out result, out output);
                return;
            }
        }

        result = value;
        output = input;
    }

    public static string GetBaseUnit(string input)
    {
        if (ModelConfig.BaseUnit.TryGetValue(input, out var ret))
            return ret ?? input;

        throw new ArgumentException();
    }

    public static List<string> GetAllUnit(string input)
    {
        var list = new List<string>();
        var dic = new Dictionary<string, decimal>();
        list.Add(input);
        run(ModelConfig.UnitNext);
        run(ModelConfig.UnitPre);
        foreach (var se in list)
        {
            if (!ModelConfig.Units.TryGetValue(se, out var v) && v > 0) continue;
            dic[se] = v;
        }

        return dic
            .OrderBy(x => x.Value)
            .Select(x => x.Key)
            .ToList();

        void run(Dictionary<string, string> dir)
        {
            string? text = null;
            while (true)
            {
                if (dir.TryGetValue(input, out text) && text != null)
                {
                    list.Add(text);
                    input = text;
                    continue;
                }

                break;
            }
        }
    }
}