using System.ComponentModel.DataAnnotations;
using Android.Helper;
using Android.HttpClients;
using Android.Models;
using AnyLibrary.Helper;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Android.ViewModel;

public partial class RegisterViewModel : BaseObservableValidator
{
    private string confirmPassword;

    [Required(ErrorMessage = "邮箱不能为空")]
    [RegularExpression(RegexHelper.Email, ErrorMessage = "邮箱格式错误")]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    
    private string email;

    [Required(ErrorMessage = "用户名不能为空")]
    [MinLength(1, ErrorMessage = "用户名长度不能小于{1}")]
    [MaxLength(20, ErrorMessage = "用户名长度不能大于{1}")]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    private string name;

    [Required(ErrorMessage = "密码不能为空")]
    [MinLength(6, ErrorMessage = "密码长度不能小于{1}")]
    [MaxLength(20, ErrorMessage = "密码长度不能大于{1}")]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    private string password;

    [Required(ErrorMessage = "手机号不能为空")]
    [RegularExpression(RegexHelper.MobileExact, ErrorMessage = "手机号格式错误")]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    private string phone;

    [Required(ErrorMessage = "需要输入确认密码")]
    [Compare(nameof(Password), ErrorMessage = "确认密码与密码不匹配")]
    public string ConfirmPassword
    {
        get => confirmPassword;
        set => SetProperty(ref confirmPassword, value, false);
    }
    
    [Required(ErrorMessage = "头像必须上传")]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    private string userImgUrl;

    public string Error
    {
        get
        {
            ValidateAllProperties();
            return string.Join('\n', GetErrors());
        }
    }

    public void Register()
    {
        if (MsgBoxHelper.Builder().TryError(Error)) return;

        DecryptHelper.GetHasPassword(Password, out var salt, out var hashpswd);

        var req = ApiEndpoints.Register(new
        {
            Name,
            Email,
            Phone,
            Password = hashpswd,
            Salt = salt,
            UserImgUrl
        });

        if (req.Execute(out var res))
            ActivityHelper.GotoLogin();
        else
            MsgBoxHelper.Builder().TryError(res.Message);
    }
}