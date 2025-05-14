using System;
using System.Collections.Generic;

namespace WebServer.DatabaseModel;

/// <summary>
/// 活动水平
/// </summary>
public partial class ConfigActivityLevel
{
    /// <summary>
    /// ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string Key { get; set; } = null!;

    /// <summary>
    /// 值
    /// </summary>
    public double Value { get; set; }

    public virtual ICollection<PhysicalSignsRecord> PhysicalSignsRecords { get; set; } = new List<PhysicalSignsRecord>();
}
