using System.Globalization;
using Android.Attribute;
using Android.Views;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;

namespace Android.Component;

[ViewClassBind(Layout.component_data_paked_bar)]
public class DataPackedBarHolder(View view) : ViewHolder<View>(view)
{
    [ViewBind(Id.id_date_text_large)] public TextView DateTextLarge;

    [ViewBind(Id.id_year_text)] public TextView YearText;

    [ViewBind(Id.id_today_text)] public TextView TodayText;

    [ViewBind(Id.id_today_icon)] public TextView todayIcon;

    protected override void Init()
    {
        var dateTime = DateTime.Now;
        todayIcon.Text = dateTime.Day.ToString();
    }

    public void Bind(DateTime time)
    {
        CultureInfo chineseCulture = new CultureInfo("zh-CN");
        string weekDay = time.ToString("dddd", chineseCulture);
        YearText.Text = time.Year.ToString();
        TodayText.Text = weekDay;
        DateTextLarge.Text = time.ToString("MM月dd日");
    }
}