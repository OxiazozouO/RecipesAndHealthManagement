using CommunityToolkit.Mvvm.ComponentModel;

namespace Android.Models;

public partial class CommentModel : ObservableObject
{
    [ObservableProperty] private long commentId;
    [ObservableProperty] private long userId;
    [ObservableProperty] private string fileUrl;
    [ObservableProperty] private string uName;
    [ObservableProperty] private long tid;
    [ObservableProperty] private sbyte typeId;
    [ObservableProperty] private string content;
    [ObservableProperty] private DateTime createDate;
}