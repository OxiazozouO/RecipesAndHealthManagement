using System.ComponentModel.DataAnnotations;
using Android.Helper;
using Android.HttpClients;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Android.Models;

public partial class ReportModel : BaseObservableValidator
{
    [Required(ErrorMessage = "密码不能为空")]
    [Range(1, long.MaxValue - 2, ErrorMessage = "请求错误")]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    private long tid;

    [Required(ErrorMessage = "举报类型不能为空")]
    [Range(1, int.MaxValue - 2, ErrorMessage = "请求错误")]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    private int status;

    [ObservableProperty] private string name;

    [Required(ErrorMessage = "关联对象的类型是必需的")]
    [Range(1, 5, ErrorMessage = "被举报的类型不能为空")]
    [ObservableProperty]
    private sbyte category;

    [Required(ErrorMessage = "举报内容是必需的")] [StringLength(100, ErrorMessage = "举报内容长度不能超过{1}")] [ObservableProperty]
    private string content;

    public void AddReport(App.Activity activity)
    {
        if (MsgBoxHelper.Builder().TryError(Error)) return;

        var req = ApiEndpoints.AddReport(new
        {
            Id = AppConfigHelper.AppConfig.Id,
            TId = Tid,
            Status = Status,
            Category = Category,
            Content = Content
        });

        if (req.Execute(out var res))
        {
            MsgBoxHelper
                .Builder(res.Message).OkCancel(activity.Finish);
        }
        else
        {
            MsgBoxHelper.Builder().TryError(res.Message);
        }
    }
}