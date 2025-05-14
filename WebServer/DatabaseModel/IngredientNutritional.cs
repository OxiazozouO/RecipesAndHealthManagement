using System;
using System.Collections.Generic;

namespace WebServer.DatabaseModel;

/// <summary>
/// 食材营养元素
/// </summary>
public partial class IngredientNutritional
{
    /// <summary>
    /// ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 食材ID
    /// </summary>
    public long IngredientId { get; set; }

    /// <summary>
    /// 营养元素ID
    /// </summary>
    public int NutritionalId { get; set; }

    /// <summary>
    /// 含量
    /// </summary>
    public double Value { get; set; }

    public virtual Ingredient Ingredient { get; set; } = null!;

    public virtual Nutrient Nutritional { get; set; } = null!;
}
