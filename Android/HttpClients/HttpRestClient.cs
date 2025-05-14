using Android.Content;
using Android.Helper;
using AnyLibrary.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using static System.Net.HttpStatusCode;

namespace Android.HttpClients;

public static class HttpRestClient
{
    public static void Execute(this ApiRequest apiRequest, Action<ApiResponses>? action = null,
        Action<string>? error = null)
    {
        action ??= MsgBoxHelper.ShowDialog;
        error ??= MsgBoxHelper.Error;
        if (Execute(apiRequest, out ApiResponses ret))
        {
            action.Invoke(ret);
        }
        else
        {
            error.Invoke(ret.Message);
        }
    }

    private static readonly RestClient _client = new("http://10.0.2.2:5256/user/");
    private static readonly RestClient _fileClient = new("http://10.0.2.2:5256/api/");

    public static bool Execute(this ApiRequest apiRequest, out ApiResponses ret)
    {
        ret = null;
        var request = new RestRequest("User" + apiRequest.Url, apiRequest.Method);
        request.AddHeader("accept", "*/*");
        if (apiRequest.IsJwt)
        {
            var jwt = AppConfigHelper.AppConfig.Jwt;
            if (jwt is null)
            {
                MsgBoxHelper.Builder("请先登录").OkCancel(ActivityHelper.GotoLogin);
                return false;
            }

            request.AddHeader("Authorization", jwt);
        }

        request.AddHeader("Content-Type", apiRequest.ContentType);
        if (apiRequest.Parameters is not null)
        {
            var json = JsonConvert.SerializeObject(apiRequest.Parameters);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
        }

        var res = _client.Execute(request);
        ret = res.StatusCode switch
        {
            OK => res.Content.ToEntity<ApiResponses>(),
            _ => ApiResponses.Error(res.StatusCode switch
            {
                Continue => "服务器正在处理请求，请稍候再试",
                SwitchingProtocols => "服务器正在处理请求，请稍候再试",
                Processing => "服务器正在处理请求，请稍候再试",
                EarlyHints => "服务器正在处理请求，请稍候再试",
                Created => "请求已被接受但尚未完成处理",
                Accepted => "请求已被接受但尚未完成处理",
                NonAuthoritativeInformation => "返回的信息可能不是最新的",
                BadRequest => "客户端请求错误：\n" + res.Content.ToEntity<JObject>()["errors"],
                Unauthorized => "未授权的请求",
                InternalServerError => "服务器内部错误",
                NotImplemented => "请求的功能尚未实现",
                BadGateway => "网关错误",
                _ => "服务器忙"
            }, (int)res.StatusCode)
        };

        switch (ret.Code)
        {
            case 0:
                MsgBoxHelper.Builder().TryError(ret.Message);
                break;
            case 403 or 401:
                MsgBoxHelper.Builder("请重新登录", "登录超时")
                    .OkCancel(ActivityHelper.GotoLogin, ActivityHelper.GotoLogin);
                break;
        }

        return ret.Code == 1;
    }

    public static bool FileUpload(this ContentResolver? resolver, Android.Net.Uri? uri, out string outFileName) =>
        FileUpload(resolver, uri, out outFileName, out _);

    public static bool FileUpload(this ContentResolver? resolver, Android.Net.Uri? uri, out string outFileName,
        out string url)
    {
        outFileName = "";
        url = "";
        if (uri == null || resolver == null) return false;
        using var stream = resolver.OpenInputStream(uri);
        if (stream == null) return false;
        byte[] data = new byte[stream.Length];
        stream.Read(data, 0, (int)stream.Length);

        if (data.Length > 1024 * 1024 * 10) MsgBoxHelper.Builder().TryError("文件大小超过10MB");

        var request = new RestRequest("File/Upload", Method.POST);
        request.AddHeader("Content-Type", "multipart/form-data");
        try
        {
            request.AddFile("file", data, "114514");

            var res = _fileClient.Execute(request);

            if (res.StatusCode == OK)
            {
                var json = res.Content.ToEntity<ApiResponses>().Data?.ToString() ?? "";
                var dto = json.ToEntity<FileNameDto>();
                outFileName = dto.NewFileName;
                url = dto.Url.Replace("localhost", "10.0.2.2");
                if (!string.IsNullOrEmpty(outFileName)) return true;
                MsgBoxHelper.Builder().TryError("上传失败");
                return false;
            }

            MsgBoxHelper.Builder().TryError($"上传失败：{res.ErrorMessage}");
            return false;
        }
        catch (Exception ex)
        {
            MsgBoxHelper.Builder().TryError($"发生错误：{ex.Message}");
            return false;
        }
    }

    public record FileNameDto
    {
        public string NewFileName { set; get; }
        public string Url { set; get; }
    }
}