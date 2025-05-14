using Microsoft.AspNetCore.Mvc;
using WebServer.DatabaseModel;

namespace WebServer.Http;

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

    //成功
    public static OkObjectResult Success(string message = "success", object? data = null) =>
        new(new ApiResponses { Code = 1, Message = message, Data = data });

    public static OkObjectResult Error(string responseMessage) =>
        new(new ApiResponses { Code = 400, Message = responseMessage });

    public static OkObjectResult ErrorResult => Error("服务器忙，请稍等");

    //权限不足
    public static OkObjectResult NoPermission => new(new ApiResponses { Code = 403, Message = "权限不足" });

    public static OkObjectResult Auto(RecipeAndHealthSystemContext db,
        string success = "", string error = "", int ans = 1)
    {
        return db.SaveChanges() == ans ? Success(success) : Error(error);
    }
}