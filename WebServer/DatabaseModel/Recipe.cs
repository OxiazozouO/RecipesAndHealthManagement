using System;
using System.Collections.Generic;

namespace WebServer.DatabaseModel;

/// <summary>
/// 食谱
/// </summary>
public partial class Recipe
{
    /// <summary>
    /// ID
    /// </summary>
    public long RecipeId { get; set; }

    /// <summary>
    /// 作者ID
    /// </summary>
    public long AuthorId { get; set; }

    /// <summary>
    /// 标题
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// 名称
    /// </summary>
    public string? RName { get; set; }

    /// <summary>
    /// 封面
    /// </summary>
    public string FileUrl { get; set; } = null!;

    /// <summary>
    /// 总结
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// 食谱状态
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateDate { get; set; }

    /// <summary>
    /// 修改时间
    /// </summary>
    public DateTime ModifyDate { get; set; }

    public virtual User Author { get; set; } = null!;

    public virtual ICollection<PreparationStep> PreparationSteps { get; set; } = new List<PreparationStep>();

    public virtual ICollection<RecipeItem> RecipeItems { get; set; } = new List<RecipeItem>();
}
