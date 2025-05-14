using Android.Attribute;
using Android.Component;
using Android.Helper;
using Android.Models;
using Android.Views;
using AnyLibrary.Helper;
using Bumptech.Glide;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;
using static Android.Text.InputTypes;

namespace Android.Holder;

[ViewClassBind(Layout.item_recipe_ingredient)]
public class ItemRecipeIngredientHolder(App.Activity activity) : ViewHolder<View>(activity)
{
    [ViewBind(Id.id_recipe_ingredient_file_url)]
    public ImageView FileUrl;

    [ViewBind(Id.id_recipe_ingredient_name)]
    public TextView Name;

    [ViewBind(Id.id_recipe_ingredient_dosage)]
    public TextView Dosage;

    [ViewBind(Id.id_recipe_ingredient_out_dosage)]
    public TextView OutDosage;

    [ViewBind(Id.id_recipe_ingredient_dosage_layout)]
    public LinearLayout InputLayout;

    [ViewBind(Id.id_recipe_ingredient_out_dosage_layout)]
    public LinearLayout OutputLayout;

    [ViewHolderBind(Id.id_recipe_ingredient_nutrient_chart)]
    public MacroNutrientChartHolder NutrientChart;

    protected override void Init()
    {
    }

    public void Bind(IngredientModel model, Action action)
    {
        Glide.With(Root).Load(model.FileUrl).Into(FileUrl);
        Name.Text = model.IName;
        InputLayout.CallClick(() =>
        {
            var items = UnitHelper.GetAllUnit(model.Unit);
            var pos = items.IndexOf(model.InputUnit);
            var msgItems = items.Select(x => new MsgItem { Text = x }).ToList();

            MsgBoxHelper.Builder()
                .AddEditText(model.InputDosage.ShortStr(), ClassNumber | NumberFlagDecimal, 12, "请输入用量")
                .AddLisView(msgItems, pos)
                .Show(list =>
                {
                    model.EchoUnit = items[(int)list[1]];
                    var data = decimal.Parse((string)list[0]);
                    model.InputDosage = data - 1;
                    model.InputDosage += 1;
                    Update(model);
                    action.Invoke();
                });
        });

        OutputLayout.CallClick(() =>
        {
            var items = model.Quantity.Keys.ToList();
            var pos = items.IndexOf(model.OutputUnit);
            var msgItems = items.Select(x => new MsgItem { Text = x }).ToList();
            if (msgItems.Count == 0) return;

            MsgBoxHelper.Builder()
                .AddEditText(model.OutputDosage.ShortStr(), ClassNumber | NumberFlagDecimal, 12, "请输入用量")
                .AddLisView(msgItems, pos)
                .Show(list =>
                {
                    model.OutputUnit = items[(int)list[1]];
                    var outputDosage = decimal.Parse((string)list[0]);
                    model.OutputDosage = outputDosage - 1;
                    model.OutputDosage += 1;
                    Update(model);
                    action.Invoke();
                });
        });

        NutrientChart.Bind(activity, model.NutrientContent);

        Root.CallClick(() => { ActivityHelper.GotoIngredient(model.IngredientId); });

        Update(model);
    }

    public void Update(IngredientModel model)
    {
        NutrientChart.Bind(activity, model.NutrientContent);
        Dosage.Text = model.InputDosage.ShortStr() + " " + model.InputUnit;
        OutDosage.Text = model.OutputDosage.ShortStr() + " " + model.OutputUnit;
    }
}

[ViewClassBind(Layout.item_recipe_ingredient)]
public class ItemRecipeIngredient2Holder(App.Activity activity) : ViewHolder<View>(activity)
{
    [ViewBind(Id.id_recipe_ingredient_file_url)]
    public ImageView FileUrl;

    [ViewBind(Id.id_recipe_ingredient_name)]
    public TextView Name;

    [ViewBind(Id.id_recipe_ingredient_dosage)]
    public TextView Dosage;

    [ViewBind(Id.id_recipe_ingredient_out_dosage)]
    public TextView OutDosage;

    [ViewBind(Id.id_recipe_ingredient_dosage_layout)]
    public LinearLayout InputLayout;

    [ViewBind(Id.id_recipe_ingredient_out_dosage_layout)]
    public LinearLayout OutputLayout;

    [ViewHolderBind(Id.id_recipe_ingredient_nutrient_chart)]
    public MacroNutrientChartHolder NutrientChart;

    [ViewBind(Id.id_recipe_ingredient_close)]
    public ImageView IngredientClose;

    protected override void Init()
    {
    }

    public void Bind(IngredientModel model, Action action)
    {
        Glide.With(Root).Load(model.FileUrl).Into(FileUrl);
        Name.Text = model.IName;
        InputLayout.CallClick(() =>
        {
            var items = UnitHelper.GetAllUnit(model.Unit);
            var pos = items.IndexOf(model.InputUnit);
            var msgItems = items.Select(x => new MsgItem { Text = x }).ToList();

            MsgBoxHelper.Builder()
                .AddEditText(model.InputDosage.ShortStr(), ClassNumber | NumberFlagDecimal, 12, "请输入用量")
                .AddLisView(msgItems, pos)
                .Show(list =>
                {
                    model.Unit = items[(int)list[1]];
                    var data = decimal.Parse((string)list[0]);
                    model.Dosage = data - 1;
                    model.Dosage += 1;
                    Update(model);
                    action.Invoke();
                });
        });

        OutputLayout.CallClick(() =>
        {
            var items = model.Quantity.Keys.ToList();
            var pos = items.IndexOf(model.OutputUnit);
            var msgItems = items.Select(x => new MsgItem { Text = x }).ToList();
            if (msgItems.Count == 0) return;

            MsgBoxHelper.Builder()
                .AddEditText(model.OutputDosage.ShortStr(), ClassNumber | NumberFlagDecimal, 12, "请输入用量")
                .AddLisView(msgItems, pos)
                .Show(list =>
                {
                    var item = items[(int)list[1]];
                    var dosage = decimal.Parse((string)list[0]);
                    if (model.Quantity.TryGetValue(item, out var vv))
                    {
                        var result = UnitHelper.ConvertBaseUnitTo(dosage * vv, model.Unit);
                        model.Dosage = result - 1;
                        model.Dosage += 1;
                    }

                    Update(model);
                    action.Invoke();
                });
        });

        NutrientChart.Bind(activity, model.NutrientContent);

        Root.CallClick(() => { ActivityHelper.GotoIngredient(model.IngredientId); });

        Update(model);
    }

    public void Update(IngredientModel model)
    {
        NutrientChart.Bind(activity, model.NutrientContent);
        Dosage.Text = model.InputDosage.ShortStr() + " " + model.InputUnit;
        OutDosage.Text = model.OutputDosage.ShortStr() + " " + model.OutputUnit;
    }
}