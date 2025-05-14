using Android.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Android.ViewModel;

public partial class FavoriteAtModel : ObservableObject
{
    [ObservableProperty] private FavoriteModel favorite;
    [ObservableProperty] private List<FavoriteItemViewModel> favoriteItems;

    public partial class FavoriteItemViewModel : ObservableObject
    {
        [ObservableProperty] private FavoriteItemModel favoriteItem;
        [ObservableProperty] private UserModel user;
    }
}