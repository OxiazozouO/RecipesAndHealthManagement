using AnyLibrary.Helper;
using WebServer.DatabaseModel;

namespace WebServer.Configurations;

public static class AppSettings
{
    private static AppSettingsModel? _model;

    public static AppSettingsModel Model
    {
        get
        {
            if (_model == null
                || _model.JwtConfig == null
                || _model.BackupsConfig == null
                || _model.UserConfig == null
                || _model.FileUrlConfig == null
                || _model.ConnectionString == null)
                _model = Default;
            return _model;
        }
    }

    public static string[] Prefixes = new[] { "JwtConfig:", "FileUrlConfig:", "BackupsConfig:", "UserConfig:" };
    public static string ConnectionString => Model.ConnectionString;
    public static JwtConfig JwtConfig => Model.JwtConfig;
    public static FileUrlConfig FileUrlConfig => Model.FileUrlConfig;
    public static BackupsConfig BackupsConfig => Model.BackupsConfig;
    public static UserConfig UserConfig => Model.UserConfig;

    private static readonly AppSettingsModel Default = new AppSettingsModel
    {
        ConnectionString = "server=localhost;user=root;password=123456;database=recipe_and_health_system", //数据库连接字符串
        JwtConfig = new JwtConfig
        {
            SecretKey = "bbwdxdzyxxgcxyjsjkxyjs211bjkh2104405140", //密钥
            Issuer = "WebAppIssuer", //签发者
            Audience = "WebAppAudience", //受众
            Expired = 120 //过期时间（分钟）
        },
        FileUrlConfig = new FileUrlConfig
        {
            OldFilePath = "File",
            Recipes = "Recipes",
            Favorites = "Favorites",
            Ingredients = "Ingredients",
            Collections = "Collections",
            Users = "Users",
            Admins = "Admins",
            Temps = "Temps",
            UserDomain = "10.0.2.2",
            AdminDomain = "localhost"
        },
        BackupsConfig = new BackupsConfig
        {
            MySQLPath = @"C:\Program Files\MySQL\MySQL Server 8.0\bin\", //数据库路径
            DbName = "recipe_and_health_system", //数据库名
            DbUser = "root", //数据库用户名
            BackupPath = @"File\SQLs" //数据库备份路径
        },
        UserConfig = new UserConfig
        {
            UserMaxReportCount = 3, //用户最大举报次数
            BanTime = 7 //用户封禁时间（天）
        }
    };

    public static void SaveToFile()
    {
        File.WriteAllText("AppSettings.json", Model.ToJson());
    } //保存配置到文件  （服务端启动，但是数据库未启动时采用）

    public static void LoadByFile()
    {
        if (File.Exists("AppSettings.json"))
        {
            _model = File.ReadAllText("AppSettings.json").ToEntity<AppSettingsModel>();
        }
    } //从文件加载配置  （服务端启动，但是数据库未启动时采用）

    public static void SaveToDb(List<SystemConfig> cfgs)
    {
        if (_model == null) return;

        if (_model.JwtConfig != null)
        {
            UpdateConfig(cfgs, "JwtConfig:SecretKey", _model.JwtConfig.SecretKey);
            UpdateConfig(cfgs, "JwtConfig:Issuer", _model.JwtConfig.Issuer);
            UpdateConfig(cfgs, "JwtConfig:Audience", _model.JwtConfig.Audience);
            UpdateConfig(cfgs, "JwtConfig:Expired", _model.JwtConfig.Expired.ToString());
        }

        if (_model.FileUrlConfig != null)
        {
            UpdateConfig(cfgs, "FileUrlConfig:OldFilePath", _model.FileUrlConfig.OldFilePath);
            UpdateConfig(cfgs, "FileUrlConfig:Recipes", _model.FileUrlConfig.Recipes);
            UpdateConfig(cfgs, "FileUrlConfig:Favorites", _model.FileUrlConfig.Favorites);
            UpdateConfig(cfgs, "FileUrlConfig:Ingredients", _model.FileUrlConfig.Ingredients);
            UpdateConfig(cfgs, "FileUrlConfig:Collections", _model.FileUrlConfig.Collections);
            UpdateConfig(cfgs, "FileUrlConfig:Users", _model.FileUrlConfig.Users);
            UpdateConfig(cfgs, "FileUrlConfig:Admins", _model.FileUrlConfig.Admins);
            UpdateConfig(cfgs, "FileUrlConfig:Temps", _model.FileUrlConfig.Temps);
            UpdateConfig(cfgs, "FileUrlConfig:UserDomain", _model.FileUrlConfig.UserDomain);
            UpdateConfig(cfgs, "FileUrlConfig:AdminDomain", _model.FileUrlConfig.AdminDomain);
        }

        if (_model.UserConfig != null)
        {
            UpdateConfig(cfgs, "UserConfig:UserMaxReportCount", _model.UserConfig.UserMaxReportCount.ToString());
            UpdateConfig(cfgs, "UserConfig:BanTime", _model.UserConfig.BanTime.ToString());
        }

        if (_model.BackupsConfig != null)
        {
            UpdateConfig(cfgs, "BackupsConfig:MySQLDumpPath", _model.BackupsConfig.MySQLPath);
            UpdateConfig(cfgs, "BackupsConfig:DbName", _model.BackupsConfig.DbName);
            UpdateConfig(cfgs, "BackupsConfig:DbUser", _model.BackupsConfig.DbUser);
            UpdateConfig(cfgs, "BackupsConfig:BackupPath", _model.BackupsConfig.BackupPath);
        }
    } //保存配置到数据库

