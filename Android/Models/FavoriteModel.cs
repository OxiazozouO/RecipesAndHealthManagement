using System.ComponentModel.DataAnnotations;
using Android.Helper;
using Android.HttpClients;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Android.Models;

public partial class FavoriteModel : BaseObservableValidator
{
    [Required(ErrorMessage = "收藏夹名称不能为空")]
    [StringLength(30, ErrorMessage = "简介不能超过{1}个字符")]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    private string fName;

    [ObservableProperty] private long favoriteId;


    [StringLength(50, ErrorMessage = "收藏夹封面不能超过{1}个字符")] [ObservableProperty] [NotifyDataErrorInfo]
    private string fileUrl;

    [Required] [Range(1, 254, ErrorMessage = "参数错误")] [NotifyDataErrorInfo] [ObservableProperty]
    private sbyte flag;

    [ObservableProperty] private DateTime modifyDate;
    [ObservableProperty] private int itemsCount;

    [StringLength(198, ErrorMessage = "收藏夹简介不能超过{1}个字符")] [NotifyDataErrorInfo] [ObservableProperty]
    private string refer;

    [ObservableProperty] private int count;

    public string Error
    {
        get
        {
            ValidateAllProperties();
            return string.Join('\n', GetErrors());
        }
    }

    public int AddFavorite()
    {
        if (MsgBoxHelper
            .Builder()
            .TryError(Error + (string.IsNullOrEmpty(FileUrl) ? "收藏夹封面不能为空" : ""))
           ) return -1;

        var req = ApiEndpoints.AddFavorite(new
        {
            Id = AppConfigHelper.AppConfig.Id,
            Flag = Flag,
            FileUrl = FileUrl,
            FName = FName,
            Refer = Refer
        });

        if (req.Execute(out var res))
        {
            return int.Parse((string)res.Data);
        }

        MsgBoxHelper.Builder().TryError(res.Message);

        return -1;
    }

    public int EditFavorite()
    {
        if (MsgBoxHelper.Builder().TryError(Error)) return -1;
        FileUrl ??= "";

        var req = ApiEndpoints.EditFavorite(new
        {
            Id = AppConfigHelper.AppConfig.Id,
            Flag = Flag,
            FavoriteId = FavoriteId,
            FileName = FileUrl,
            FName = FName,
            Refer = Refer
        });

        if (req.Execute(out var res))
        {
            return 1;
        }

        MsgBoxHelper.Builder().TryError(res.Message);

        return -1;
    }
}