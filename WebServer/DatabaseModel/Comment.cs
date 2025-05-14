using System;
using System.Collections.Generic;

namespace WebServer.DatabaseModel;

/// <summary>
/// 评论
/// </summary>
public partial class Comment
{
    /// <summary>
    /// ID
    /// </summary>
    public long CommentId { get; set; }

    /// <summary>
    /// 用户ID
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// 目标ID
    /// </summary>
    public long TId { get; set; }

    /// <summary>
    /// 评论类型
    /// </summary>
    public int TypeId { get; set; }

    /// <summary>
    /// 评论内容
    /// </summary>
    public string Content { get; set; } = null!;

    /// <summary>
    /// 评论状态
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateDate { get; set; }

    /// <summary>
    /// 修改时间
    /// </summary>
    public DateTime UpdateDate { get; set; }

    public virtual User User { get; set; } = null!;
}
