using System;
using System.Collections.Generic;

namespace WebServer.DatabaseModel;

/// <summary>
/// 食谱食材项
/// </summary>
public partial class RecipeItem
{
    /// <summary>
    /// ID
    /// </summary>
    public long RecipeItemId { get; set; }

    /// <summary>
    /// 食谱ID
    /// </summary>
    public long RecipeId { get; set; }

    /// <summary>
    /// 食材ID
    /// </summary>
    public long IngredientId { get; set; }

    /// <summary>
    /// 用量
    /// </summary>
    public decimal Dosage { get; set; }

    public virtual Ingredient Ingredient { get; set; } = null!;

    public virtual Recipe Recipe { get; set; } = null!;
}
