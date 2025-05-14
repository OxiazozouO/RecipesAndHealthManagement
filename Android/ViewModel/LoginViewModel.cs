using System.ComponentModel.DataAnnotations;
using Android.Configurations;
using Android.Helper;
using Android.HttpClients;
using Android.Models;
using AnyLibrary.Helper;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Android.ViewModel;

public partial class LoginViewModel : BaseObservableValidator
{
    [Required(ErrorMessage = "用户名/邮箱/手机号不能为空")]
    [MinLength(1, ErrorMessage = "最小长度为 {1} 字符")]
    [MaxLength(20, ErrorMessage = "最大长度为 {1} 字符")]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    private string identifier;

    [Required(ErrorMessage = "密码不能为空")]
    [MinLength(6, ErrorMessage = "密码长度不能小于{1}")]
    [MaxLength(20, ErrorMessage = "密码长度不能大于{1}")]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    private string password;

    public void Login()
    {
        if (MsgBoxHelper.Builder().TryError(Error)) return;

        var req = ApiEndpoints.Login(new { Identifier, Password });

        req.Execute(res =>
        {
            AppConfigHelper.AppConfig = res.Data.ToEntity<AppConfig>();
            ActivityHelper.GoHome();
        });
    }
}