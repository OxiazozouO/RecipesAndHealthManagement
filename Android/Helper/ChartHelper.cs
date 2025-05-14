using MikePhil.Charting.Charts;

namespace Android.Helper;

public static class ChartHelper
{
    public static void Init(this PieChart chart)
    {
        chart.Description.Enabled = false;
        chart.SetDrawEntryLabels(false);
        chart.SetUsePercentValues(false);
        chart.Legend.Enabled = false;
        chart.Invalidate(); // 刷新图表
    }
}