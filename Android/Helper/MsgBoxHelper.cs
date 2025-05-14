using Android.Content;
using Android.Holder;
using Android.HttpClients;
using Android.Text;
using Android.Views;

namespace Android.Helper;

public class MsgBoxHelper
{
    public static void ShowToast(string message = "", ToastLength length = ToastLength.Short) =>
        Toast.MakeText(ActivityHelper.CurrentActivity, message, length).Show();

    public static void Error(string? message) =>
        Builder().TryError(message);

    public static void ShowDialog(ApiResponses res) =>
        Builder("", res.Message).ShowDialog();

    public static MsgBoxHelper Builder(string message = "", string caption = "")
    {
        return new MsgBoxHelper().Create(message, caption);
    }

    private AlertDialog.Builder? builder = null;

    private MsgBoxHelper Create(string message, string caption)
    {
        try
        {
            builder = new AlertDialog.Builder(ActivityHelper.CurrentActivity)
                ?.SetTitle(caption)
                ?.SetMessage(message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return this;
    }

    private static void Handler(object? sender, DialogClickEventArgs e)
    {
    }

    public MsgBoxHelper AddDatePicker(DateTime date)
    {
        var datePicker = new DatePicker(ActivityHelper.CurrentActivity);
        datePicker.DateTime = date.Date.ToUniversalTime();
        datePicker.MinDate = (long)DateTime.Now.AddYears(-99).ToUniversalTime()
            .Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        datePicker.MaxDate = (long)DateTime.Now.AddYears(99).ToUniversalTime()
            .Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        Funcs.Add(() => datePicker.DateTime.Date.ToLocalTime());
        return TryAddView(datePicker);
    }

    public MsgBoxHelper AddTimePicker()
    {
        var timePicker = new MsgTimeHolder(ActivityHelper.CurrentActivity);
        timePicker.Root.Is24HourView();
        DateTime date = DateTime.Now;
        timePicker.Root.Hour = date.Hour;
        timePicker.Root.Minute = date.Minute;
        Funcs.Add(() => new TimeSpan(timePicker.Root.Hour, timePicker.Root.Minute, 0));
        return TryAddView(timePicker.Root);
    }

    public List<Func<object>> Funcs = [];
    private LinearLayout Layout;

    public MsgBoxHelper AddEditText(string? text, InputTypes inputType, int maxLen, string tip = "")
    {
        var holder = new ItemLayoutMsgEditHolder(ActivityHelper.CurrentActivity);
        holder.MsgText.Text = tip;
        holder.MsgEdit.Text = text;
        holder.MsgEdit.InputType = inputType | InputTypes.TextFlagMultiLine;
        holder.MsgEdit.SetFilters([new InputFilterLengthFilter(maxLen)]);
        Funcs.Add(() => holder.MsgEdit.Text);
        return TryAddView(holder.Root);
    }

    private MsgBoxHelper TryAddView(View view)
    {
        if (Layout == null)
        {
            Layout = new LinearLayout(ActivityHelper.CurrentActivity);
            Layout.Orientation = Orientation.Vertical;
            builder?.SetView(Layout);
        }

        Layout.AddView(view);
        return this;
    }

    public MsgBoxHelper AddLisView(List<MsgItem> items, int pos)
    {
        var msgHolder = new LayoutMsgHolder(ActivityHelper.CurrentActivity);
        if (items.Count > 7)
        {
            msgHolder.Root.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                1000);
        }

        msgHolder.Bind(items);
        msgHolder.CallClick(pos);
        Funcs.Add(() => msgHolder.SelectedItem);

        return TryAddView(msgHolder.Root);
    }

    public MsgBoxHelper AddEditNutrient()
    {
        Builder("供能占比调整", "供能占比调整");
        var holder = new MsgEditNutrientHelper(ActivityHelper.CurrentActivity);
        Funcs.Add(() => "");
        return TryAddView(holder.Root);
    }

    public void OkCancel(Action action, Action? cancel = null)
    {
        builder
            ?.SetPositiveButton("确定", (_, _) => { action(); })
            ?.SetNegativeButton("取消", (_, _) => { cancel?.Invoke(); })
            ?.Create()
            ?.Show();
    }

    public void ShowDialog()
    {
        builder
            ?.SetPositiveButton("确定", Handler)
            ?.Create()
            ?.Show();
    }

    public void Show(Action<List<object>> action)
    {
        builder
            ?.SetPositiveButton("确认", (_, _) =>
            {
                try
                {
                    action.Invoke(Funcs.Select(func => func.Invoke()).ToList());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            })
            ?.SetNegativeButton("取消", Handler)
            ?.Create()
            ?.Show();
    }

    // 尝试显示错误类型对话框，如果消息为空则返回false
    public bool TryError(string? message)
    {
        if (string.IsNullOrEmpty(message)) return false;
        builder
            ?.SetMessage(message)
            ?.SetPositiveButton("确定", Handler)
            ?.Create()
            ?.Show();
        return true;
    }
}