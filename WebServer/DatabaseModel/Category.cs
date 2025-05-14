using System;
using System.Collections.Generic;

namespace WebServer.DatabaseModel;

/// <summary>
/// 分类
/// </summary>
public partial class Category
{
    /// <summary>
    /// ID
    /// </summary>
    public long CategoryId { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string CName { get; set; } = null!;

    /// <summary>
    /// 标注类型
    /// </summary>
    public int TypeId { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateDate { get; set; }

    public virtual ICollection<CategoryItem> CategoryItems { get; set; } = new List<CategoryItem>();
}
