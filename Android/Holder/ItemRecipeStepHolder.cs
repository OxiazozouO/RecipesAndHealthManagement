using Android.Attribute;
using Android.Helper;
using Android.Models;
using Android.Views;
using AnyLibrary.Helper;
using Bumptech.Glide;
using Google.Android.Flexbox;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;

namespace Android.Holder;

[ViewClassBind(Layout.item_recipe_step)]
public class ItemRecipeStepHolder(App.Activity activity) : ViewHolder<View>(activity)
{
    [ViewBind(Id.id_step_name)] public TextView Name;

    [ViewBind(Id.id_step_file_url)] public ImageView FileUrl;

    [ViewBind(Id.id_step_time_img)] public ImageView TimeImg;
    [ViewBind(Id.id_step_time)] public TextView Time;

    [ViewBind(Id.id_step_stock_layout)] public FlexboxLayout StockLayout;

    [ViewBind(Id.id_item_step_refer)] public TextView Refer;

    [ViewBind(Id.id_step_summary)] public TextView Summary;

    protected override void Init()
    {
    }

    public void Bind(StepModel step)
    {
        Name.Text = step.Title;
        Glide.With(Root)
            .Load(step.FileUrl)
            .Into(FileUrl);

        Time.Text = step.OutputRequiredTime?.Convert();

        foreach (var root in step.IngredientRoots)
        {
            var item = new ItemFlagHolder(activity);
            item.Bind(root.IName, Drawable.shape_text_bg_2, Color.text_2);
            item.Name.TextSize = 14;
            StockLayout.AddView(item.Root);
            item.Root.Click += (_, _) => { ActivityHelper.GotoIngredient(root.IngredientId); };
        }

        Refer.Text = step.Refer;
        Summary.Text = step.Summary;

        TimeImg.Visibility = string.IsNullOrEmpty(Time.Text?.Trim())
            ? ViewStates.Invisible
            : ViewStates.Visible;

        StockLayout.Visibility = step.IngredientRoots.Count == 0
            ? ViewStates.Gone
            : ViewStates.Visible;
    }
}