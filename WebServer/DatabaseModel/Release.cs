using System;
using System.Collections.Generic;

namespace WebServer.DatabaseModel;

/// <summary>
/// 发布
/// </summary>
public partial class Release
{
    /// <summary>
    /// ID
    /// </summary>
    public long ReleaseId { get; set; }

    /// <summary>
    /// 作者ID
    /// </summary>
    public long AuthorId { get; set; }

    /// <summary>
    /// 操作者类型
    /// </summary>
    public int OpFlag { get; set; }

    /// <summary>
    /// 目标ID
    /// </summary>
    public long TId { get; set; }

    /// <summary>
    /// 目标类型
    /// </summary>
    public int IdCategory { get; set; }

    /// <summary>
    /// 内容
    /// </summary>
    public string Content { get; set; } = null!;

    /// <summary>
    /// 参考来源
    /// </summary>
    public string ReleaseInfo { get; set; } = null!;

    /// <summary>
    /// 标题缓存
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// 封面缓存
    /// </summary>
    public string FileUrl { get; set; } = null!;

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateDate { get; set; }

    public virtual User Author { get; set; } = null!;

    public virtual ICollection<ReleaseFlowHistory> ReleaseFlowHistories { get; set; } = new List<ReleaseFlowHistory>();
}
