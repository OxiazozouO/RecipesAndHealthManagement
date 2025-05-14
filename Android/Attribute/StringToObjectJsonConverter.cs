using Newtonsoft.Json;

namespace Android.Attribute;

public class StringToObjectJsonConverter : Newtonsoft.Json.JsonConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
        }
        else
        {
            var jsonString = JsonConvert.SerializeObject(value);
            writer.WriteValue(jsonString);
        }
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue,
        JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null) return null;

        var jsonString = reader.Value as string;
        if (string.IsNullOrEmpty(jsonString)) return null;

        return JsonConvert.DeserializeObject(jsonString, objectType);
    }

    public override bool CanConvert(Type objectType)
    {
        return true;
    }
}