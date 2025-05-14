using Android.Helper;
using Android.HttpClients;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Android.Models;

public partial class CategoryModel : ObservableObject
{
    [ObservableProperty] private long id;
    [ObservableProperty] private string name;
    [ObservableProperty] private int count;
    [ObservableProperty] private bool isLike;
    [ObservableProperty] private sbyte typeId;

    public void OnLike(long tid, int idCategory)
    {
        var parameters = new
        {
            CategoryId = Id,
            TId = tid,
            IdCategory = idCategory,
            UserId = AppConfigHelper.AppConfig.Id
        };

        var req = IsLike
            ? ApiEndpoints.DeleteCategoryItem(parameters)
            : ApiEndpoints.AddCategoryItem(parameters);


        if (!req.Execute(out var res)) return;
        IsLike = !IsLike;
        Count += IsLike ? 1 : -1;
    }
}