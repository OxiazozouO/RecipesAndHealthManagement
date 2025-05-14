using CommunityToolkit.Mvvm.ComponentModel;

namespace Android.ViewModel;

public partial class CollectionTabModel : ObservableObject
{
    [ObservableProperty] private int idCategory;
    [ObservableProperty] private int id;
    [ObservableProperty] private string? fileUrl;
    [ObservableProperty] private string? title;
    [ObservableProperty] private string? refer;
}