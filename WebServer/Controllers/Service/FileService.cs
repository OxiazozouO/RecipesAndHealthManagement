using AnyLibrary.Constants;
using AnyLibrary.Helper;
using NuGet.Packaging;
using WebServer.Configurations;
using WebServer.DatabaseModel;
using WebServer.DTOs;

namespace WebServer.Controllers.Service;

public static class FileService
{
    public static List<string> GetNotUsed(RecipeAndHealthSystemContext db)
    {
        var list = GetFiles(db);
        var files = GetFiles();
        return files.Where(f => !list.Contains(f)).ToList();
    }

    public static dynamic GetFileInfo(RecipeAndHealthSystemContext db)
    {
        var list = GetFiles(db);
        var files = GetFiles();

        int all = files.Count;
        var notUsed = files.Count(f => !list.Contains(f));
        var invalid = list.Count(l => !files.Contains(l));
        long totalSize = 0;
        long spaceSize = 0;
        foreach (DriveInfo drive in DriveInfo.GetDrives())
        {
            if (!drive.IsReady) continue;
            totalSize += drive.TotalSize;
            spaceSize += drive.AvailableFreeSpace;
        }

        var fs = Directory.GetFiles(Path.Combine(AppSettings.FileUrlConfig.OldFilePath,
            AppSettings.BackupsConfig.BackupPath));
        return new
        {
            All = all,
            NotUsed = notUsed,
            Invalid = invalid,
            TotalSize = totalSize,
            SpaceSize = spaceSize,
            Files = fs.Select(Path.GetFileName).ToList()
        };
    }

    public static List<string> GetFiles()
    {
        var url = AppSettings.FileUrlConfig;
        List<string> list =
        [
            url.Recipes,
            url.Ingredients,
            url.Collections,
            url.Favorites,
            url.Users,
            url.Admins,
            url.Temps
        ];
        var files = list
            .Select(l =>
                Directory.GetFiles(
                    Path.GetFullPath(
                        Path.Combine(url.OldFilePath, l)
                    )
                )
            )
            .SelectMany(f => f)
            .ToList();
        return files;
    }

    public static List<string> GetFiles(RecipeAndHealthSystemContext db)
    {
        HashSet<string> list = [];
        var url = AppSettings.FileUrlConfig;
        var r1 = Path.GetFullPath(Path.Combine(url.OldFilePath, url.Recipes));
        var i1 = Path.GetFullPath(Path.Combine(url.OldFilePath, url.Ingredients));
        var c1 = Path.GetFullPath(Path.Combine(url.OldFilePath, url.Collections));
        var f1 = Path.GetFullPath(Path.Combine(url.OldFilePath, url.Favorites));
        var u1 = Path.GetFullPath(Path.Combine(url.OldFilePath, url.Users));
        var a1 = Path.GetFullPath(Path.Combine(url.OldFilePath, url.Admins));

        list.AddRange(db.Recipes.Select(r => Path.Combine(r1, r.FileUrl)).AsEnumerable());
        list.AddRange(db.PreparationSteps
            .Where(s => !string.IsNullOrEmpty(s.FileUrl))
            .Select(s => Path.Combine(r1, s.FileUrl))
            .AsEnumerable()
        );
        list.AddRange(db.Ingredients
            .Where(s => !string.IsNullOrEmpty(s.FileUrl))
            .Select(i => Path.Combine(i1, i.FileUrl))
            .AsEnumerable()
        );

        db.Collections
            .Select(c => new { c.FileUrl, c.Content })
            .ToList()
            .ForEach(c =>
            {
                list.Add(Path.Combine(c1, c.FileUrl));
                list.AddRange(c.Content.ToEntity<HtmlData>().Images.Select(img => Path.Combine(c1, img)));
            });

        list.AddRange(db.Favorites
            .Where(s => !string.IsNullOrEmpty(s.FileUrl))
            .Select(f => Path.Combine(f1, f.FileUrl))
            .AsEnumerable()
        );
        list.AddRange(db.Users
            .Where(s => !string.IsNullOrEmpty(s.FileUrl))
            .Select(f => Path.Combine(u1, f.FileUrl))
            .AsEnumerable()
        );
        list.AddRange(db.Admins
            .Where(s => !string.IsNullOrEmpty(s.FileUrl))
            .Select(f => Path.Combine(a1, f.FileUrl))
            .AsEnumerable()
        );
        return list.ToList();
    }
}