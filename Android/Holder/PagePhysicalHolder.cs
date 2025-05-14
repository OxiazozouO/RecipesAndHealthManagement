using _Microsoft.Android.Resource.Designer;
using Android.Attribute;
using Android.Helper;
using Android.HttpClients;
using Android.Models;
using Android.Views;
using AndroidX.CardView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using AnyLibrary.Helper;
using MikePhil.Charting.Charts;
using MikePhil.Charting.Components;
using MikePhil.Charting.Data;
using MikePhil.Charting.Formatter;
using MikePhil.Charting.Interfaces.Datasets;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;
using static Android.Text.InputTypes;
using Color = Android.Graphics.Color;

namespace Android.Holder;

[ViewClassBind(Layout.page_physical)]
public class PagePhysicalHolder(App.Activity activity) : ViewHolder<View>(activity)
{
    [ViewBind(Id.id_physical_chart_style)] public LinearLayout ChartStyle;

    [ViewBind(Id.id_physical_chart_style_text)]
    public TextView ChartStyleText;

    [ViewBind(Id.id_physical_swipe_refresh)]
    public SwipeRefreshLayout Swipe;

    [ViewBind(Id.id_physical_bmi_card_view)]
    public CardView Bmi;

    [ViewBind(Id.id_physical_sex_text)] public TextView Sex;

    [ViewBind(Id.id_physical_age_text)] public TextView Age;

    [ViewBind(Id.id_physical_bmi_tag_text)]
    public TextView BmiTagText;

    [ViewBind(Id.id_physical_bmi_text)] public TextView BmiText;

    [ViewBind(Id.id_physical_bmi_bar)] public LinearLayout BmiBar;
    [ViewBind(Id.id_physical_bmi_bar_p)] public View BmiBarP;

    [ViewBind(Id.id_physical_bmi_bar_pos)] public TextView BmiBarPos;

    [ViewBind(Id.id_physical_bmi_bar_a)] public View BmiBarA;

    [ViewBind(Id.id_physical_bmi_bar_b)] public View BmiBarB;

    [ViewBind(Id.id_physical_bmi_bar_c)] public View BmiBarC;

    [ViewBind(Id.id_physical_bmi_bar_d)] public View BmiBarD;

    [ViewBind(Id.id_physical_height_text)] public TextView HeightText;

    [ViewBind(Id.id_physical_height_add)] public ImageView HeightAdd;

    [ViewBind(Id.id_physical_height_chart)]
    public LineChart HeightChart;

    [ViewBind(Id.id_physical_weight_text)] public TextView WeightText;

    [ViewBind(Id.id_physical_weight_add)] public ImageView WeightAdd;

    [ViewBind(Id.id_physical_weight_chart)]
    public LineChart WeightChart;

    [ViewBind(Id.id_physical_activity_level_text)]
    public TextView ActivityLevelText;

    [ViewBind(Id.id_physical_activity_level_add)]
    public ImageView ActivityLevelAdd;

    [ViewBind(Id.id_physical_activity_level_chart)]
    public LineChart ActivityLevelChart;

    [ViewBind(Id.id_physical_nutrient_text)]
    public TextView NutrientText;

    [ViewBind(Id.id_physical_nutrient_add)]
    public ImageView NutrientAdd;

    [ViewBind(Id.id_physical_nutrient_chart)]
    public LineChart NutrientChart;

    [ViewBind(Id.id_physical_energy_text)] public TextView EnergyText;

    [ViewBind(Id.id_physical_energy_chart)]
    public LineChart EnergyChart;


    private List<Entry> HeightEntries = [];
    private List<Entry> WeightEntries = [];
    private List<Entry> ActivityLevels = [];
    private List<List<Entry>> Nutrients = [];
    private List<List<Entry>> Energys = [];

    private List<Color> _colors =
    [
        Color.ParseColor("#00DCBC"),
        Color.ParseColor("#7AFF0C"),
        Color.ParseColor("#FFDA0C"),
        Color.ParseColor("#FF5707")
    ];

    private void UpdateBmi()
    {
        var model = AppConfigHelper.MyInfo;
        Sex.Text = model.Gender ? "男" : "女";
        Age.Text = model.BirthDate.GetAge().ToString();
        if (model?.Physical is not null)
            Bind(model);
    }

