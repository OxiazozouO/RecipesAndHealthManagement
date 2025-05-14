using AnyLibrary.Helper;
using Microsoft.AspNetCore.Mvc;
using WebServer.Helper;
using WebServer.Http;
using static System.IO.File;

namespace WebServer.Controllers.Any;

[Route("api/[controller]/[action]")]
[ApiController]
public class FileController : ControllerBase
{
    [HttpGet("files/{path}/{fileName}")]
    public IActionResult GetImageFile(string path, string fileName)
    {
        try
        {
            var filePath = Path.Combine(FileUrlHelper.OldFilePath, path, fileName);

            if (!Exists(filePath)) return ApiResponses.ErrorResult;

            var fileStream = OpenRead(filePath);
            byte[] buffer = new byte[4];
            fileStream.Read(buffer, 0, buffer.Length);
            fileStream.Seek(0, SeekOrigin.Begin);
            string contentType = "*";

            foreach (var (key, value) in headers)
                if (buffer.Take(value.Length).SequenceEqual(value))
                {
                    contentType = key;
                    break;
                }

            contentType = $"image/{contentType}";

            return File(fileStream, contentType);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    public Task<IActionResult> Upload(IFormFile file)
    {
        try
        {
            if (file == null)
                return Task.FromResult<IActionResult>(ApiResponses.Error("文件为空"));

            if (file.Length > 1024 * 1024 * 10)
                return Task.FromResult<IActionResult>(ApiResponses.Error("文件大小超过10MB"));

            if (file.ContentType.Contains("application/octet-stream") || file.ContentType.Contains("image"))
            {
                var newFileName = StringHelper.GetRandomString();
                var newFilePath = Path.Combine(FileUrlHelper.OldFilePath,
                    FileUrlHelper.Temps, newFileName);

                using var stream = new FileStream(newFilePath, FileMode.Create);
                file.CopyTo(stream);

                return Task.FromResult<IActionResult>(ApiResponses.Success("上传成功", new
                {
                    NewFileName = newFileName,
                    Url = Url.GetTmpUrl(Request, newFileName)
                }));
            }

            return Task.FromResult<IActionResult>(ApiResponses.Error("只允许上传图片"));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return Task.FromResult<IActionResult>(ApiResponses.Error("上传失败"));
    }

    private Dictionary<string, byte[]> headers = new()
    {
        { "jpg", [0xFF, 0xD8] },
        { "png", [0x89, 0x50, 0x4E, 0x47] },
        { "gif", [0x47, 0x49, 0x46] },
        { "bmp", [0x42, 0x4D] },
        { "tiff", [0x49, 0x49, 0x2A, 0x00] }
    };
}