using System;
using System.Collections.Generic;

namespace WebServer.DatabaseModel;

/// <summary>
/// 营养元素
/// </summary>
public partial class Nutrient
{
    /// <summary>
    /// ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 营养元素名称
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// 计量单位
    /// </summary>
    public string Unit { get; set; } = null!;

    /// <summary>
    /// 介绍
    /// </summary>
    public string? Refer { get; set; }

    public virtual ICollection<Dri> Dris { get; set; } = new List<Dri>();

    public virtual ICollection<IngredientNutritional> IngredientNutritionals { get; set; } = new List<IngredientNutritional>();
}
