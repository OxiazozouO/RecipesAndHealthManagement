using Android.Configurations;
using Android.Content;
using Android.HttpClients;
using Android.Models;
using Android.Preferences;
using AnyLibrary.Helper;

namespace Android.Helper;

public static class AppConfigHelper
{
    public static ISharedPreferences sp = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
    public static AppConfig _appConfig;
    public static MyInfoModel _myInfo;
    public static ModelConfig _modelConfig;


    public static double _tdeeCorrect;

    public static double TdeeCorrect
    {
        get
        {
            _tdeeCorrect = sp.GetFloat("tdeeCorrect", 1.2f);
            return _tdeeCorrect;
        }
        set
        {
            sp.Edit()?.PutFloat("tdeeCorrect", (float)value)?.Commit();
            _tdeeCorrect = value;
        }
    }

    public static List<string> _searchHistory;

    public static List<string> SearchHistory
    {
        get
        {
            InitSearchHistory();
            return _searchHistory;
        }
        set => _searchHistory = value;
    }

    public static void InitSearchHistory()
    {
        if (_searchHistory is null)
        {
            var config = sp.GetString("searchHistory", null);
            if (!string.IsNullOrEmpty(config))
                _searchHistory = config.ToEntity<List<string>>();
        }

        _searchHistory ??= [];
    }

    public static void SaveSearchHistory()
    {
        sp.Edit()?.Remove("searchHistory")?.Commit();
        sp.Edit()?.PutString("searchHistory", _searchHistory.ToJson())?.Commit();
    }

    public static List<string> _favoriteSearchHistory;

    public static List<string> FavoriteSearchHistory
    {
        get
        {
            InitFavoriteSearchHistory();
            return _searchHistory;
        }
        set => _searchHistory = value;
    }

    public static void InitFavoriteSearchHistory()
    {
        if (_searchHistory is null)
        {
            var config = sp.GetString("favoriteSearchHistory", null);
            if (!string.IsNullOrEmpty(config))
                _searchHistory = config.ToEntity<List<string>>();
        }

        _searchHistory ??= new List<string>();
    }

    public static void SaveFavoriteSearchHistory()
    {
        sp.Edit()?.Remove("favoriteSearchHistory")?.Commit();
        sp.Edit()?.PutString("favoriteSearchHistory", _favoriteSearchHistory.ToJson())?.Commit();
    }

    public static AppConfig AppConfig
    {
        get
        {
            if (_appConfig is null)
                InitAppConfig();
            return _appConfig;
        }
        set
        {
            _appConfig = value;
            InitAppConfig();
        }
    }

    public static ModelConfig ModelConfig
    {
        get
        {
            if (_myInfo is null)
                InitMyInfo();
            return _modelConfig;
        }
        private set
        {
            _modelConfig = value;
            InitAppConfig();
        }
    }

    public static event EventHandler AppConfigChanged;

    public static void InitAppConfig()
    {
        var config = _appConfig;
        if (config is null)
        {
            var str = sp.GetString("app_config", null);
            if (!string.IsNullOrEmpty(str))
            {
                config = str.ToEntity<AppConfig>();
            }

            if (config is null)
            {
                ActivityHelper.GotoLogin();
                return;
            }
        }

        Del();
        _appConfig = config;
        Save();
    }


    public static MyInfoModel MyInfo
    {
        get
        {
            if (_myInfo is null)
                InitMyInfo();
            return _myInfo;
        }
        private set
        {
            _myInfo = value;
            InitAppConfig();
        }
    }

    public static void InitMyInfo()
    {
        if (_modelConfig is null)
        {
            _modelConfig ??= ApiService.GetConfig();
            _modelConfig.Init();
        }

        _myInfo = ApiService.GetInfo(_appConfig.Id);
        _modelConfig.InitNutritional(
            _myInfo.Gender,
            _myInfo.BirthDate.GetAge()
        );
        _myInfo.PhysicalList ??= ApiService.GetMyAllInfo();
    }

    public static void Save()
    {
        _appConfig ??= new AppConfig();
        sp.Edit()?.PutString("app_config", _appConfig.ToJson())?.Commit();
        AppConfigChanged?.Invoke(null, null);
    }

    public static void Del()
    {
        _appConfig = null;
        _myInfo = null;
        _modelConfig = null;
        sp.Edit()?.Remove("app_config")?.Commit();
    }
}