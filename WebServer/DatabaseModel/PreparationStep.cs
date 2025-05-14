using System;
using System.Collections.Generic;

namespace WebServer.DatabaseModel;

/// <summary>
/// 制作步骤
/// </summary>
public partial class PreparationStep
{
    /// <summary>
    /// ID
    /// </summary>
    public long PreparationStepId { get; set; }

    /// <summary>
    /// 食谱ID
    /// </summary>
    public long RecipeId { get; set; }

    /// <summary>
    /// 顺序编号
    /// </summary>
    public int SequenceNumber { get; set; }

    /// <summary>
    /// 标题
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// 步骤图片
    /// </summary>
    public string? FileUrl { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string Refer { get; set; } = null!;

    /// <summary>
    /// 所需时间
    /// </summary>
    public TimeSpan? RequiredTime { get; set; }

    /// <summary>
    /// 耗时比例
    /// </summary>
    public string RequiredIngredient { get; set; } = null!;

    /// <summary>
    /// 小结
    /// </summary>
    public string? Summary { get; set; }

    public virtual Recipe Recipe { get; set; } = null!;
}
