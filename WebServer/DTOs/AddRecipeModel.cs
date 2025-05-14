using System.ComponentModel.DataAnnotations;
using WebServer.Helper;

namespace WebServer.DTOs;

public record AddRecipeModel
{
    [Range(-1, long.MaxValue - 2, ErrorMessage = "请求错误")]
    public long RecipeId { get; set; }

    [Required(ErrorMessage = "必须有食谱封面")]
    [StringLength(300, ErrorMessage = "上传食谱封面错误")]
    public string FileUrl { get; set; }

    [Required(ErrorMessage = "必须有食谱标题")]
    [StringLength(30, ErrorMessage = "食谱标题长度不能超过{1}")]
    public string Title { get; set; }

    [Required(ErrorMessage = "必须有食谱名字")]
    [StringLength(30, ErrorMessage = "食谱名字长度不能超过{1}")]
    public string RName { get; set; }

    [StringLength(200, ErrorMessage = "食谱简介长度不能超过{1}")]
    public string Summary { get; set; }

    [Required(ErrorMessage = "不能没有步骤")]
    [CollectionLength(1, int.MaxValue - 2, ErrorMessage = "至少添加{1}个步骤")]
    public List<AddRecipeStepModel> Steps { get; set; }

    [Required(ErrorMessage = "不能没有食材")]
    [CollectionLength(1, int.MaxValue - 2, ErrorMessage = "至少添加{1}个食材")]
    public List<AddRecipeIngredientModel> Ingredients { get; set; }

    public List<string>? Categories { get; set; }
}

public record AddRecipeIngredientModel
{
    [Required(ErrorMessage = "食材的id是必须的")]
    [Range(1, long.MaxValue - 2, ErrorMessage = "没有该食材")]
    public long IngredientId { get; set; }

    [Required(ErrorMessage = "密码不能为空")] public decimal Dosage { get; set; }
}

public record AddRecipeStepModel
{
    [Range(-1, int.MaxValue - 2, ErrorMessage = "请求错误")]
    public long Id { get; set; }

    [Required(ErrorMessage = "必须有步骤的标题")]
    [StringLength(30, ErrorMessage = "步骤的标题最大长度限制在{1}")]
    public string Title { get; set; }

    [StringLength(300, ErrorMessage = "上传步骤图片错误")]
    public string? FileUrl { get; set; }

    [StringLength(200, ErrorMessage = "步骤的说明最大长度限制在{1}")]
    public string? Summary { get; set; }

    public TimeSpan? RequiredTime { get; set; }

    public string? RequiredIngredient { get; set; }

    [Required(ErrorMessage = "必须有步骤的描述")]
    [StringLength(200, ErrorMessage = "步骤的总结最大长度限制在{1}")]
    public string Refer { get; set; }
}