using CommunityToolkit.Mvvm.ComponentModel;

namespace Android.ViewModel;

public partial class HomeInfoViewModel : ObservableObject
{
    [ObservableProperty] private string fileUrl;
    [ObservableProperty] private int id;
    [ObservableProperty] private string title;
    [ObservableProperty] private string userName;
    [ObservableProperty] private string authorFileUrl;
    [ObservableProperty] private string summary;
    [ObservableProperty] private int flag;
    [ObservableProperty] private string likeCount;
}