using _Microsoft.Android.Resource.Designer;
using Android.Attribute;
using Android.Component;
using Android.Helper;
using Android.Models;
using Android.Util;
using Android.ViewModel;
using Android.Views;
using AnyLibrary.Constants;
using AnyLibrary.Helper;
using Bumptech.Glide;
using Google.Android.Flexbox;
using MikePhil.Charting.Charts;
using MikePhil.Charting.Data;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;
using static Android.Text.InputTypes;
using Color = Android.Graphics.Color;

namespace Android.Holder;

[ViewClassBind(ResourceConstant.Layout.activity_ingredient)]
public class ActivityIngredientHolder(App.Activity activity) : ViewHolder<ScrollView>(activity)
{
    [ViewBind(Id.id_ingredient_img)] public ImageView Img;

    [ViewBind(Id.id_ingredient_name)] public TextView Name;

    [ViewBind(Id.id_ingredient_ref)] public TextView Ref;

    [ViewBind(Id.id_ingredient_emoji_layout)]
    public FlexboxLayout EmojiLayout;

    [ViewBind(Id.id_ingredient_category_layout)]
    public FlexboxLayout CategoryLayout;

    [ViewBind(Id.id_ingredient_add_emoji)] public ImageView AddEmoji;

    [ViewBind(Id.id_ingredient_add_category)]
    public ImageView AddCategory;

    [ViewBind(Id.id_ingredient_negative_layout)]
    public FlexboxLayout NegativeLayout;

    [ViewBind(Id.id_ingredient_positive_layout)]
    public FlexboxLayout PositiveLayout;

    [ViewBind(Id.id_ingredient_allergy_layout)]
    public FlexboxLayout AllergyLayout;

    [ViewBind(Id.id_ingredient_dosage)] public TextView Dosage;

    [ViewBind(Id.id_ingredient_input_layout)]
    public LinearLayout Layout;

    [ViewBind(Id.id_ingredient_output_layout)]
    public LinearLayout OutLayout;

    [ViewBind(Id.id_ingredient_out_dosage)]
    public TextView OutDosage;

    [ViewBind(Id.id_ingredient_info_dosage)]
    public TextView EstimatedDosage;

    [ViewBind(Id.id_ingredient_refresh)] public ImageView Refresh;

    [ViewBind(Id.id_ingredient_add_to_physical)]
    public LinearLayout AddToPhysical;

    [ViewHolderBind(Id.id_ingredient_nutrient_chart)]
    public MacroNutrientChartHolder NutrientChart;

    [ViewBind(Id.id_pie_chart_ent)] public PieChart PieChartEnt;

    [ViewBind(Id.id_pie_chart_more)] public PieChart PieChartMore;

    [ViewBind(Id.id_ingredient_nutrient_list)]
    public LinearLayout NutrientList;

    [ViewBind(Id.id_ingredient_f1)] public TextView F1;
    [ViewBind(Id.id_ingredient_f2)] public TextView F2;
    [ViewBind(Id.id_ingredient_f3)] public TextView F3;
    [ViewBind(Id.id_ingredient_f4)] public TextView F4;


    [ViewHolderBind(Id.id_ingredient_comment)]
    public CommentHolder CommentHolder;

    [ViewBind(Id.id_ingredient_like)] public ImageView Like;

    [ViewBind(Id.id_ingredient_like_num)] public TextView LikeNum;

    [ViewBind(Id.id_ingredient_share)] public ImageView Share;

    [ViewBind(Id.id_ingredient_report)] public ImageView Report;


