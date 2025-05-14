using Newtonsoft.Json;

namespace Android.JsonConverter;

public class StringToRequiredIngredientJsonConverter : Newtonsoft.Json.JsonConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is not Tuple<List<int>, List<int>, List<int>> t)
        {
            writer.WriteNull();
            return;
        }

        var tag = 1;
        for (var i = 0; i < t.Item1.Count; i++)
            if (t.Item2[i] != t.Item1[i])
                tag = 0;

        if (tag == 1)
            foreach (var ti in t.Item1)
                t.Item2.Remove(ti);

        var jsonString = $"{string.Join('.', t.Item2)}|{string.Join('.', t.Item1)}|{string.Join('.', t.Item3)}";
        writer.WriteValue(jsonString);
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue,
        JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null) return null;

        var jsonString = reader.Value as string;
        if (string.IsNullOrEmpty(jsonString)) return null;

        var arr = jsonString.Split('|');
        var arr0 = arr[0].Split('.').Where(s => !string.IsNullOrEmpty(s)).Select(int.Parse).ToList();
        var arr1 = arr[1].Split('.').Where(s => !string.IsNullOrEmpty(s)).Select(int.Parse).ToList();
        var arr2 = arr[2].Split('.').Where(s => !string.IsNullOrEmpty(s)).Select(int.Parse).ToList();
        arr1.InsertRange(0, arr0);

        return new Tuple<List<int>, List<int>, List<int>>(arr0, arr1, arr2);
    }

    public override bool CanConvert(Type objectType)
    {
        return true;
    }
}