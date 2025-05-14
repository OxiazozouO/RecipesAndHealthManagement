using System;
using System.Collections.Generic;

namespace WebServer.DatabaseModel;

/// <summary>
/// 收藏项
/// </summary>
public partial class FavoriteItem
{
    /// <summary>
    /// ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 收藏ID
    /// </summary>
    public long FavoriteId { get; set; }

    /// <summary>
    /// 目标ID
    /// </summary>
    public long TId { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateDate { get; set; }

    /// <summary>
    /// 修改时间
    /// </summary>
    public DateTime ModifyDate { get; set; }
}