    protected override void Init()
    {
        AddEmoji.CallClick(() =>
        {
            MsgBoxHelper.Builder()
                .AddEditText("", ClassText, 12, "请输入表情")
                .Show(list => { model.AddCategory((string)list[0], CategoryType.Emoji, BindCategory); });
        });

        AddCategory.CallClick(() =>
        {
            MsgBoxHelper.Builder()
                .AddEditText("", ClassText, 12, "请输入分类名称")
                .Show(list => { model.AddCategory((string)list[0], CategoryType.Category, BindCategory); });
        });

        AddToPhysical.CallClick(() =>
        {
            MsgBoxHelper.Builder("请输入时间")
                .AddDatePicker(DateTime.Now)
                .AddTimePicker()
                .Show(list =>
                {
                    var data = (DateTime)list[0];
                    var time = (TimeSpan)list[1];
                    data = data.Add(time);
                    model.AddEatingDiary(data);
                });
        });

        Layout.CallClick(() =>
        {
            var ingredient = model.Ingredient;
            var items = UnitHelper.GetAllUnit(ingredient.Unit);
            var pos = items.IndexOf(ingredient.InputUnit);
            var msgItems = items.Select(x => new MsgItem { Text = x }).ToList();

            MsgBoxHelper.Builder()
                .AddEditText(ingredient.InputDosage.ShortStr(), ClassNumber | NumberFlagDecimal, 12, "请输入用量")
                .AddLisView(msgItems, pos)
                .Show(list =>
                {
                    ingredient.InputUnit = items[(int)list[1]];
                    var data = decimal.Parse((string)list[0]);
                    ingredient.InputDosage = data - 1;
                    ingredient.InputDosage += 1;
                    Update(model);
                });
        });

        OutLayout.CallClick(() =>
        {
            var ingredient = model.Ingredient;
            var items = ingredient.Quantity.Keys.ToList();
            var pos = items.IndexOf(ingredient.OutputUnit);
            var msgItems = items.Select(x => new MsgItem { Text = x }).ToList();

            MsgBoxHelper.Builder()
                .AddEditText(ingredient.OutputDosage.ShortStr(), ClassNumber | NumberFlagDecimal, 12, "请输入用量")
                .AddLisView(msgItems, pos)
                .Show(list =>
                {
                    ingredient.OutputUnit = items[(int)list[1]];
                    var data = decimal.Parse((string)list[0]);
                    ingredient.OutputDosage = data - 1;
                    ingredient.OutputDosage += 1;
                    Update(model);
                });
        });

        Refresh.CallClick(() =>
        {
            model.Ingredient.Reset();
            Update(model);
        });

        Share.CallClick(() =>
        {
            // ApiService.Share
        });

        Report.CallClick(() =>
        {
            ActivityHelper.GotoReport(model.Ingredient.IngredientId, IdCategory.Ingredient,
                model.Ingredient.IName);
        });
    }

    private IngredientViewModel model;

    public void Bind(IngredientViewModel model)
    {
        this.model = model;
        var ingredient = model.Ingredient;
        Glide.With(Root)
            .Load(ingredient.FileUrl)
            .Into(Img);
        Name.Text = ingredient.IName;
        Ref.Text = ingredient.Refer;

        CommentHolder.Bind(Root, model.Ingredient.IngredientId, IdCategory.Ingredient, activity);
        Update2(model);
        Update(model);
    }

    public void Update2(IngredientViewModel model)
    {
        LikeNum.Text = model.Ingredient.FavoriteCount.ToString();
        Like.SetImageResource(model.Ingredient.IsLike ? Drawable.ic_collect : Drawable.ic_no_collect);
    }

