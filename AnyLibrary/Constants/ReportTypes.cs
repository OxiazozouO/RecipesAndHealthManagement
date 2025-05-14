namespace AnyLibrary.Constants;

public class ReportTypes
{
    // 违法内容举报
    public const int IllegalContent = 1;

    // 侵权举报
    public const int CopyrightInfringement = 2;

    // 骚扰举报
    public const int Harassment = 3;

    // 虚假信息举报
    public const int FalseInformation = 4;

    // 其他类型举报
    public const int Other = 5;

    public static string GetName(int id)
    {
        return id switch
        {
            IllegalContent => "违法内容",
            CopyrightInfringement => "侵权",
            Harassment => "骚扰",
            FalseInformation => "虚假信息",
            _ => "其他类型"
        };
    }
}