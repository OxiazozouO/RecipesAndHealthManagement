using Android.Attribute;
using Android.Component;
using Android.Helper;
using Android.Models;
using Android.ViewModel;
using Android.Views;
using AnyLibrary.Helper;
using MikePhil.Charting.Charts;
using MikePhil.Charting.Data;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;
using static Android.Views.ViewStates;

namespace Android.Holder;

[ViewClassBind(Layout.page_diary)]
public class PageDiaryHolder(App.Activity activity) : ViewHolder<View>(activity)
{
    [ViewHolderBind(Id.id_diary_data_pake_bar)]
    public DataPackedBarHolder DataBar;

    [ViewHolderBind(Id.id_diary_data_pake_content)]
    public DataPackedContentHolder DataPacked;

    [ViewBind(Id.id_diary_view_list)] public LinearLayout ViewList;

    [ViewBind(Id.id_diary_energy_dotted_line_1)]
    public View DottedLine1;

    [ViewBind(Id.id_diary_energy_dotted_line_2)]
    public View DottedLine2;

    [ViewBind(Id.id_diary_energy_dotted_line_3)]
    public View DottedLine3;

    [ViewHolderBind(Id.id_diary_energy_progress)]
    public ProgressBarHolder Progress;

    [ViewBind(Id.id_diary_ree)] public TextView Ree;
    [ViewBind(Id.id_diary_tdee)] public TextView Tdee;

    [ViewBind(Id.id_diary_max_tdee)] public TextView MaxTdee;

    [ViewBind(Id.id_diary_now_tdee)] public TextView NowTdee;
    [ViewBind(Id.id_diary_nutrient_list)] public LinearLayout NutrientList;

    [ViewBind(Id.id_diary_bnt1)] public TextView Bnt1;
    [ViewBind(Id.id_diary_bnt2)] public TextView Bnt2;

    [ViewBind(Id.id_diary_pie_chart_ent)] public PieChart PieChartEnt;

    [ViewHolderBind(Id.id_diary_nutrient_chart)]
    public MacroNutrientChartHolder NutrientChart;
    
    
    private ViewStates flag1 = Visible;
    protected override void Init()
    {
        DataPacked.Init(activity, DataBar);
        var adapter = DataPacked.Adapter;

        DateTime _week = DateTime.Now;
        DateTime _today = DateTime.Now;
        EatingDiaryInfoViewModel _model = null;
        EatingDiaryBarViewModel _energy = null;

        adapter.OnItemClick += (sender, pos) =>
        {
            int sub = pos - ((int)(pos / 7)) * 7;
            adapter.GetDateTime(pos, out _, out var today);
            _today = today;
            var week = today.AddDays(-sub);
            _week = week;
            if (!adapter.Diaries.TryGetValue(week, out var diary)) return;
            if (!diary.EatingDiaryBar.TryGetValue(today, out var energy)) return;

            var models = (from e in diary.EatingDiaries
                where e.EatingDiary.TieUpDate.Date == today.Date
                select e).ToList();
            var model = new EatingDiaryInfoViewModel { Select = models };
            _model = model;
            _energy = energy;
            Bind(model, energy);
            Bnt1.Text = "单日分析";
        };

        bool flag = false;

        Bnt1.CallClick(() =>
        {
            flag = !flag;
            if (flag)
            {
                if (!adapter.Diaries.TryGetValue(_week, out var diary)) return;
                if (!diary.EatingDiaryBar.TryGetValue(_today, out var energy)) return;
                var models = diary.EatingDiaries;
                var model = new EatingDiaryInfoViewModel { Day = 7, Select = models };
                Bind(model, energy);
                Bnt1.Text = "本周分析";
            }
            else
            {
                Bind(_model, _energy);
                Bnt1.Text = "单日分析";
            }
        });
    }

    public void Bind(EatingDiaryInfoViewModel model, EatingDiaryBarViewModel energy)
    {
        var ear = (decimal)energy.TdeeGroup.Ree;
        var rni = (decimal)energy.TdeeGroup.Tdee;
        var ul = (decimal)energy.TdeeGroup.MaxTdee;
        var value = model.Energy;
        var maxValue = Math.Max(ul, value);
        var color = ColorHelper.GetColor(value, ear, rni, ul);
        Progress.SetColors(color.GetAndroidColor());
        Progress.SetProgress(value, maxValue);

        var width = Progress.Width;

        if (ear > 0.001m)
        {
            Update(DottedLine1, p => p.LeftMargin = -width + Progress.CalculationProgress(ear, maxValue));
        }
        else
        {
            DottedLine1.Visibility = Gone;
        }

        if (rni > 0.001m)
        {
            Update(DottedLine2, p => p.LeftMargin = -width + Progress.CalculationProgress(rni, maxValue));
        }
        else
        {
            DottedLine2.Visibility = Gone;
        }

        if (ul > 0.001m)
        {
            Update(DottedLine3, p => p.LeftMargin = -width + Progress.CalculationProgress(ul, maxValue));
            // Update(DottedLine3, p => p.LeftMargin = 0);
        }
        else
        {
            DottedLine3.Visibility = Gone;
        }

        Ree.Text = energy.TdeeGroup.Ree.ToKcalStr();
        Tdee.Text = energy.TdeeGroup.Tdee.ToKcalStr();
        MaxTdee.Text = energy.TdeeGroup.MaxTdee.ToKcalStr();
        NowTdee.Text = model.Energy.ToKcalStr();

        {
            var entries = new List<PieEntry>();
            var colors = new List<int>();
            foreach (var pro in model.ProteinNutrient)
            {
                entries.Add(new PieEntry(pro.Rate, pro.Str1));
                colors.Add(pro.Colors.GetAndroidColor()[0]);
            }

            var dataSet = new PieDataSet(entries, "") { Colors = colors };
            dataSet.SetDrawValues(false);
            PieChartEnt.Data = new PieData(dataSet);
            PieChartEnt.Init();
        }

        NutrientChart.Bind(activity, model.ProteinNutrient);

        ViewList.RemoveAllViews();
        NutrientList.RemoveAllViews();
        foreach (var diaryAt in model.Select)
        {
            var h = new ItemDiaryViewHolder(activity);
            h.Bind(DataPacked.Adapter, diaryAt);
            ViewList.AddView(h.Root);
        }

        int pos = 0;
        List<ItemDiaryNutrientHolder> hs = [];
        foreach (var nutrient in model.OtherNutrientContent)
        {
            var h = new ItemDiaryNutrientHolder(activity);
            var nn = nutrient;
            h.Bind(nn);
            h.Root.CallClick(() => { ActivityHelper.GotoDiaryNutrient(nn.ToJson()); });
            NutrientList.AddView(h.Root);
            pos++;
            if (pos > 4) hs.Add(h);
        }

        Bnt2.CallClick(() =>
        {
            var str = flag1 == Visible ? "⬤⬤⬤ 更多" : "收起";
            Bnt2.Text = str;
            flag1 = flag1 == Visible ? Gone : Visible;
            hs.ForEach(h => h.Root.Visibility = flag1);
        });

        Bnt2.Visibility = pos <= 4 ? Gone : Visible;
        Bnt2.CallOnClick();
    }

    private void Update(View view, Action<FrameLayout.LayoutParams> action)
    {
        var parameters = (FrameLayout.LayoutParams)view.LayoutParameters;
        action.Invoke(parameters);
        view.LayoutParameters = parameters;
    }
}