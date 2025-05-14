using System;
using System.Collections.Generic;

namespace WebServer.DatabaseModel;

/// <summary>
/// 食材
/// </summary>
public partial class Ingredient
{
    /// <summary>
    /// ID
    /// </summary>
    public long IngredientId { get; set; }

    /// <summary>
    /// 作者ID
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string IName { get; set; } = null!;

    /// <summary>
    /// 封面
    /// </summary>
    public string? FileUrl { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string? Refer { get; set; }

    /// <summary>
    /// 单位
    /// </summary>
    public string Unit { get; set; } = null!;

    /// <summary>
    /// 转换计量
    /// </summary>
    public string Quantity { get; set; } = null!;

    /// <summary>
    /// 过敏信息
    /// </summary>
    public string? Allergy { get; set; }

    /// <summary>
    /// 食材状态
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 净含量
    /// </summary>
    public double Content { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateDate { get; set; }

    /// <summary>
    /// 修改时间
    /// </summary>
    public DateTime ModifyDate { get; set; }

    public virtual ICollection<IngredientNutritional> IngredientNutritionals { get; set; } = new List<IngredientNutritional>();

    public virtual ICollection<RecipeItem> RecipeItems { get; set; } = new List<RecipeItem>();

    public virtual User User { get; set; } = null!;
}
