using Android.Activity;
using Android.Attribute;
using Android.Component;
using Android.Helper;
using Android.HttpClients;
using Android.Models;
using Android.Util;
using Android.ViewModel;
using AnyLibrary.Constants;
using AnyLibrary.Helper;
using Bumptech.Glide;
using Google.Android.Flexbox;
using MikePhil.Charting.Charts;
using MikePhil.Charting.Data;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;
using static Android.Text.InputTypes;
using static Android.Views.ViewStates;
using Uri = Android.Net.Uri;

namespace Android.Holder;

[ViewClassBind(Layout.activity_add_recipe)]
public class ActivityAddRecipeHolder(App.Activity activity) : ViewHolder<ScrollView>(activity)
{
    [ViewBind(Id.id_add_recipe_img)] public ImageView Img;

    [ViewBind(Id.id_add_recipe_title)] public TextView Title;

    [ViewBind(Id.id_add_recipe_name)] public TextView Name;

    [ViewBind(Id.id_add_recipe_ref)] public TextView Ref;

    [ViewBind(Id.id_add_recipe_category_layout)]
    public FlexboxLayout CategoryLayout;

    [ViewBind(Id.id_add_recipe_add_category)]
    public ImageView AddCategory;

    [ViewBind(Id.id_add_recipe_ingredient_list)]
    public LinearLayout IngredientList;

    [ViewBind(Id.id_add_recipe_add_ingredient_bnt)]
    public ImageView AddIngredient;

    [ViewBind(Id.id_add_recipe_add_step_bnt)]
    public ImageView AddStepBnt;

    [ViewBind(Id.id_add_recipe_input_layout)]
    public LinearLayout InputLayout;

    [ViewBind(Id.id_add_recipe_dosage)] public TextView Dosage;

    [ViewBind(Id.id_add_recipe_output_layout)]
    public LinearLayout OutputLayout;

    [ViewBind(Id.id_add_recipe_out_dosage)]
    public TextView OutDosage;

    [ViewBind(Id.id_add_recipe_nutritional)]
    public LinearLayout Nutritional;

    [ViewBind(Id.id_add_recipe_nutritional_arrow)]
    public ImageView NutritionalArrow;

    [ViewBind(Id.id_add_recipe_nutritional_layout)]
    public LinearLayout NutritionalLayout;

    [ViewBind(Id.id_add_recipe_positive_layout)]
    public FlexboxLayout PositiveLayout;

    [ViewBind(Id.id_add_recipe_negative_layout)]
    public FlexboxLayout NegativeLayout;

    [ViewBind(Id.id_add_recipe_allergy_layout)]
    public FlexboxLayout AllergyLayout;

    [ViewHolderBind(Id.id_add_recipe_nutrient_chart)]
    public MacroNutrientChartHolder NutrientChart;

    [ViewBind(Id.id_add_recipe_pie_chart_ent)]
    public PieChart PieChartEnt;

    [ViewBind(Id.id_add_recipe_pie_chart_more)]
    public PieChart PieChartMore;

    [ViewBind(Id.id_add_recipe_nutrient_list)]
    public LinearLayout NutrientList;

    [ViewBind(Id.id_add_recipe_f1)] public TextView F1;
    [ViewBind(Id.id_add_recipe_f2)] public TextView F2;
    [ViewBind(Id.id_add_recipe_f3)] public TextView F3;
    [ViewBind(Id.id_add_recipe_f4)] public TextView F4;

    [ViewBind(Id.id_add_recipe_all_time)] public TextView AllTime;

    [ViewBind(Id.id_add_recipe_step_list)] public LinearLayout StepList;

    [ViewBind(Id.id_add_recipe_release_info)]
    public TextView ReleaseInfo;

    [ViewBind(Id.id_add_recipe_add_step_sub)]
    public TextView Sub;

    public List<ItemAddRecipeStepHolder> StepHolders = new List<ItemAddRecipeStepHolder>();

    public RecipeViewModel model;

    public long Tid;
    public long ReleaseId;

