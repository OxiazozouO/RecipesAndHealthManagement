using Android.Attribute;
using Android.Component;
using Android.Helper;
using Android.Models;
using Android.Views;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;

namespace Android.Holder;

[ViewClassBind(Layout.item_ent_date)] // 将your_layout_name替换为实际对应的布局文件名
public class ItemEntDateHolder(View view) : ViewHolder<View>(view)
{
    [ViewBind(Id.id_date_num)] public TextView DateNum;

    [ViewHolderBind(Id.id_date_progress_bar)]
    public ProgressBarHolder ProgressHolder;

    [ViewBind(Id.id_dotted_line_1)] public View DottedLine1;

    [ViewBind(Id.id_dotted_line_2)] public View DottedLine2;

    [ViewBind(Id.id_dotted_line_3)] public View DottedLine3;

    protected override void Init()
    {
    }

    public static void UpdateParameters(View view, Action<FrameLayout.LayoutParams> action)
    {
        var parameters = (FrameLayout.LayoutParams)view.LayoutParameters;
        action.Invoke(parameters);
        view.LayoutParameters = parameters;
    }

    public void Bind(DateTime time, decimal max, EatingDiaryBarViewModel model)
    {
        DateNum.Text = time.Day.ToString();
        DateNum.Tag = time.Ticks;
        var group = model.TdeeGroup;

        if (max < 0.00001m) max = (decimal)Math.Max(Math.Max(group.Ree, group.Tdee), group.MaxTdee);

        ProgressHolder.SetProgress(model.Energy, max);

        UpdateParameters(DottedLine1,
            p => p.TopMargin = ProgressHolder.CalculationProgress((decimal)group.Ree, max));
        UpdateParameters(DottedLine2,
            p => p.TopMargin = ProgressHolder.CalculationProgress((decimal)group.Tdee, max));
        UpdateParameters(DottedLine3,
            p => p.TopMargin = ProgressHolder.CalculationProgress((decimal)group.MaxTdee, max));

        ProgressHolder.SetColors(ColorHelper
            .GetColor(model.Energy, (decimal)group.Ree, (decimal)group.Tdee, (decimal)group.MaxTdee));
    }
}

[ViewClassBind(Layout.linear_item_ent_date)]
public class LinearEntDateHolder(App.Activity activity) : ViewHolder<View>(activity)
{
    [ViewHolderArrayBind([
        Id.id_item_ent_date_1,
        Id.id_item_ent_date_2,
        Id.id_item_ent_date_3,
        Id.id_item_ent_date_4,
        Id.id_item_ent_date_5,
        Id.id_item_ent_date_6,
        Id.id_item_ent_date_7
    ])]
    public List<ItemEntDateHolder> Holders;

    public void AddRange(List<ItemEntDateHolder> models, int pos)
    {
        for (int i = 0; i < 7; i++) models[pos + i] = Holders[i];
    }

    protected override void Init()
    {
    }
}