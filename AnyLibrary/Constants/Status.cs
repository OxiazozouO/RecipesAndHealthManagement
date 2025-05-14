namespace AnyLibrary.Constants;

public static class Status
{
    // 默认状态
    public const int Default = 0; // 无意义

    // 审核相关状态（区间：100-199）
    public const int Pending = 100; // 待审核
    public const int Approve = 101; // 批准
    public const int Reject = 102; // 驳回
    public const int Locked = 103; // 已锁定 只读

    // 上下架相关状态（区间：200-299）
    public const int On = 200; // 上架
    public const int Off = 201; // 下架
    public const int ForceOff = 202; // 强制下架
    public const int ReportOff = 203; // 举报下架


    // 操作相关状态（区间：300-399）
    public const int Deleted = 300; // 删除
    public const int NeedEdit = 301; // 需修改
    public const int Cancel = 302; // 取消
    public const int Confirm = 303; // 待确认

    // 其他状态（区间：400+）
    public const int All = 400; // 全部
    public const int Other = 401; // 其他

    //待审核
    public static bool IsPending(int id) => id is Pending or Confirm or NeedEdit;

    //通过
    public static bool IsApprove(int id) => id is Approve or On or Off;

    //维护中
    public static bool IsOther(int id) => id is Deleted or Cancel or Reject or Locked or ForceOff or ReportOff;
}