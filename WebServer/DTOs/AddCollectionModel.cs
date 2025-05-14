using System.ComponentModel.DataAnnotations;
using WebServer.Helper;

namespace WebServer.DTOs;

public record AddCollectionModel
{
    [Range(-1, int.MaxValue - 2, ErrorMessage = "请求错误")]
    public long CollectionId { get; set; }

    [Required(ErrorMessage = "必须有合集封面")]
    [StringLength(200, ErrorMessage = "上传合集封面错误")]
    public string FileUrl { set; get; }

    [Required(ErrorMessage = "必须有合集标题")]
    [StringLength(30, ErrorMessage = "合集标题长度不能超过{1}")]
    public string Title { set; get; }

    [StringLength(200, ErrorMessage = "合集简介长度不能超过{1}")]
    public string? Summary { set; get; }

    [Required(ErrorMessage = "必须有合集内容")] public HtmlData Content { set; get; }
}

public record HtmlData
{
    [Required(ErrorMessage = "必须插入有食谱/食材/合集")]
    [CollectionLength(1, int.MaxValue - 2, ErrorMessage = "至少添加{1}个食谱/食材/合集")]
    public Dictionary<sbyte, HashSet<long>> Dirs { set; get; }

    [Required(ErrorMessage = "必须插入有食谱/食材/合集")]
    public List<string> Images { set; get; }

    [Required(ErrorMessage = "必须写入合集内容")] public string Html { set; get; }
}