    public void Bind(long id)
    {
        if (model.Recipe.Ingredients.Any(w => w.IngredientId == id)) return;
        var req = ApiEndpoints.GetIngredientInfo(new
        {
            UserId = AppConfigHelper.AppConfig.Id,
            Id = id
        });
        if (req.Execute(out var res))
        {
            var imodel = res.Data.ToEntity<IngredientModel>();
            model.Recipe.Ingredients.Add(imodel);
            Reset();
        }
    }

    protected override void Init()
    {
        model = new RecipeViewModel
        {
            Recipe = new RecipeModel
            {
                Category = new List<CategoryModel>(),
                Steps = new List<StepModel>(),
                Ingredients = new List<IngredientModel>()
            }
        };

        AddCategory.CallClick(() =>
        {
            MsgBoxHelper.Builder()
                .AddEditText("", ClassText, 12, "请输入分类名称")
                .Show(list =>
                {
                    var name = (string)list[0];
                    var select = model
                        .Recipe
                        .Category
                        .Any(w => w.Name == name);
                    if (select) return;

                    var item = new CategoryModel
                    {
                        TypeId = CategoryType.Category,
                        Name = (string)list[0],
                        Count = 1,
                        IsLike = true
                    };
                    model.Recipe.Category.Add(item);
                    var holder = new ItemCategoryHolder(activity);
                    holder.Bind(item);
                    CategoryLayout.AddView(holder.Root, 0);

                    holder.CategoryLayout
                        .CallClick(() =>
                        {
                            CategoryLayout.RemoveView(holder.Root);
                            model.Recipe.Category.Remove(item);
                        });
                });
        });

        AddIngredient.CallClick(() => { ActivityHelper.GotoSearch(SearchFlag.Ingredient); });

        InputLayout.CallClick(() =>
        {
            var recipe = model.Recipe;

            MsgBoxHelper.Builder()
                .AddEditText(recipe.Dosage.ShortStr(), ClassNumber | NumberFlagDecimal, 12, "请输入用量")
                .Show(list =>
                {
                    var data = decimal.Parse((string)list[0]);
                    recipe.Dosage = data;
                    recipe.PutDosage();
                    Reset();
                });
        });

        OutputLayout.CallClick(() =>
        {
            var recipe = model.Recipe;

            MsgBoxHelper.Builder()
                .AddEditText(recipe.EstimatedDosage.ShortStr(), ClassNumber | NumberFlagDecimal, 12, "请输入用量")
                .Show(list =>
                {
                    recipe.EstimatedDosage = decimal.Parse((string)list[0]);
                    recipe.PutDosage();
                    Reset();
                });
        });

        Nutritional.CallClick(() =>
        {
            var b = Math.Abs(NutritionalArrow.Rotation - 90) < 0.01;
            NutritionalArrow.Rotation = b ? -90 : 90;
            NutritionalLayout.Visibility = b ? Gone : Visible;
        });

        AddStepBnt.CallClick(() =>
        {
            var stepModel = new StepModel();
            model.Recipe.Steps.Add(stepModel);
            Reset();
        });

        Img.CallClick(() => { activity.SelectImage(); });

        Title.CallClick(() =>
        {
            var recipe = model.Recipe;
            MsgBoxHelper.Builder()
                .AddEditText(recipe.Title, ClassText, 30, "请输入食谱标题（最多30字）")
                .Show(list =>
                {
                    recipe.Title = (string)list[0];
                    Title.Text = recipe.Title;
                });
        });

        Name.CallClick(() =>
        {
            var recipe = model.Recipe;
            MsgBoxHelper.Builder()
                .AddEditText(recipe.RName, ClassText, 30, "请输入食谱名字（最多30字）")
                .Show(list =>
                {
                    recipe.RName = (string)list[0];
                    Name.Text = recipe.RName;
                });
        });

        Ref.CallClick(() =>
        {
            var recipe = model.Recipe;
            MsgBoxHelper.Builder()
                .AddEditText(recipe.Summary, ClassText, 200, "请输入食谱的简介（最多200字）")
                .Show(list =>
                {
                    recipe.Summary = (string)list[0];
                    Ref.Text = recipe.Summary;
                });
        });

        ReleaseInfo.CallClick(() =>
        {
            MsgBoxHelper.Builder()
                .AddEditText(model.ReleaseInfo, ClassText, 200, "请输入参考资料来源（最多200字）")
                .Show(list =>
                {
                    model.ReleaseInfo = (string)list[0];
                    ReleaseInfo.Text = model.ReleaseInfo;
                });
        });

        Sub.CallClick(() =>
        {
            var recipe = model.Recipe;
            var recipeModel = new AddRecipeModel
            {
                RecipeId = Tid,
                FileUrl = recipe.FileUrl,
                Title = recipe.Title,
                RName = recipe.RName,
                Summary = recipe.Summary,
                ReleaseInfo = model.ReleaseInfo,

                Ingredients = recipe.Ingredients
                    .Where(i => i.Dosage > 0.0001m
                                && !string.IsNullOrEmpty(i.Unit)
                                && i.IngredientId != 0)
                    .Select(i => new AddRecipeIngredientModel
                    {
                        Dosage = UnitHelper.ConvertToBaseUnit(i.Dosage, i.Unit),
                        IngredientId = i.IngredientId
                    }).ToList(),

                Categories = recipe.Category
                    .Where(c => !string.IsNullOrEmpty(c.Name))
                    .Select(c => c.Name)
                    .ToList(),

                Steps = recipe.Steps.Where(s =>
                        !string.IsNullOrEmpty(s.Title)
                        && !string.IsNullOrEmpty(s.Refer))
                    .Select(s => new AddRecipeStepModel
                        {
                            Id = s.Id,
                            Title = s.Title,
                            FileUrl = s.FileUrl,
                            Summary = s.Summary,
                            RequiredTime = s.RequiredTime,
                            RequiredIngredient = TimeRateModel.ToString(s.TimeRateModels),
                            Refer = s.Refer
                        }
                    ).ToList()
            };

            bool b = recipeModel.ReleaseRecipe(ReleaseId);
            if (b) activity.Finish();
        });


        Reset();
    }

