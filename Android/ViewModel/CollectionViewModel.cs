using Android.Helper;
using Android.HttpClients;
using Android.Models;
using AnyLibrary.Constants;
using AnyLibrary.Helper;
using CommunityToolkit.Mvvm.ComponentModel;
using Generators;

namespace Android.ViewModel;

public partial class CollectionModel : ObservableObject
{
    [ObservableProperty] private int collectionId;
    [ObservableProperty] private string fileUrl;
    [ObservableProperty] private string refer;
    [ObservableProperty] private string title;
    [AddOnlyUpdate] [ObservableProperty] private string html;
    [ObservableProperty] private List<CollectionTabModel> tabs;
    [ObservableProperty] private List<CollectionImageModel> images;
    
    [ObservableProperty] private int favoriteCount;
    [ObservableProperty] private bool isLike;
    [ObservableProperty] private List<CategoryModel> category;
    

    partial void OnHtmlChanged(string? oldValue, string newValue)
    {
        Init();
    }

    partial void OnTabsChanged(List<CollectionTabModel>? oldValue, List<CollectionTabModel> newValue)
    {
        Init();
    }


    partial void OnImagesChanged(List<CollectionImageModel>? oldValue, List<CollectionImageModel> newValue)
    {
        Init();
    }

    private void Init()
    {
        if (IsOnlyUpdate(nameof(Html))) return;
        var h = HtmlHelper.SetHtml(Html, Images, Tabs);
        if (h is not null) OnlyUpdateHtml = h;
    }
    
    public void AddCategory(string text, sbyte flag, Action action)
    {
        var chars = text.ToCharArray();
        if (flag != CategoryType.Category && (flag != CategoryType.Emoji || chars.Length != 2)) return;
        var req = ApiEndpoints.CreatCategoryItem(new
        {
            UserId = AppConfigHelper.AppConfig.Id,
            TypeId = flag,
            TId = CollectionId,
            IdCategory = IdCategory.Collection,
            Name = text
        });

        if (!req.Execute(out var res)) return;
        var entity = res.Data.ToEntity<CategoryModel>();
        Category.Add(entity);
        action.Invoke();
    }
}

