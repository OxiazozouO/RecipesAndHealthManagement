using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebServer.Controllers.Any;
using WebServer.Controllers.Service;
using WebServer.DatabaseModel;
using WebServer.Helper;
using WebServer.Http;

namespace WebServer.Controllers.User;

[Route("user/[controller]/[action]")]
[ApiController]
public class UserConfigController(RecipeAndHealthSystemContext db, JwtHelper jwt) : MyControllerBase(db, jwt)
{
    [HttpPost]
    [Authorize(Roles = nameof(JwtType.User))]
    public IActionResult GetConfigs(ConfigDto dto)
    {
        try
        {
            if (this.CheckUserRole(dto.Id.ToString(), out var res, out var user))
                return res;

            var arr = dto.Key.Split(',');
            var list = Db.SystemConfigs.Where(c => c.ConfigId < 100 && arr.Contains(c.ConfigName)).ToList();
            var result = new Dictionary<string, object?>();
            foreach (var cfg in list)
                result[cfg.ConfigName] = cfg.ConfigValue;
            if (arr.Contains("Nutrients"))
            {
                result["Nutrients"] = Db.Nutrients.Select(n => new
                {
                    n.Id,
                    n.Name,
                    n.Unit,
                    n.Refer
                }).ToList();
            }


            if (arr.Contains("Cals"))
            {
                result["Cals"] = Db.ConfigActivityLevels.Select(c => new
                {
                    c.Id,
                    Name = c.Key,
                    c.Value
                }).ToList();
            }

            if (arr.Contains("ReferenceIntakeOfNutrients"))
            {
                result["ReferenceIntakeOfNutrients"] = NutrientContentService.get(db);
            }

            return ApiResponses.Success("", result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }
}

public record ConfigDto
{
    [Range(1, int.MaxValue - 2, ErrorMessage = "请求错误")]
    public int Id { get; set; }

    [Required(ErrorMessage = "配置名必填")] public string Key { get; set; }
}