    public void Put()
    {
        Reset();
        if (!string.IsNullOrEmpty(model.Recipe.FileUrl))
            Glide
                .With(Root)
                .Load(model.Recipe.FileUrl)
                .Into(Img);

        Title.Text = model.Recipe.Title;
        Name.Text = model.Recipe.RName;
        Ref.Text = model.Recipe.Summary;
        ReleaseInfo.Text = model.ReleaseInfo;
    }

    public void Reset()
    {
        model.Recipe.Init();
        model.InitNutritional();
        Update();
    }

    public void Update()
    {
        var recipe = model.Recipe;
        Dosage.Text = recipe.Dosage.ShortStr() + " 克";
        OutDosage.Text = recipe.EstimatedDosage.ShortStr() + " 克";
        AllTime.Text = recipe.SpendTime.Convert();

        IngredientList.RemoveAllViews();
        foreach (var ingredient in model.Recipe.Ingredients)
        {
            var holder = new ItemRecipeIngredient2Holder(activity);
            holder.Bind(ingredient, Reset);
            IngredientList.AddView(holder.Root);
            holder.IngredientClose.Visibility = Visible;
            holder.IngredientClose.CallClick(() =>
            {
                model.Recipe.Ingredients.Remove(ingredient);
                Reset();
            });
        }

        StepList.RemoveAllViews();
        StepHolders.Clear();
        foreach (var step in model.Recipe.Steps)
        {
            var holder = new ItemAddRecipeStepHolder(activity);
            holder.Bind(step, () =>
            {
                recipe.InitSpendTime();
                AllTime.Text = recipe.SpendTime.Convert();
            });
            StepHolders.Add(holder);
            StepList.AddView(holder.Root);
            holder.Close.CallClick(() =>
            {
                model.Recipe.Steps.Remove(step);
                Reset();
            });
            holder.FileUrl.CallClick(() => { activity.SelectImage(model.Recipe.Steps.IndexOf(step) + 2); });
        }

        var dir = new Dictionary<EvaluateTag, (FlexboxLayout, int, int)>
        {
            [EvaluateTag.Positive] = (PositiveLayout, Drawable.shape_text_bg_2, Color.text_2),
            [EvaluateTag.Negative] = (NegativeLayout, Drawable.shape_text_bg_1, Color.text_1),
            [EvaluateTag.Allergy] = (AllergyLayout, Drawable.shape_text_bg_3, Color.text_3)
        };
        foreach (var (key, value) in recipe.Evaluate)
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
            foreach (var pro in recipe.ProteinNutrient)
            {
                entries.Add(new PieEntry(pro.Rate, pro.Str2));
                colors.Add(pro.Colors.GetAndroidColor()[0]);
            }

            var dataSet = new PieDataSet(entries, "") { Colors = colors };
            dataSet.SetDrawValues(false);
            if (entries.Count > 0)
            {
                PieChartEnt.Data = new PieData(dataSet);
                PieChartEnt.Init();
            }
        }