    protected override void Init()
    {
        Bmi.CallClick(ActivityHelper.GotoUserInfo);
        AppConfigHelper.AppConfigChanged += (sender, args) => { UpdateBmi(); };
        UpdateBmi();

        InitChart(HeightChart);
        InitChart(WeightChart);
        InitChart(ActivityLevelChart);
        InitChart(NutrientChart);
        InitChart(EnergyChart);

        BmiBarA.SetBackgroundColor(_colors[0]);
        BmiBarB.SetBackgroundColor(_colors[1]);
        BmiBarC.SetBackgroundColor(_colors[2]);
        BmiBarD.SetBackgroundColor(_colors[3]);
        Swipe.BindRefresh(Refresh);
        Refresh();

        ChartStyle.CallClick(() =>
        {
            var list = new List<MsgItem>
            {
                new() { Text = "周视图" },
                new() { Text = "月视图" },
                new() { Text = "年视图" }
            };
            var item = list.FirstOrDefault(l => l.Text == ChartStyleText.Text);
            var ind = list.IndexOf(item);
            if (ind == -1) ind = 0;

            MsgBoxHelper.Builder()
                .AddLisView(list, ind)
                .Show(l =>
                {
                    Flag = (XAxisFlags)l[0];
                    ChartStyleText.Text = list[(int)Flag].Text;
                    Bind(AppConfigHelper.MyInfo);
                });
        });

        HeightAdd.CallClick(() =>
        {
            model.InitPhysical();
            MsgBoxHelper
                .Builder()
                .AddEditText($"{model.Physical.Height:#.00}", ClassNumber | NumberFlagDecimal, 6, "请输入身高(120~255)(cm)")
                .Show(list =>
                {
                    try
                    {
                        var height = double.Parse((string)list[0]);
                        model.Physical.Height = height;
                        Add();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                });
        });

        WeightAdd.CallClick(() =>
        {
            model.InitPhysical();
            MsgBoxHelper
                .Builder()
                .AddEditText($"{model.Physical.Weight * 2:#.00}", ClassNumber | NumberFlagDecimal, 6,
                    "请输入体重(60~510)(公斤)")
                .Show(list =>
                {
                    try
                    {
                        var weight = double.Parse((string)list[0]);
                        model.Physical.Weight = weight / 2;
                        Add();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                });
        });

        ActivityLevelAdd.CallClick(() =>
        {
            model.InitPhysical();
            var levels = AppConfigHelper
                .ModelConfig
                .ActivityLevels
                .Values
                .ToList()
                .OrderBy(i => i.Value)
                .ToList();
            var list = levels
                .Select(a => new MsgItem { Text = $"{a.Value:#.00} {a.Name}" })
                .ToList();
            var item = levels.FirstOrDefault(v => v.Id == model.Physical.ActivityLevel.Id);
            int pos = 0;
            if (item is not null)
                pos = levels.IndexOf(item);
            MsgBoxHelper
                .Builder()
                .AddLisView(list, pos)
                .Show(l =>
                {
                    try
                    {
                        model.Physical.ActivityLevel = levels[(int)l[0]];
                        Add();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                });
        });

        NutrientAdd.CallClick(() =>
        {
            MsgBoxHelper
                .Builder()
                .AddEditNutrient()
                .Show(l => { Add(); });
        });
    }

    private void Refresh()
    {
        var model = AppConfigHelper.MyInfo;
        model.PhysicalList = ApiService.GetMyAllInfo();
        Bind(model);
    }


    public double BmiBarp
    {
        set
        {
            int w = BmiBar.LayoutParameters.Width;
            double p = value switch
            {
                < 12.5 => 0,
                > 33.9 => w,
                _ => (10 * value - 125) * w / 214
            };
            BmiBarP.ChangeSize((int)p, null);

            BmiTagText.Text = NutritionalHelper.GetBmiString(value);
            var color = value switch
            {
                < 18.5 => _colors[0],
                <= 23.9 => _colors[1],
                <= 27.9 => _colors[2],
                _ => _colors[3]
            };

            BmiTagText.SetBackgroundColor(color);
            BmiBarPos.SetTextColor(color);
            BmiText.Text = $"{value:#.00} kg/m²";
        }
    }

    public static void InitChart(LineChart chart)
    {
        chart.SetDrawGridBackground(false);
        chart.Description.Enabled = false;
        chart.SetTouchEnabled(true);
        chart.DragEnabled = true;
        chart.SetScaleEnabled(false);
        chart.SetPinchZoom(true);
        chart.AxisRight.Enabled = false;
        chart.ExtraLeftOffset = 6f;
        chart.ExtraRightOffset = 6f;
        var leftAxis = chart.AxisLeft;
        leftAxis.SetDrawGridLines(true);
        leftAxis.SetDrawZeroLine(false);
        leftAxis.EnableGridDashedLine(10f, 10f, 0f);
        leftAxis.GridColor = Color.Gray;
        var xAxis = chart.XAxis;
        xAxis.Position = XAxis.XAxisPosition.Bottom;
        xAxis.ValueFormatter = new DateValueFormatter();
    }

    public void InitSet(LineChart chart, List<List<Entry>> entries, List<int[]> colors, List<string> titles,
        bool isItem3 = false)
    {
        List<ILineDataSet> dataSets = [];
        colors = colors.Select(c => c.GetAndroidColor()).ToList();
        var week = DataTimeHelper.MondayOfThisWeek();
        var time = DateOnly.FromDateTime(week);
        for (var i = 0; i < entries.Count; i++)
        {
            var title = titles[i];
            var entry = entries[i];
            if (entry.Count == 0) continue;
            var color = colors[i];
            var last = entry.Last();
            var x = (int)last.GetX();
            if (x < time.DayNumber)
            {
                entry.Add(new Entry(time.DayNumber, entry[^1].GetY(), entry[^1].Data));
            }

            var dataSet = new LineDataSet(entry, title);
            dataSets.Add(dataSet);

            dataSet.Color = color[0];
            dataSet.SetCircleColor(color[1]);
            dataSet.LineWidth = 2f;
            dataSet.CircleRadius = 4f;
            dataSet.SetDrawValues(true);
            dataSet.ValueTextSize = 14f;
            dataSet.SetValueTextColor(Color.Black);
        }

        switch (Flag)
        {
            case XAxisFlags.Week:
            {
                var xAxis = chart.XAxis;
                xAxis.LabelCount = 7;
                xAxis.Granularity = time.AddDays(1).DayNumber - time.DayNumber;
                xAxis.AxisMinimum = time.DayNumber;
                xAxis.AxisMaximum = DateOnly.FromDateTime(week.AddDays(7)).DayNumber;
                if (isItem3)
                    dataSets.ForEach(d => ((LineDataSet)d).ValueFormatter = new StringValueFormatter());
                else dataSets.ForEach(d => ((LineDataSet)d).ValueFormatter = new FloatValueFormatter());
                dataSets.ForEach(d => d.SetDrawValues(true));
                break;
            }
            case XAxisFlags.Month:
            {
                var xAxis = chart.XAxis;
                xAxis.LabelCount = 12;
                xAxis.Granularity = time.AddMonths(1).DayNumber - time.DayNumber;

                xAxis.AxisMinimum = DateOnly.FromDateTime(week.AddMonths(-6)).DayNumber;
                xAxis.AxisMaximum = DateOnly.FromDateTime(week.AddMonths(6)).DayNumber;
                dataSets.ForEach(d => d.SetDrawValues(false));
                dataSets.ForEach(d => ((LineDataSet)d).ValueFormatter = new FloatValueFormatter());
                break;
            }
            case XAxisFlags.Year:
            {
                var xAxis = chart.XAxis;
                xAxis.LabelCount = 12;
                xAxis.Granularity = time.AddYears(1).DayNumber - time.DayNumber;
                xAxis.AxisMinimum = DateOnly.FromDateTime(week.AddYears(-6)).DayNumber;
                xAxis.AxisMaximum = DateOnly.FromDateTime(week.AddYears(6)).DayNumber;
                dataSets.ForEach(d => d.SetDrawValues(false));
                dataSets.ForEach(d => ((LineDataSet)d).ValueFormatter = new FloatValueFormatter());
                break;
            }
        }


        chart.Data = new LineData(dataSets);
        chart.Invalidate();
    }

    public XAxisFlags Flag = XAxisFlags.Week;

    public enum XAxisFlags
    {
        Week = 0,
        Month = 1,
        Year = 2,
    }

    private MyInfoModel model;

    public void Bind(MyInfoModel model)
    {
        this.model = model;
        HeightText.Text = $"身高: {model.Physical.Height:#.00} 厘米";
        WeightText.Text = $"体重: {model.Physical.Weight * 2:#.00} 公斤";
        ActivityLevelText.Text = $"劳动强度: {model.Physical.ActivityLevel.Value:#.00} {model.Physical.ActivityLevel.Name}";
        NutrientText.Text = $"每日总能量消耗: {model.Physical.TdeeGroup.Tdee:#.00} kcal/天";
        EnergyText.Text = "";

        HeightEntries.Clear();
        WeightEntries.Clear();
        ActivityLevels.Clear();
        Nutrients.Clear();
        Energys.Clear();

        HeightEntries.AddRange(model.PhysicalList.Select(a => new Entry(a.DayNumber,
            (float)a.Height)));
        WeightEntries.AddRange(model.PhysicalList.Select(a => new Entry(a.DayNumber,
            (float)a.Weight * 2)));
        ActivityLevels.AddRange(model.PhysicalList.Select(a => new Entry(a.DayNumber,
            (float)a.ActivityLevel.Value)));

        Nutrients.Add(model.PhysicalList.Select(a => new Entry(a.DayNumber,
                (float)(a.TdeeGroup.Tdee * a.CarbohydrateRequirement), $"{a.CarbohydrateRequirement:#.00} g"))
            .ToList());
        Nutrients.Add(model.PhysicalList.Select(a => new Entry(a.DayNumber,
            (float)(a.TdeeGroup.Tdee * a.FatRequirement), $"{a.FatRequirement:#.00} g")).ToList());
        Nutrients.Add(model.PhysicalList.Select(a => new Entry(a.DayNumber,
            (float)(a.TdeeGroup.Tdee * a.ProteinRequirement), $"{a.ProteinRequirement:#.00} g")).ToList());

        Energys.Add(model.PhysicalList.Select(a => new Entry(a.DayNumber, (float)a.TdeeGroup.Ree,
            a.TdeeGroup.Ree.ToKcalStr())).ToList());
        Energys.Add(model.PhysicalList.Select(a => new Entry(a.DayNumber, (float)a.TdeeGroup.Tdee,
            a.TdeeGroup.Tdee.ToKcalStr())).ToList());
        Energys.Add(model.PhysicalList.Select(a => new Entry(a.DayNumber, (float)a.TdeeGroup.MaxTdee,
            a.TdeeGroup.MaxTdee.ToKcalStr())).ToList());

        InitSet(HeightChart, [HeightEntries], [ColorHelper.H], ["cm"]);
        InitSet(WeightChart, [WeightEntries], [ColorHelper.H], ["kg"]);
        InitSet(ActivityLevelChart, [ActivityLevels], [ColorHelper.H], [""]);
        InitSet(NutrientChart, Nutrients,
            [ColorHelper.H, ColorHelper.Purple, ColorHelper.Blue, ColorHelper.Orange],
            ["基础代谢", "碳水化合物", "脂肪", "蛋白质"],
            true);
        InitSet(EnergyChart, Energys, [ColorHelper.K, ColorHelper.Q, ColorHelper.OrangeRed],
            ["基础代谢", "每日能量总耗", "建议最大摄入量"], true);
        BmiBarp = (double)model.Physical.TdeeGroup.Bmi;
    }

    public class DateValueFormatter : ValueFormatter
    {
        public override string GetAxisLabel(float value, AxisBase axis) =>
            DateOnly.FromDayNumber((int)value).ToString("M.d");

        public override string GetFormattedValue(float value) =>
            DateOnly.FromDayNumber((int)value).ToString("M.d");
    }

    public class StringValueFormatter : ValueFormatter
    {
        public override string GetPointLabel(Entry entry) => entry.Data.ToString();
    }

    public class FloatValueFormatter : ValueFormatter
    {
        public override string GetPointLabel(Entry entry) => $"{entry.GetY():#.00}";
    }

    public void Add()
    {
        var physical = model.Physical;
        physical.Bind(model);
        ApiEndpoints.AddMyInfo(new
        {
            Id = AppConfigHelper.AppConfig.Id,
            Height = physical.Height,
            Weight = physical.Weight,
            CalId = physical.ActivityLevel.Id,
            ProteinPercentage = physical.ProteinPercentage,
            FatPercentage = physical.FatPercentage
        }).Execute(res => { Refresh(); });
    }
}