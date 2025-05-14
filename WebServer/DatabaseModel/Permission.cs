using System;
using System.Collections.Generic;

namespace WebServer.DatabaseModel;

/// <summary>
/// 权限
/// </summary>
public partial class Permission
{
    /// <summary>
    /// ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 请求路径
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// 分组名
    /// </summary>
    public string Category { get; set; } = null!;

    /// <summary>
    /// 权限名
    /// </summary>
    public string Title { get; set; } = null!;

    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
