using System.ComponentModel.DataAnnotations;

namespace WebServer.DTOs;

public record AdminDto
{
    [Range(1, int.MaxValue - 2, ErrorMessage = "请求错误")]
    [Required(ErrorMessage = "管理员id不能为空")]
    public int AdminId { get; set; }
}

public record AdminUserDto : AdminDto
{
    [Required(ErrorMessage = "用户id不能为空")]
    [Range(1, long.MaxValue - 2, ErrorMessage = "请求错误")]
    public long Id { get; set; }
}