        {
            var entries = new List<PieEntry>();
            var colors = new List<int>();
            foreach (var other in recipe.OtherNutrient)
            {
                entries.Add(new PieEntry((float)other.Rate, other.Name));
                colors.Add(other.Color.GetAndroidColor()[0]);
            }

            var dataSet = new PieDataSet(entries, "") { Colors = colors };
            dataSet.SetDrawValues(false);
            if (entries.Count > 0)
            {
                PieChartMore.Data = new PieData(dataSet);
                PieChartMore.Init();
            }
        }

        NutrientChart.Bind(activity, recipe.ProteinNutrient);
        NutrientChart.NutrientChartItems.ForEach(x =>
        {
            x.NutrientChartImg.ChangeSize(60, 60);
            x.Progress.Root.ChangeSize(null, 30);
            x.NutrientChartText.SetTextSize(ComplexUnitType.Dip, 12);
        });

        NutrientList.RemoveAllViews();
        foreach (var contentModel in recipe.OtherNutrient)
        {
            var h = new ItemIngredientInfoHolder(activity);
            h.Bind(contentModel);
            NutrientList.AddView(h.Root);
        }

        var list = new List<(TextView, TextView)> { (F2, F1), (F4, F3) };
        for (var i = 0; i < recipe.EnergyNutrient.Count; i++)
        {
            var (item1, item2) = recipe.EnergyNutrient[i];
            (list[i].Item1.Text, list[i].Item2.Text) = (item1, item2.ShortStr());
        }

        if (recipe.EnergyNutrient.Count == 0)
        {
            (list[0].Item1.Text, list[0].Item2.Text) = ("", "");
            (list[1].Item1.Text, list[1].Item2.Text) = ("", "");
        }

        PositiveLayout.Visibility = PositiveLayout.ChildCount == 1 ? Gone : Visible;
        NegativeLayout.Visibility = NegativeLayout.ChildCount == 1 ? Gone : Visible;
        AllergyLayout.Visibility = AllergyLayout.ChildCount == 1 ? Gone : Visible;

        if (recipe.ProteinNutrient.Count == 0)
        {
            NutrientChart.Root.Visibility = Gone;
            PieChartEnt.Visibility = Gone;
        }
        else
        {
            NutrientChart.Root.Visibility = Visible;
            PieChartEnt.Visibility = Visible;
        }

        if (recipe.OtherNutrient.Count == 0)
        {
            NutrientList.Visibility = Gone;
            PieChartMore.Visibility = Gone;
        }
        else
        {
            NutrientList.Visibility = Visible;
            PieChartMore.Visibility = Visible;
        }
    }

    public void SetFileUrl(Uri? uri)
    {
        if (!activity.ContentResolver.FileUpload(uri, out string outFileName)) return;
        model.Recipe.FileUrl = outFileName;
        Glide
            .With(activity)
            .Load(uri)
            .Into(Img);
    }
}