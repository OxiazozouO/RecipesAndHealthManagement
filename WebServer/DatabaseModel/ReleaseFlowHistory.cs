using System;
using System.Collections.Generic;

namespace WebServer.DatabaseModel;

/// <summary>
/// 审核状态流转表
/// </summary>
public partial class ReleaseFlowHistory
{
    /// <summary>
    /// ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 发布ID
    /// </summary>
    public long ReleaseId { get; set; }

    /// <summary>
    /// 操作者ID
    /// </summary>
    public long OpId { get; set; }

    /// <summary>
    /// 操作者类型
    /// </summary>
    public int OpFlag { get; set; }

    /// <summary>
    /// 审核状态
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 补充信息
    /// </summary>
    public string? Info { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateDate { get; set; }

    public virtual Release Release { get; set; } = null!;
}
