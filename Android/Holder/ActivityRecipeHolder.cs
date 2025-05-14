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
using static Android.Views.ViewStates;

namespace Android.Holder;

[ViewClassBind(Layout.activity_recipe)]
public class ActivityRecipeHolder(App.Activity activity) : ViewHolder<View>(activity)
{
    [ViewHolderBind(Id.id_recipe_image_viewer)]
    public ImageViewerHolder ImageViewer;

    [ViewBind(Id.id_recipe_scroll_view)] public ScrollView Scroll;
    [ViewBind(Id.id_recipe_img)] public ImageView Image;

    [ViewBind(Id.id_recipe_name)] public TextView Name;

    [ViewBind(Id.id_recipe_all_time)] public TextView AllTime;

    [ViewBind(Id.id_recipe_ref)] public TextView Ref;

    [ViewBind(Id.id_recipe_emoji_layout)] public FlexboxLayout EmojiLayout;

    [ViewBind(Id.id_recipe_add_emoji)] public ImageView AddEmoji;

    [ViewBind(Id.id_recipe_category_layout)]
    public FlexboxLayout CategoryLayout;

    [ViewBind(Id.id_recipe_add_category)] public ImageView AddCategory;

    [ViewBind(Id.id_recipe_ingredient_list)]
    public LinearLayout IngredientList;

    [ViewBind(Id.id_recipe_input_layout)] public LinearLayout InputLayout;
    [ViewBind(Id.id_recipe_output_layout)] public LinearLayout OutputLayout;

    [ViewBind(Id.id_recipe_dosage)] public TextView Dosage;

    [ViewBind(Id.id_recipe_out_dosage)] public TextView OutputDosage;

    [ViewBind(Id.id_recipe_refresh)] public ImageView Refresh;

    [ViewBind(Id.id_recipe_add_to_physical)]
    public LinearLayout AddToPhysical;

    [ViewBind(Id.id_pie_chart_ent)] public PieChart PieChartEnt;

    [ViewBind(Id.id_recipe_f1)] public TextView F1;

    [ViewBind(Id.id_recipe_f2)] public TextView F2;

    [ViewBind(Id.id_recipe_f3)] public TextView F3;

    [ViewBind(Id.id_recipe_f4)] public TextView F4;

    [ViewHolderBind(Id.id_recipe_nutrient_chart)]
    public MacroNutrientChartHolder NutrientChart;

    [ViewBind(Id.id_recipe_other_layout)] public FlexboxLayout OtherLayout;

    [ViewBind(Id.id_recipe_positive_layout)]
    public FlexboxLayout PositiveLayout;

    [ViewBind(Id.id_recipe_negative_layout)]
    public FlexboxLayout NegativeLayout;

    [ViewBind(Id.id_recipe_allergy_layout)]
    public FlexboxLayout AllergyLayout;

    [ViewBind(Id.id_recipe_step_list)] public LinearLayout StepList;

    [ViewHolderBind(Id.id_recipe_comment)] public CommentHolder CommentHolder;

    [ViewBind(Id.id_recipe_like)] public ImageView Islike;

    [ViewBind(Id.id_recipe_like_num)] public TextView LikeNum;

    [ViewBind(Id.id_recipe_share)] public ImageView Share;

    [ViewBind(Id.id_recipe_report)] public ImageView Report;

    [ViewBind(Id.id_recipe_more_bnt)] public LinearLayout RecipeMore;

    [ViewBind(Id.id_recipe_more_text)] public TextView RecipeMoreText;


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

        InputLayout.CallClick(() =>
        {
            var recipe = model.Recipe;

            MsgBoxHelper.Builder()
                .AddEditText(recipe.Dosage.ShortStr(), ClassNumber | NumberFlagDecimal, 12, "请输入用量")
                .Show(list =>
                {
                    var data = decimal.Parse((string)list[0]);
                    recipe.Dosage = data - 1;
                    recipe.Dosage += 1;
                    Update();
                });
        });

        OutputLayout.CallClick(() =>
        {
            var recipe = model.Recipe;

            MsgBoxHelper.Builder()
                .AddEditText(recipe.EstimatedDosage.ShortStr(), ClassNumber | NumberFlagDecimal, 12, "请输入用量")
                .Show(list =>
                {
                    var data = decimal.Parse((string)list[0]);
                    recipe.EstimatedDosage = data - 1;
                    recipe.EstimatedDosage += 1;
                    Update();
                });
        });

        Refresh.CallClick(() =>
        {
            model.Reset();
            Update();
        });

        Share.CallClick(() => { });

