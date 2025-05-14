using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebServer.Configurations;
using WebServer.Controllers.Any;
using WebServer.Controllers.Service;
using WebServer.DatabaseModel;
using WebServer.DTOs;
using WebServer.Helper;
using WebServer.Http;

namespace WebServer.Controllers.Admin;

[Route("admin/[controller]/[action]")]
[ApiController]
public class AdminConfigController(RecipeAndHealthSystemContext db, JwtHelper jwt) : MyControllerBase(db, jwt)
{
    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult GetConfigs(AdminDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;
            var result = (from s in Db.SystemConfigs
                where s.ConfigId >= 100
                select new
                {
                    Id = s.ConfigId,
                    Name = s.ConfigName,
                    Value = s.ConfigValue,
                    Refer = s.ConfigRefer,
                    CreateDate = s.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    UpdateDate = s.UpdateDate.ToString("yyyy-MM-dd HH:mm:ss")
                }).ToList();

            return ApiResponses.Success("", result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult EditConfig(EditConfigDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;
            var systemConfig = Db.SystemConfigs.FirstOrDefault(s => s.ConfigId == dto.Id);
            if (systemConfig is null)
                return ApiResponses.Error("没有找到相关数据");
            switch (dto.Name)
            {
                case "value":
                    systemConfig.ConfigValue = dto.Value;
                    break;
                case "refer":
                    systemConfig.ConfigRefer = dto.Value;
                    break;
                default:
                    return ApiResponses.Error($"暂时不支持修改{dto.Name}字段");
            }

            systemConfig.UpdateDate = DateTime.Now;
            Db.SystemConfigs.Update(systemConfig);
            bool b = Db.SaveChanges() == 1;
            if (b)
            {
                AppSettings.LoadByDb(new Dictionary<string, string>
                    { [systemConfig.ConfigName] = systemConfig.ConfigValue });
            }

            return b
                ? ApiResponses.Success("修改成功")
                : ApiResponses.Error("修改失败");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult Reset(AdminDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;
            AppSettings.Reset();
            var list = Db.SystemConfigs
                .Where(s => AppSettings.Prefixes.Any(prefix => s.ConfigName.StartsWith(prefix)))
                .ToList();
            AppSettings.SaveToDb(list);
            Db.SystemConfigs.UpdateRange(list);
            Db.SaveChanges();
            return ApiResponses.Success("重置成功");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }


    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult GetBackupInfo(AdminDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;
            var files = FileService.GetFileInfo(db);
            return ApiResponses.Success("请求成功", files);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult Backup(AdminDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;
            var dump = RunProcess("mysqldump", string.Format(
                """
                --defaults-file="{0}" -u{1} -h 127.0.0.1 {2} --result-file="{3}\{4}"
                """,
                Path.GetFullPath(".my.cnf"),
                AppSettings.BackupsConfig.DbUser,
                AppSettings.BackupsConfig.DbName,
                Path.GetFullPath(Path.Combine(AppSettings.FileUrlConfig.OldFilePath,
                    AppSettings.BackupsConfig.BackupPath)),
                $"{DateTime.Now:yyyy-MM-dd HH-mm-ss}.sql"
            ));
            if (dump is null) return ApiResponses.Success("备份成功");
            return ApiResponses.Success("备份失败" + dump);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }


    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult Restore(RestoreDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;
            var restore = RunProcess("mysql", string.Format(
                """
                --defaults-file="{0}" -u{1} {2} -e "source {3}"
                """,
                Path.GetFullPath(".my.cnf"),
                AppSettings.BackupsConfig.DbUser,
                AppSettings.BackupsConfig.DbName,
                Path.GetFullPath(Path.Combine(AppSettings.FileUrlConfig.OldFilePath,
                    AppSettings.BackupsConfig.BackupPath, dto.Name))
            ));
            if (restore is null) return ApiResponses.Success("还原成功");
            return ApiResponses.Success("还原失败" + restore);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult DeleteSqlFile(DeleteSqlFileDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;
            var path = Path.GetFullPath(Path.Combine(FileUrlHelper.OldFilePath, AppSettings.BackupsConfig.BackupPath,
                dto.Name));
            if (!System.IO.File.Exists(path))
                return ApiResponses.Error("文件不存在");
            System.IO.File.Delete(path);
            return ApiResponses.Success("删除成功");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult DeleteUselessFiles(AdminDto dto)
    {
        try
        {
            if (this.CheckAdminRole(dto.AdminId, out var responses, out var admin))
                return responses;

            var notUsed = FileService.GetNotUsed(db);
            foreach (var re in notUsed)
            {
                System.IO.File.Delete(re);
            }

            return ApiResponses.Success($"已清理{notUsed.Count}文件！");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.ErrorResult;
    }

    private static string? RunProcess(string run, string cmd)
    {
        try
        {
            using var process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                FileName = Path.GetFullPath(Path.Combine(AppSettings.BackupsConfig.MySQLPath, run + ".exe")),
                Arguments = cmd,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };
            Console.WriteLine(process.StartInfo.FileName);
            Console.WriteLine(process.StartInfo.Arguments);
            // 启动进程
            process.Start();

            string error = process.StandardError.ReadToEnd();

            // 等待进程结束
            process.WaitForExit();
            return error;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        return null;
    }
}

public record EditConfigDto : AdminUserDto
{
    [Required(ErrorMessage = "请求错误")]
    [MinLength(1, ErrorMessage = "请求错误")]
    [MaxLength(20, ErrorMessage = "请求错误")]
    public string Name { get; set; }

    [Required(ErrorMessage = "请求错误")]
    [MinLength(1, ErrorMessage = "请求错误")]
    [MaxLength(80, ErrorMessage = "请求错误")]
    public string Value { get; set; }
}

public record DeleteSqlFileDto : AdminDto
{
    [Required(ErrorMessage = "请求错误")]
    [MinLength(1, ErrorMessage = "请求错误")]
    [MaxLength(80, ErrorMessage = "请求错误")]
    public string Name { get; set; }
}

public record RestoreDto : AdminDto
{
    [Required(ErrorMessage = "请求错误")]
    [MinLength(1, ErrorMessage = "请求错误")]
    [MaxLength(80, ErrorMessage = "请求错误")]
    public string Name { get; set; }
}