using System;
using System.Collections.Generic;

namespace WebServer.DatabaseModel;

/// <summary>
/// 系统配置
/// </summary>
public partial class SystemConfig
{
    /// <summary>
    /// ID
    /// </summary>
    public int ConfigId { get; set; }

    /// <summary>
    /// 键名
    /// </summary>
    public string ConfigName { get; set; } = null!;

    /// <summary>
    /// 配置值
    /// </summary>
    public string ConfigValue { get; set; } = null!;

    /// <summary>
    /// 描述
    /// </summary>
    public string? ConfigRefer { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateDate { get; set; }

    /// <summary>
    /// 修改时间
    /// </summary>
    public DateTime UpdateDate { get; set; }
}