        Report.CallClick(() =>
        {
            ActivityHelper.GotoReport(model.Recipe.RecipeId, IdCategory.Recipe, model.Recipe.RName);
        });
    }

    public RecipeViewModel model;
    public List<(string, string, string)> files;

    public void Bind(RecipeViewModel model)
    {
        this.model = model;
        var recipe = model.Recipe;
        Glide.With(Root)
            .Load(recipe.FileUrl)
            .Into(Image);
        files = [(recipe.Title, recipe.FileUrl, recipe.Summary)];
        Image.CallClick(() => { FileUrlOnClick(recipe.FileUrl); });

        Name.Text = recipe.RName;
        AllTime.Text = recipe.SpendTime.Convert();
        Ref.Text = recipe.Summary;

        CommentHolder.Bind(Scroll, model.Recipe.RecipeId, IdCategory.Recipe, activity);

        LikeNum.Text = recipe.FavoriteCount.ToString();

        Update();
        Update2(model.Recipe);
    }

    public void Update2(RecipeModel model)
    {
        LikeNum.Text = model.FavoriteCount.ToString();
        Islike.SetImageResource(model.IsLike ? Drawable.ic_collect : Drawable.ic_no_collect);
    }

    public void Update()
    {
        model.Recipe.InitAllNutritional();
        var recipe = model.Recipe;

        Dosage.Text = recipe.Dosage.ShortStr() + " 克";
        OutputDosage.Text = recipe.EstimatedDosage.ShortStr() + " 克";

        BindCategory();
        var dir = new Dictionary<EvaluateTag, (FlexboxLayout, int, int)>
        {
            [EvaluateTag.Positive] = (PositiveLayout, Drawable.shape_text_bg_2, Color.text_2),
            [EvaluateTag.Negative] = (NegativeLayout, Drawable.shape_text_bg_1, Color.text_1),
            [EvaluateTag.Allergy] = (AllergyLayout, Drawable.shape_text_bg_3, Color.text_3)
        };
        foreach (var (key, value) in model.Recipe.Evaluate)
        {
            if (!dir.TryGetValue(key, out var item)) continue;
            var (layout, bgColor, textColor) = item;

            if (layout.FlexItemCount > 1)
                layout.RemoveViews(1, layout.FlexItemCount - 1);

            foreach (var se in value)
            {
                var holder = new ItemFlagHolder(activity);
                holder.Bind(se, bgColor, textColor);
                layout.AddView(holder.Root, 1);
            }
        }

        {
            var entries = new List<PieEntry>();
            var colors = new List<int>();
            foreach (var pro in model.Recipe.OtherNutrient)
            {
                entries.Add(new PieEntry((float)pro.Rate, pro.Name));
                colors.Add(pro.Color.GetAndroidColor()[0]);
            }

            var dataSet = new PieDataSet(entries, "") { Colors = colors };
            dataSet.SetDrawValues(false);
            PieChartEnt.Data = new PieData(dataSet);
            PieChartEnt.Init();
        }

        NutrientChart.Bind(activity, model.Recipe.ProteinNutrient);
        NutrientChart.NutrientChartItems.ForEach(x =>
        {
            x.NutrientChartImg.ChangeSize(60, 60);
            x.Progress.Root.ChangeSize(null, 30);
            x.NutrientChartText.SetTextSize(ComplexUnitType.Dip, 12);
        });

        var list = new List<(TextView, TextView)> { (F2, F1), (F4, F3) };
        for (var i = 0; i < model.Recipe.EnergyNutrient.Count; i++)
        {
            var (item1, item2) = model.Recipe.EnergyNutrient[i];
            (list[i].Item1.Text, list[i].Item2.Text) = (item1, item2.ShortStr());
        }

        StepList.RemoveAllViews();
        foreach (var recipeStep in model.Recipe.Steps)
        {
            var holder = new ItemRecipeStepHolder(activity);
            holder.Bind(recipeStep);
            StepList.AddView(holder.Root);
            if (!string.IsNullOrEmpty(recipeStep.FileUrl))
                files.Add((recipeStep.Title, recipeStep.FileUrl, recipeStep.Summary + "\n\n" + recipeStep.Refer));
            holder.FileUrl.CallClick(() => { FileUrlOnClick(recipeStep.FileUrl); });
        }

        IngredientList.RemoveAllViews();
        foreach (var ingredient in model.Recipe.Ingredients)
        {
            var holder = new ItemRecipeIngredientHolder(activity);
            holder.Bind(ingredient, Update);
            IngredientList.AddView(holder.Root);
        }

        {
            int pos = 0;
            OtherLayout.RemoveAllViews();
            List<ItemNutrientHolder> hs = [];
            foreach (var ingredient in model.Recipe.OtherNutrient)
            {
                var holder = new ItemNutrientHolder(activity);
                holder.Bind(ingredient);
                OtherLayout.AddView(holder.Root);

                pos++;
                if (pos > 3) hs.Add(holder);
            }

            ViewStates flag = Visible;
            RecipeMore.CallClick(() =>
            {
                var str = flag == Visible ? "更多" : "收起";
                RecipeMoreText.Text = str;
                flag = flag == Visible ? Gone : Visible;
                hs.ForEach(h => h.Root.Visibility = flag);
            });
            RecipeMore.CallOnClick();

            RecipeMore.Visibility = pos <= 3 ? Gone : Visible;
        }


        PositiveLayout.Visibility = PositiveLayout.ChildCount == 1 ? Gone : Visible;
        NegativeLayout.Visibility = NegativeLayout.ChildCount == 1 ? Gone : Visible;
        AllergyLayout.Visibility = AllergyLayout.ChildCount == 1 ? Gone : Visible;

        if (model.Recipe.ProteinNutrient.Count == 0)
        {
            NutrientChart.Root.Visibility = Gone;
            PieChartEnt.Visibility = Gone;
        }
        else
        {
            NutrientChart.Root.Visibility = Visible;
            PieChartEnt.Visibility = Visible;
        }
    }

    private void FileUrlOnClick(string e)
    {
        int pos = files.IndexOf(files.FirstOrDefault(f => f.Item2 == e));
        ImageViewer.Root.Visibility = Visible;
        ImageViewer.Bind(activity, files, pos, () =>
        {
            ImageViewer.Root.Visibility = Gone;
        });
    }

    public void BindCategory()
    {
        if (EmojiLayout.FlexItemCount > 1)
            EmojiLayout.RemoveViews(0, EmojiLayout.FlexItemCount - 1);
        if (CategoryLayout.FlexItemCount > 1)
            CategoryLayout.RemoveViews(0, CategoryLayout.FlexItemCount - 1);

        foreach (var t in model.Recipe.Category)
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
                t.OnLike(model.Recipe.RecipeId, IdCategory.Recipe);
                holder.Bind(t);
            });
        }
    }
}