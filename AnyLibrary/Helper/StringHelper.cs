using System.Text;

namespace AnyLibrary.Helper;

public static class StringHelper
{
    public static string GetRandomString()
    {
        return new StringBuilder()
            .Append(DateTime.UtcNow.ToString("yyyyMMddHHmmssfff"))
            .Append(Guid.NewGuid().ToString().Replace("-", ""))
            .ToString();
    }

    public static string ShortStr(this decimal d) => $"{d:0.####}";


    public static string Convert(this TimeSpan span, string? fmt = null)
    {
        var (day, h, min, s) = ("天", "小时", "分", "秒");

        var ret = "";
        // @formatter:off
        if (span.Days != 0)
            ret = $"{span.Days} {day} {span.Hours} {h} {span.Minutes} {min} {span.Seconds} {s}";
        else if (span.Hours != 0)
            ret =                   $"{span.Hours} {h} {span.Minutes} {min} {span.Seconds} {s}";
        else if (span.Minutes != 0)
            ret =                                    $"{span.Minutes} {min} {span.Seconds} {s}";
        else if (span.Seconds != 0)
            ret =                                                         $"{span.Seconds} {s}";
        return ret;
        // @formatter:on
    }

    public static string TimeStr1(this DateTime dateTime) => dateTime.ToString("yyyy-MM-dd HH:mm:ss");
    public static string TimeStr2(this DateTime dateTime) => dateTime.ToString("yyyy-MM-dd");

    public static string ToHexColor(this int color)
    {
        byte r = (byte)((color >> 16) & 0xFF);
        byte g = (byte)((color >> 8) & 0xFF);
        byte b = (byte)(color & 0xFF);
        return $"#{r:X2}{g:X2}{b:X2}";
    }

    public static string ToKcalStr(this decimal str) => $"{str:0.00} kcal/天";
    public static string ToKcalStr(this double str) => $"{str:0.00} kcal/天";
    
    
    
    public static string FormatFileSize(long bytes)
    {
        const int scale = 1024;
        string[] suffixes = ["B", "KB", "MB", "GB", "TB"];
        if (bytes == 0) return "0 B";
        int order = (int)Math.Log(bytes, scale);
        double adjustedSize = bytes / Math.Pow(scale, order);
        return $"{adjustedSize:0.##} {suffixes[order]}";
    }
}
   // Pending// 待审核
   // Approve// 批准
   // Reject// 驳回
   // Locked// 已锁定 只读
   // On// 上架
   // Off// 下架
   // ForceOff// 强制下架
   // ReportOff// 举报下架
   // Deleted// 删除
   // NeedEdit// 需修改
   // Cancel// 取消
   // Confirm// 待确认
   //  用户端：
   //     [新建Pending] => [修改后Pending, Deleted],
   //     [NeedEdit] => [修改后Pending, Deleted],
   //     [On] => [修改后Pending,Off, Deleted],
   //     [Off] => [修改后Pending, On,  Deleted],
   //     [ForceOff,ReportOff] => Deleted,
   // 审核员端：
   //     [Pending, Approve,Deleted, NeedEdit, Cancel,Confirm, Reject,Locked] => '查看', 查看并不是状态
   //     [Pending] => Confirm,
   //     [Pending, Approve] => NeedEdit, 实际上Locked原来的，然后新建一个审核，并且状态为NeedEdit
   //     [NeedEdit] => Pending,
   //     [Pending, NeedEdit] => Reject
   //     系统管理员端：
   //     [Pending, Approve,Deleted, NeedEdit,Cancel, Confirm,Reject, Locked] => '查看'  查看并不是状态
   //     [Confirm] => Approve,
   //     [Approve, Confirm] => NeedEdit, 实际上Locked原来的，然后新建一个审核，并且状态为NeedEdit
   //     [Confirm] => Reject,
   //     [Cancel, Reject] => Deleted
   //     [On] => [修改后Pending,ForceOff, Deleted],
   //     [Off,ForceOff] => [On]
   