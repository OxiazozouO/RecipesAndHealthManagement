using System.ComponentModel.DataAnnotations;
using Android.Helper;
using Android.HttpClients;
using Android.Models;
using AnyLibrary.Helper;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Android.ViewModel;

public partial class ChangePasswordViewModel : BaseObservableValidator
{
    private string confirmPassword;

    [Required(ErrorMessage = "密码不能为空")]
    [MinLength(6, ErrorMessage = "密码长度不能小于{1}")]
    [MaxLength(20, ErrorMessage = "密码长度不能大于{1}")]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    private string newPassword;

    [Required(ErrorMessage = "密码不能为空")]
    [MinLength(6, ErrorMessage = "密码长度不能小于{1}")]
    [MaxLength(20, ErrorMessage = "密码长度不能大于{1}")]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    private string password;

    [Required(ErrorMessage = "用户名/邮箱/手机号不能为空")]
    [MinLength(1, ErrorMessage = "最小长度为 {1} 字符")]
    [MaxLength(20, ErrorMessage = "最大长度为 {1} 字符")]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    private string identifier;

    [Required(ErrorMessage = "需要输入确认密码")]
    [Compare(nameof(NewPassword), ErrorMessage = "确认密码与密码不匹配")]
    public string ConfirmPassword
    {
        get => confirmPassword;
        set => SetProperty(ref confirmPassword, value, false);
    }

    public string Error
    {
        get
        {
            ValidateAllProperties();
            return string.Join('\n', GetErrors());
        }
    }

    public bool ChangePassword()
    {
        if (MsgBoxHelper.Builder().TryError(Error)) return false;
        DecryptHelper.GetHasPassword(NewPassword, out var salt, out var hashpswd);
        var req = ApiEndpoints.ChangePassword(new
        {
            Id = AppConfigHelper.AppConfig.Id,
            Password = hashpswd,
            NewPassword,
            PhoneOrEmail = Identifier,
            Salt = salt
        });
        if (req.Execute(out var ret))
        {
            MsgBoxHelper.Builder("修改密码成功").ShowDialog();
            return true;
        }
        else
        {
            MsgBoxHelper.Builder().TryError(ret.Message);
        }

        return false;
    }
}