using System.Text.Json;
using AnyLibrary.Helper;

namespace WebServer.Helper;

public static class ObjectComparer
{
    public static bool CompareObjects(this object obj1, object? obj2, string arrayConfig)
    {
        if (obj2 is null) return false;

        var element1 = obj1 is string str1
            ? ParseStringToElement(str1)
            : ParseStringToElement(obj1.ToJson());

        var element2 = obj2 is string str2
            ? ParseStringToElement(str2)
            : ParseStringToElement(obj2.ToJson());

        var config = ParseArrayConfig(arrayConfig);
        return CompareJsonElements(element1, element2, config);
    }

    private static JsonElement ParseStringToElement(string jsonString)
    {
        using var doc = JsonDocument.Parse(jsonString);
        return doc.RootElement.Clone();
    }

    private static Dictionary<string, bool> ParseArrayConfig(string config)
    {
        var result = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        if (string.IsNullOrWhiteSpace(config)) return result;

        foreach (var part in config.Split(','))
        {
            var pair = part.Split(':');
            if (pair.Length != 2) continue;

            var key = pair[0].Trim();
            if (bool.TryParse(pair[1].Trim(), out bool value))
            {
                result[key] = value;
            }
        }

        return result;
    }

    private static bool CompareJsonElements(JsonElement a, JsonElement b, Dictionary<string, bool> arrayConfig,
        string currentField = null)
    {
        if (a.ValueKind != b.ValueKind) return false;
        switch (a.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var prop in b.EnumerateObject())
                {
                    if (!a.TryGetProperty(prop.Name, out JsonElement aValue)) return false;
                    if (!CompareJsonElements(aValue, prop.Value, arrayConfig, prop.Name)) return false;
                }

                return true;
            case JsonValueKind.Array:
                // 根据字段名获取数组比较方式
                bool ordered = arrayConfig.GetValueOrDefault(currentField, true);
                var aItems = a.EnumerateArray().ToList();
                var bItems = b.EnumerateArray().ToList();
                if (aItems.Count != bItems.Count) return false;

                if (ordered)
                {
                    // 顺序敏感比较
                    for (int i = 0; i < aItems.Count; i++)
                    {
                        if (!CompareJsonElements(aItems[i], bItems[i], arrayConfig, currentField)) return false;
                    }
                }
                else
                {
                    var bList = bItems.ToList();
                    foreach (var aItem in aItems)
                    {
                        bool found = false;
                        for (int i = 0; i < bList.Count; i++)
                        {
                            if (!CompareJsonElements(aItem, bList[i], arrayConfig, currentField))
                                continue;
                            bList.RemoveAt(i);
                            found = true;
                            break;
                        }

                        if (!found) return false;
                    }

                    // 检查所有b元素是否都被匹配
                    if (bList.Count != 0) return false;
                }

                return true;
            case JsonValueKind.String:
                return a.GetString() == b.GetString();
            case JsonValueKind.Number:
                // 精确比较数值的原始文本表示
                return a.GetRawText() == b.GetRawText();
            case JsonValueKind.True:
            case JsonValueKind.False:
                return a.GetBoolean() == b.GetBoolean();
            case JsonValueKind.Null:
                return true;
            default:
                return false;
        }
    }
}