    public static void LoadByDb(Dictionary<string, string> dir)
    {
        if (dir is null) return;
        Model.JwtConfig ??= new JwtConfig();
        SetValueIfExists(dir, "JwtConfig:SecretKey", value => Model.JwtConfig.SecretKey = value);
        SetValueIfExists(dir, "JwtConfig:Issuer", value => Model.JwtConfig.Issuer = value);
        SetValueIfExists(dir, "JwtConfig:Audience", value => Model.JwtConfig.Audience = value);
        SetValueIfExists(dir, "JwtConfig:Expired", value => Model.JwtConfig.Expired = int.Parse(value));
        Model.FileUrlConfig ??= new FileUrlConfig();
        SetValueIfExists(dir, "FileUrlConfig:OldFilePath", value => Model.FileUrlConfig.OldFilePath = value);
        SetValueIfExists(dir, "FileUrlConfig:Recipes",
            value => SetValueAndCreateDirectory(v => Model.FileUrlConfig.Recipes = v, value));
        SetValueIfExists(dir, "FileUrlConfig:Favorites", value => Model.FileUrlConfig.Favorites = value);
        SetValueIfExists(dir, "FileUrlConfig:Ingredients", value => Model.FileUrlConfig.Ingredients = value);
        SetValueIfExists(dir, "FileUrlConfig:Collections", value => Model.FileUrlConfig.Collections = value);
        SetValueIfExists(dir, "FileUrlConfig:Users", value => Model.FileUrlConfig.Users = value);
        SetValueIfExists(dir, "FileUrlConfig:Admins", value => Model.FileUrlConfig.Admins = value);
        SetValueIfExists(dir, "FileUrlConfig:Temps", value => Model.FileUrlConfig.Temps = value);

        SetValueIfExists(dir, "FileUrlConfig:UserDomain", value => Model.FileUrlConfig.UserDomain = value);
        SetValueIfExists(dir, "FileUrlConfig:AdminDomain", value => Model.FileUrlConfig.AdminDomain = value);
        Model.UserConfig ??= new UserConfig();
        SetValueIfExists(dir, "UserConfig:UserMaxReportCount",
            value => Model.UserConfig.UserMaxReportCount = int.Parse(value));
        SetValueIfExists(dir, "UserConfig:BanTime", value => Model.UserConfig.BanTime = int.Parse(value));
        Model.BackupsConfig ??= new BackupsConfig();
        SetValueIfExists(dir, "BackupsConfig:MySQLDumpPath", value => Model.BackupsConfig.MySQLPath = value);
        SetValueIfExists(dir, "BackupsConfig:DbName", value => Model.BackupsConfig.DbName = value);
        SetValueIfExists(dir, "BackupsConfig:DbUser", value => Model.BackupsConfig.DbUser = value);
        SetValueIfExists(dir, "BackupsConfig:BackupPath", value => Model.BackupsConfig.BackupPath = value);
        SaveToFile();
    } //从数据库加载配置


    public static void Reset()
    {
        _model = Default;
        File.WriteAllText("AppSettings.json", Model.ToJson());
    }

    private static void SetValueIfExists(Dictionary<string, string> dir, string key, Action<string> setter)
    {
        if (dir.TryGetValue(key, out var value))
        {
            setter(value);
        }
    }

    private static void SetValueAndCreateDirectory(Action<string> setValueAction, string value)
    {
        setValueAction(value);
        Directory.CreateDirectory(Path.Combine(Model.FileUrlConfig.OldFilePath, value));
    }

    private static void UpdateConfig(List<SystemConfig> cfgs, string configName, string configValue)
    {
        var config = cfgs.FirstOrDefault(c => c.ConfigName == configName);
        if (config != null)
        {
            config.ConfigValue = configValue;
        }
        else
        {
            cfgs.Add(new SystemConfig { ConfigName = configName, ConfigValue = configValue });
        }
    }
}

public class AppSettingsModel
{
    public string ConnectionString { get; set; }
    public JwtConfig JwtConfig { get; set; }
    public FileUrlConfig FileUrlConfig { get; set; }
    public BackupsConfig BackupsConfig { get; set; }
    public UserConfig UserConfig { get; set; }
}

public class FileUrlConfig
{
    public string OldFilePath { get; set; }
    public string Recipes { get; set; }
    public string Favorites { get; set; }
    public string Ingredients { get; set; }
    public string Collections { get; set; }
    public string Users { get; set; }
    public string Admins { get; set; }
    public string Temps { get; set; }
    public string UserDomain { get; set; }
    public string AdminDomain { get; set; }
}

public class BackupsConfig
{
    public string MySQLPath { get; set; }
    public string DbName { get; set; }
    public string DbUser { get; set; }
    public string BackupPath { get; set; }
}

public class UserConfig
{
    public int UserMaxReportCount { get; set; }
    public int BanTime { get; set; }
}