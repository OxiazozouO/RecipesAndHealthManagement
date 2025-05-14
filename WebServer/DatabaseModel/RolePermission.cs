using System;
using System.Collections.Generic;

namespace WebServer.DatabaseModel;

/// <summary>
/// 角色权限
/// </summary>
public partial class RolePermission
{
    /// <summary>
    /// ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 角色ID
    /// </summary>
    public int RoleId { get; set; }

    /// <summary>
    /// 权限ID
    /// </summary>
    public int PermissionId { get; set; }

    public virtual Permission Permission { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
