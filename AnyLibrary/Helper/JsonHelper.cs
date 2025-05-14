using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AnyLibrary.Helper;

public static class JsonHelper
{
    private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    public static string ToJson(this object data)
    {
        return JsonConvert.SerializeObject(data, Settings);
    }

    public static T ToEntity<T>(this object? json) =>
        JsonConvert.DeserializeObject<T>(json.ToString()) ??
        throw new FileLoadException("读取json失败！");
}