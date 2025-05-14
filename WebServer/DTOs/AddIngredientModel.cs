using System.ComponentModel.DataAnnotations;

namespace WebServer.DTOs;

public record AddIngredientModel
{
    [Required(ErrorMessage = "食材id不能为空")]
    [Range(-1, long.MaxValue - 2, ErrorMessage = "请求错误")]
    public long IngredientId { get; set; }

    [StringLength(300, ErrorMessage = "多媒体文件路径长度不能超过60")]
    public string? FileUrl { get; set; }

    [Required(ErrorMessage = "名称是必需的")]
    [StringLength(30, ErrorMessage = "名称长度不能超过30")]
    public string IName { get; set; }

    [StringLength(200, ErrorMessage = "描述长度不能超过200")]
    public string? Refer { get; set; }

    [StringLength(10, ErrorMessage = "单位长度不能超过10")]
    public string Unit { get; set; }

    public Dictionary<string, decimal> Quantity { get; set; }

    [StringLength(200, ErrorMessage = "过敏信息长度不能超过200")]
    public string? Allergy { get; set; }

    [Required(ErrorMessage = "净含量是必需的")] public double Content { get; set; }

    public Dictionary<int, double> Nutrients { get; set; }
}