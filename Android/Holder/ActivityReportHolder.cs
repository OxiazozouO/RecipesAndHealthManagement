using Android.Attribute;
using Android.Helper;
using Android.Models;
using Android.Views;
using AnyLibrary.Constants;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;

namespace Android.Holder;

[ViewClassBind(Layout.activity_report)]
public class ActivityReportHolder(App.Activity activity) : ViewHolder<View>(activity)
{
    [ViewBind(Id.id_report_title)] public TextView Title;

    [ViewBind(Id.id_report_main)] public LinearLayout Main;
    [ViewBind(Id.id_report_text)] public TextView Text;
    [ViewBind(Id.id_report_tip)] public TextView Tip;
    [ViewBind(Id.id_report_sub)] public TextView Sub;

    private ItemReportHolder select = null;

    public ReportModel model = new ReportModel();

    protected override void Init()
    {
        var list = new List<int>
        {
            ReportTypes.IllegalContent,
            ReportTypes.CopyrightInfringement,
            ReportTypes.Harassment,
            ReportTypes.FalseInformation,
            ReportTypes.Other
        }.Select(i => (i, ReportTypes.GetName(i))).ToList();

        foreach (var (i, item2) in list)
        {
            var item = new ItemReportHolder(activity);
            item.Bind(i, item2);
            item.Root.CallClick(() =>
            {
                select?.Img.SetImageResource(Drawable.shape_button_bg5);
                select = item;
                model.Status = i;
                select.Img.SetImageResource(Drawable.shape_button_bg);
            });
            Main.AddView(item.Root);
        }

        Text.BindTo(s =>
        {
            s ??= "";
            if (s.Length > 100)
            {
                Text.Text = s[..100];
                model.Content = Text.Text;
                return;
            }

            model.Content = s;
            Tip.Text = $"{s.Length}/100";
        });

        Sub.CallClick(() => { model.AddReport(activity); });
    }

    public void Bind(long tid, int category, string name)
    {
        Title.Text = $"举报[{name}]";
        model.Tid = tid;
        model.Category = (sbyte)category;
    }
}