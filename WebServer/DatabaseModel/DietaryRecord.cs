using System;
using System.Collections.Generic;

namespace WebServer.DatabaseModel;

/// <summary>
/// 饮食记录
/// </summary>
public partial class DietaryRecord
{
    /// <summary>
    /// ID
    /// </summary>
    public long EdId { get; set; }

    /// <summary>
    /// 用户ID
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// 目标类型
    /// </summary>
    public int IdCategory { get; set; }

    /// <summary>
    /// 目标ID
    /// </summary>
    public long Tid { get; set; }

    /// <summary>
    /// 用量列表
    /// </summary>
    public string Dosages { get; set; } = null!;

    /// <summary>
    /// 营养元素缓存
    /// </summary>
    public string NutrientContent { get; set; } = null!;

    /// <summary>
    /// 关联时间
    /// </summary>
    public DateTime TieUpDate { get; set; }

    public virtual User User { get; set; } = null!;
}
