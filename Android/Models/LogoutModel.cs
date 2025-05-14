using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Android.Models;

public partial class LogoutModel : BaseObservableValidator
{
    [Required(ErrorMessage = "密码不能为空")]
    [MinLength(6, ErrorMessage = "密码长度不能小于{1}")]
    [MaxLength(20, ErrorMessage = "密码长度不能大于{1}")]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    private string password;
}