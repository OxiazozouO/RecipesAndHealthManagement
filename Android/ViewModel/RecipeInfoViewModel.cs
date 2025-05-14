using CommunityToolkit.Mvvm.ComponentModel;

namespace Android.ViewModel;

public partial class RecipeInfoViewModel : ObservableObject
{
    [ObservableProperty] private int recipeId;
    [ObservableProperty] public int authorId;
    [ObservableProperty] public string authorUName;
    [ObservableProperty] public string authorFileUrl;
    [ObservableProperty] private string title;
    [ObservableProperty] private string fileUrl;
    [ObservableProperty] private string summary;
    
    [ObservableProperty] private DateTime modifyDate;
    
    [ObservableProperty] private int favoriteCount;
    [ObservableProperty] private bool isLike;
}