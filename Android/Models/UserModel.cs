using CommunityToolkit.Mvvm.ComponentModel;

namespace Android.Models;

public partial class UserModel : ObservableObject
{
    [ObservableProperty] private long userId;
    [ObservableProperty] private string authorUName;
    [ObservableProperty] private string authorFileUrl;

    [ObservableProperty] private DateTime modifyDate;
}