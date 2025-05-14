using System;
using System.Collections.Generic;

namespace WebServer.DatabaseModel;

/// <summary>
/// 体征记录
/// </summary>
public partial class PhysicalSignsRecord
{
    /// <summary>
    /// ID
    /// </summary>
    public long UpiId { get; set; }

    /// <summary>
    /// 用户ID
    /// </summary>
    public long UId { get; set; }

    /// <summary>
    /// 活动水平ID
    /// </summary>
    public int CalId { get; set; }

    /// <summary>
    /// 身高
    /// </summary>
    public double Height { get; set; }

    /// <summary>
    /// 体重
    /// </summary>
    public double Weight { get; set; }

    /// <summary>
    /// 蛋白质供能占比
    /// </summary>
    public double ProteinPercentage { get; set; }

    /// <summary>
    /// 脂肪供能占比
    /// </summary>
    public double FatPercentage { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime CreateDate { get; set; }

    public virtual ConfigActivityLevel Cal { get; set; } = null!;

    public virtual User UIdNavigation { get; set; } = null!;
}
