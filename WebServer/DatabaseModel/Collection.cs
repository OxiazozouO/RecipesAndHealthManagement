using System;
using System.Collections.Generic;

namespace WebServer.DatabaseModel;

/// <summary>
/// 合集
/// </summary>
public partial class Collection
{
    /// <summary>
    /// ID
    /// </summary>
    public long CollectionId { get; set; }

    /// <summary>
    /// 作者ID
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// 封面图片
    /// </summary>
    public string FileUrl { get; set; } = null!;

    /// <summary>
    /// 合集标题
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// 简介
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// 内容
    /// </summary>
    public string Content { get; set; } = null!;

    /// <summary>
    /// 合集状态
    /// </summary>
    public int Status { get; set; }

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
