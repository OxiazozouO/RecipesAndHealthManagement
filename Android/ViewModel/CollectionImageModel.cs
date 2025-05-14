using CommunityToolkit.Mvvm.ComponentModel;

namespace Android.ViewModel;

public partial class CollectionImageModel : ObservableObject
{
    [ObservableProperty] private string id;
    [ObservableProperty] private string url;
}