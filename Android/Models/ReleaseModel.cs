using CommunityToolkit.Mvvm.ComponentModel;

namespace Android.Models;

public partial class ReleaseModel : ObservableObject
{
    [ObservableProperty] private long releaseId;
    [ObservableProperty] private List<ReleaseFlowHistory> releaseFlowHistories;
    [ObservableProperty] private DateTime createDate;
    [ObservableProperty] private int idCategory;
    [ObservableProperty] private long tId;
    
    

    #region 计算出来的变量

    [ObservableProperty] private int status;

    [ObservableProperty] private string fileUrl;

    [ObservableProperty] private string reviewFeedback;

    [ObservableProperty] private string title;

    #endregion

    partial void OnReleaseFlowHistoriesChanged(List<ReleaseFlowHistory>? oldValue, List<ReleaseFlowHistory> newValue)
    {
        if (ReleaseFlowHistories.Count < 1) return;
        var model = ReleaseFlowHistories.First();
        Status = model.Status;
        ReviewFeedback = string.IsNullOrEmpty(model.Info) ? "无" : model.Info;
    }
}

public partial class ReleaseFlowHistory : ObservableObject
{
    [ObservableProperty] private AdminInfo user;

    [ObservableProperty] private int status;

    [ObservableProperty] private string info;

    [ObservableProperty] private DateTime createDate;
}

public partial class AdminInfo : ObservableObject
{
    [ObservableProperty] private string name;
    [ObservableProperty] private int id;
    [ObservableProperty] private string fileUrl;
}