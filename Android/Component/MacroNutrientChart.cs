using Android.Attribute;
using Android.ViewModel;
using Android.Views;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;

namespace Android.Component;

[ViewClassBind(Layout.component_macro_nutrient_chart)]
public class MacroNutrientChartHolder(View root) : ViewHolder<View>(root)
{
    public List<ItemMacroNutrientChartHolder> NutrientChartItems;

    protected override void Init()
    {
    }

    public void Bind(Android.App.Activity activity, List<NutrientContentViewModel> models)
    {
        if (Root is not LinearLayout layout) return;
        NutrientChartItems = [];
        layout.RemoveAllViews();
        if (models is null || models.Count == 0) return;
        foreach (var model in models)
        {
            var holder = new ItemMacroNutrientChartHolder(activity);
            holder.Bind(model);
            layout.AddView(holder.Root);
            NutrientChartItems.Add(holder);
        }
    }
}