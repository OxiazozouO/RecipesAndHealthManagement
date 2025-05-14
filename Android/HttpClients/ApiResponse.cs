namespace Android.HttpClients;

/// <summary>
///     响应模型
/// </summary>
public class ApiResponses
{
    /// <summary>
    ///     结果编码
    /// </summary>
    public int Code { get; set; }

    /// <summary>
    ///     结果消息
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    ///     结果数据
    /// </summary>
    public object? Data { get; set; }

    public static ApiResponses ErrorResult => new() { Code = 400, Message = "服务器忙，请稍等" };

    public static ApiResponses Error(string responseMessage, int code = 400)
    {
        return new ApiResponses { Code = code, Message = responseMessage };
    }
}