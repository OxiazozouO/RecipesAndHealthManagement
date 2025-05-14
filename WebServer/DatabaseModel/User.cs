using System;
using System.Collections.Generic;

namespace WebServer.DatabaseModel;

/// <summary>
/// 用户
/// </summary>
public partial class User
{
    /// <summary>
    /// ID
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string UName { get; set; } = null!;

    /// <summary>
    /// 头像
    /// </summary>
    public string? FileUrl { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; } = null!;

    /// <summary>
    /// 邮箱
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// 手机
    /// </summary>
    public string Phone { get; set; } = null!;

    /// <summary>
    /// 盐
    /// </summary>
    public string Salt { get; set; } = null!;

    /// <summary>
    /// 性别
    /// </summary>
    public bool Gender { get; set; }

    /// <summary>
    /// 生日
    /// </summary>
    public DateTime BirthDate { get; set; }

    /// <summary>
    /// 账户状态
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateDate { get; set; }

    /// <summary>
    /// 解禁时间
    /// </summary>
    public DateTime UnbanTime { get; set; }

    public virtual ICollection<CategoryItem> CategoryItems { get; set; } = new List<CategoryItem>();

    public virtual ICollection<Collection> Collections { get; set; } = new List<Collection>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<DietaryRecord> DietaryRecords { get; set; } = new List<DietaryRecord>();

    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public virtual ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();

    public virtual ICollection<PhysicalSignsRecord> PhysicalSignsRecords { get; set; } = new List<PhysicalSignsRecord>();

    public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();

    public virtual ICollection<Release> Releases { get; set; } = new List<Release>();

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
}
