using System.ComponentModel.DataAnnotations;
using Android.Attribute;
using Android.Helper;
using Android.HttpClients;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Android.Models;

public partial class AddRecipeModel : BaseObservableValidator
{
    [Range(-1, long.MaxValue - 2, ErrorMessage = "请求错误")]
    public long RecipeId { get; set; }


    [Required(ErrorMessage = "必须有食谱封面")]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [StringLength(200, ErrorMessage = "上传食谱封面错误")]
    private string fileUrl;

    [Required(ErrorMessage = "必须有食谱标题")]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [StringLength(30, ErrorMessage = "食谱标题长度不能超过{1}")]
    private string title;

    [Required(ErrorMessage = "必须有食谱名字")]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [StringLength(30, ErrorMessage = "食谱名字长度不能超过{1}")]
    private string rName;

    [ObservableProperty] [NotifyDataErrorInfo] [StringLength(200, ErrorMessage = "食谱简介长度不能超过{1}")]
    private string summary;

    [ObservableProperty] [NotifyDataErrorInfo] [StringLength(200, ErrorMessage = "参考资料来源长度不能超过{1}")]
    private string releaseInfo;


    [Required(ErrorMessage = "不能没有食材")]
    [CollectionLength(1, int.MaxValue - 2, ErrorMessage = "至少添加{1}个食材")]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    private List<AddRecipeIngredientModel> ingredients;

    [ObservableProperty] private List<string>? categories;

    [Required(ErrorMessage = "不能没有步骤")]
    [CollectionLength(1, int.MaxValue - 2, ErrorMessage = "至少添加{1}个步骤")]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    private List<AddRecipeStepModel> steps;


    public bool ReleaseRecipe(long releaseId = -1)
    {
        if (MsgBoxHelper.Builder().TryError(Error)) return false;

        var parameters = new
        {
            Id = AppConfigHelper.AppConfig.Id,
            ReleaseId = releaseId,
            ReleaseInfo,
            Recipe = new
            {
                RecipeId,
                FileUrl,
                Title,
                RName,
                Summary,
                Ingredients = Ingredients.Select(i => new
                {
                    i.Dosage,
                    i.IngredientId
                }).ToList(),
                Categories,
                Steps = Steps.Select(s => new
                {
                    s.Id,
                    s.Title,
                    s.FileUrl,
                    s.Summary,
                    s.RequiredTime,
                    s.RequiredIngredient,
                    s.Refer
                }).ToList()
            }
        };

        var req = ApiEndpoints.ReleaseRecipe(parameters);
        if (req.Execute(out var res))
        {
            MsgBoxHelper.Builder(res.Message).ShowDialog();
            return true;
        }

        MsgBoxHelper.Builder().TryError(res.Message);

        return false;
    }
}

public partial class AddRecipeIngredientModel : BaseObservableValidator
{
    [Required(ErrorMessage = "食材的id是必须的")]
    [Range(1, long.MaxValue - 2, ErrorMessage = "没有该食材")]
    [ObservableProperty]
    private long ingredientId;
    
    [Required(ErrorMessage = "用量不能为空")]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    private decimal dosage;
}

public partial class AddRecipeStepModel : BaseObservableValidator
{
    [Range(1, long.MaxValue - 2, ErrorMessage = "请求错误")]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    public long id;
    
    [Required(ErrorMessage = "必须有步骤的标题")]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [StringLength(30, ErrorMessage = "步骤的标题最大长度限制在{1}")]
    private string title;

    [ObservableProperty] [NotifyDataErrorInfo] [StringLength(200, ErrorMessage = "上传步骤图片错误")]
    private string fileUrl;

    [Required(ErrorMessage = "必须有食谱封面")]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [StringLength(200, ErrorMessage = "步骤的总结最大长度限制在{1}")]
    private string summary;

    [ObservableProperty] private TimeSpan? requiredTime;

    [ObservableProperty] private string requiredIngredient;

    [Required(ErrorMessage = "必须有食谱封面")]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [StringLength(200, ErrorMessage = "步骤的总结最大长度限制在{1}")]
    private string refer;
}