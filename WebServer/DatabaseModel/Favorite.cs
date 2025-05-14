using System;
using System.Collections.Generic;

namespace WebServer.DatabaseModel;

/// <summary>
/// 收藏
/// </summary>
public partial class Favorite
{
    /// <summary>
    /// ID
    /// </summary>
    public long FavoriteId { get; set; }

    /// <summary>
    /// 用户ID
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// 封面图片
    /// </summary>
    public string FileUrl { get; set; } = null!;

    /// <summary>
    /// 名称
    /// </summary>
    public string FName { get; set; } = null!;

    /// <summary>
    /// 简介
    /// </summary>
    public string Refer { get; set; } = null!;

    /// <summary>
    /// 收藏类型
    /// </summary>
    public int IdCategory { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateDate { get; set; }

    /// <summary>
    /// 修改时间
    /// </summary>
    public DateTime ModifyDate { get; set; }

    public virtual User User { get; set; } = null!;
}
