using System.ComponentModel.DataAnnotations;
using Android.Attribute;
using Android.Helper;
using Android.HttpClients;
using Android.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using Generators;

namespace Android.ViewModel;

public partial class AddCollectionViewModel : BaseObservableValidator
{
    [Required(ErrorMessage = "必须有合集封面")]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [StringLength(200, ErrorMessage = "上传合集封面错误")]
    private string fileUrl;

    [Required(ErrorMessage = "必须有合集标题")]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [StringLength(30, ErrorMessage = "合集标题长度不能超过{1}")]
    private string title;

    [ObservableProperty] [NotifyDataErrorInfo] [StringLength(200, ErrorMessage = "合集简介长度不能超过{1}")]
    private string? summary;

    [Required(ErrorMessage = "必须有合集内容")] [ObservableProperty]
    private HtmlData content;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "必须有参考资料")]
    [StringLength(200, ErrorMessage = "参考资料来源长度不能超过{1}")]
    private string releaseInfo;

    public bool AddCollection(long releaseId, long tid)
    {
        if (MsgBoxHelper.Builder().TryError(Error)) return false;

        var req = ApiEndpoints.ReleaseCollection(new
        {
            Id = AppConfigHelper.AppConfig.Id,
            ReleaseId = releaseId,
            ReleaseInfo,
            Collection = new
            {
                CollectionId = tid,
                FileUrl,
                Title,
                Summary,
                Content = new
                {
                    Content.Dirs,
                    Content.Images,
                    Content.Html,
                }
            }
        });

        if (req.Execute(out var res))
        {
            MsgBoxHelper.Builder(res.Message).ShowDialog();
            return true;
        }

        MsgBoxHelper.Builder().TryError(res.Message);

        return false;
    }
}

public partial class HtmlData : BaseObservableValidator
{
    [Required(ErrorMessage = "必须插入有食谱/食材/合集")]
    [CollectionLength(1, int.MaxValue - 2, ErrorMessage = "至少添加{1}个食谱/食材/合集")]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    private Dictionary<int, HashSet<int>> dirs;

    [Required(ErrorMessage = "必须插入有食谱/食材/合集")]
    [CollectionLength(1, int.MaxValue - 2, ErrorMessage = "至少添加{1}个食谱/食材/合集")]
    [ObservableProperty]
    private List<string> images;

    [ObservableProperty] [NotifyDataErrorInfo] [Required(ErrorMessage = "必须写入合集内容")] [AddOnlyUpdate]
    private string html;

    partial void OnHtmlChanged(string? oldValue, string newValue)
    {
        if (IsOnlyUpdate(nameof(Html))) return;
        Init();
    }

    public void Init()
    {
        OnlyUpdateHtml = HtmlHelper.GetHtml(Html, out var images, out var dirs);
        Dirs = dirs;
        Images = images;
    }
}