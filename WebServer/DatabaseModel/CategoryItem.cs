using System;
using System.Collections.Generic;

namespace WebServer.DatabaseModel;

/// <summary>
/// 分类项
/// </summary>
public partial class CategoryItem
{
    /// <summary>
    /// ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 分类ID
    /// </summary>
    public long CategoryId { get; set; }

    /// <summary>
    /// 作者ID
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// 目标ID
    /// </summary>
    public long TId { get; set; }

    /// <summary>
    /// 目标类型
    /// </summary>
    public int IdCategory { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
