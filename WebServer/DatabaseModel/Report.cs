using System;
using System.Collections.Generic;

namespace WebServer.DatabaseModel;

/// <summary>
/// 举报
/// </summary>
public partial class Report
{
    /// <summary>
    /// ID
    /// </summary>
    public long ReportId { get; set; }

    /// <summary>
    /// 举报者ID
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

    /// <summary>
    /// 举报类型
    /// </summary>
    public int RType { get; set; }

    /// <summary>
    /// 补充说明
    /// </summary>
    public string Content { get; set; } = null!;

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateDate { get; set; }

    /// <summary>
    /// 处理状态
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 状态说明
    /// </summary>
    public string? StatusContent { get; set; }

    /// <summary>
    /// 处理时间
    /// </summary>
    public DateTime ProcessingTime { get; set; }

    public virtual User User { get; set; } = null!;
}
