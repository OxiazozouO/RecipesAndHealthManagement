using System;
using System.Collections.Generic;

namespace WebServer.DatabaseModel;

/// <summary>
/// 管理员
/// </summary>
public partial class Admin
{
    /// <summary>
    /// ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 角色ID
    /// </summary>
    public int RoleId { get; set; }

    /// <summary>
    /// 头像
    /// </summary>
    public string? FileUrl { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; } = null!;

    /// <summary>
    /// 盐
    /// </summary>
    public string Salt { get; set; } = null!;

    /// <summary>
    /// 账户状态
    /// </summary>
    public int Status { get; set; }

    public virtual Role Role { get; set; } = null!;
}