    public void Update(IngredientViewModel model)
    {
        model.InitAllNutritional();
        var ingredient = model.Ingredient;

        Dosage.Text = ingredient.InputDosage.ShortStr() + " " + ingredient.InputUnit;

        OutDosage.Text = ingredient.OutputDosage.ShortStr() + " " + ingredient.OutputUnit;

        EstimatedDosage.Text = ingredient.EstimatedDosage.ShortStr() + " " + ingredient.InputUnit;

        BindCategory();
        var dir = new Dictionary<EvaluateTag, (FlexboxLayout, int, int)>
        {
            [EvaluateTag.Positive] = (PositiveLayout, Drawable.shape_text_bg_2, ResourceConstant.Color.text_2),
            [EvaluateTag.Negative] = (NegativeLayout, Drawable.shape_text_bg_1, ResourceConstant.Color.text_1),
            [EvaluateTag.Allergy] = (AllergyLayout, Drawable.shape_text_bg_3, ResourceConstant.Color.text_3)
        };
        foreach (var (key, value) in model.Evaluate)
        {
            if (!dir.TryGetValue(key, out var item)) continue;
            var (layout, bgColor, textColor) = item;

            if (layout.FlexItemCount > 1)
                layout.RemoveViews(1, layout.FlexItemCount - 1);

            foreach (var se in value)
            {
                var holder = new ItemFlagHolder(activity);
                holder.Name.Text = se;
                holder.Name.SetBackgroundResource(bgColor);
                var color = activity?.Resources?.GetColor(textColor);
                holder.Name.SetTextColor((Color)color);
                layout.AddView(holder.Root, 1);
            }
        }

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

        {
            var entries = new List<PieEntry>();
            var colors = new List<int>();
            foreach (var other in model.OtherNutrient)
            {
                entries.Add(new PieEntry((float)other.Rate, other.Name));
                colors.Add(other.Color.GetAndroidColor()[0]);
            }

            var dataSet = new PieDataSet(entries, "") { Colors = colors };
            dataSet.SetDrawValues(false);
            PieChartMore.Data = new PieData(dataSet);
            PieChartMore.Init();
        }

        NutrientChart.Bind(activity, model.ProteinNutrient);
        NutrientChart.NutrientChartItems.ForEach(x =>
        {
            x.NutrientChartImg.ChangeSize(60, 60);
            x.Progress.Root.ChangeSize(null, 30);
            x.NutrientChartText.SetTextSize(ComplexUnitType.Dip, 12);
        });

        NutrientList.RemoveAllViews();
        foreach (var contentModel in model.OtherNutrient)
        {
            var h = new ItemIngredientInfoHolder(activity);
            h.Bind(contentModel);
            NutrientList.AddView(h.Root);
        }

        var list = new List<(TextView, TextView)> { (F2, F1), (F4, F3) };
        for (var i = 0; i < model.EnergyNutrient.Count; i++)
        {
            var (item1, item2) = model.EnergyNutrient[i];
            (list[i].Item1.Text, list[i].Item2.Text) = (item1, item2.ShortStr());
        }

        if (model.EnergyNutrient.Count == 0)
        {
            (list[0].Item1.Text, list[0].Item2.Text) = ("", "");
            (list[1].Item1.Text, list[1].Item2.Text) = ("", "");
        }


        if (PositiveLayout.ChildCount == 1) PositiveLayout.Visibility = ViewStates.Gone;
        if (NegativeLayout.ChildCount == 1) NegativeLayout.Visibility = ViewStates.Gone;
        if (AllergyLayout.ChildCount == 1) AllergyLayout.Visibility = ViewStates.Gone;

        if (model.ProteinNutrient.Count == 0)
        {
            NutrientChart.Root.Visibility = ViewStates.Gone;
            PieChartEnt.Visibility = ViewStates.Gone;
        }
        else
        {
            NutrientChart.Root.Visibility = ViewStates.Visible;
            PieChartEnt.Visibility = ViewStates.Visible;
        }

        if (model.OtherNutrient.Count == 0)
        {
            NutrientList.Visibility = ViewStates.Gone;
            PieChartMore.Visibility = ViewStates.Gone;
        }
        else
        {
            NutrientList.Visibility = ViewStates.Visible;
            PieChartMore.Visibility = ViewStates.Visible;
        }
    }

    public void BindCategory()
    {
        if (EmojiLayout.FlexItemCount > 1)
            EmojiLayout.RemoveViews(0, EmojiLayout.FlexItemCount - 1);
        if (CategoryLayout.FlexItemCount > 1)
            CategoryLayout.RemoveViews(0, CategoryLayout.FlexItemCount - 1);

        foreach (var t in model.Category)
        {
            var holder = new ItemCategoryHolder(activity);
            holder.Bind(t);
            switch (t.TypeId)
            {
                case CategoryType.Emoji:
                    EmojiLayout.AddView(holder.Root, 0);
                    break;
                case CategoryType.Category:
                    CategoryLayout.AddView(holder.Root, 0);
                    break;
            }

            holder.CategoryLayout.CallClick(() =>
            {
                t.OnLike(model.Ingredient.IngredientId, IdCategory.Ingredient);
                holder.Bind(t);
            });
        }
    }
}