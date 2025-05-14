using Android.Attribute;
using Android.Models;
using Android.Views;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;

namespace Android.Holder;

[ViewClassBind(Layout.item_category)]
public class ItemCategoryHolder(App.Activity activity) : ViewHolder<View>(activity)
{
    [ViewBind(Id.id_category_layout)]
    public LinearLayout CategoryLayout;

    [ViewBind(Id.id_category_name)]
    public TextView CategoryName;

    [ViewBind(Id.id_category_cont)]
    public TextView CategoryCont;

    protected override void Init()
    {
    }

    public void Bind(CategoryModel model)
    {
        if (model.Count == 0)
        {
            Root.Visibility = ViewStates.Gone;
            return;
        }

        CategoryCont.Visibility = model.Count > 1
            ? ViewStates.Visible
            : ViewStates.Gone;

        CategoryName.Text = model.Name;
        CategoryCont.Text = model.Count.ToString();

        int color = model.IsLike
            ? Drawable.shape_label12_bg
            : Drawable.shape_label1_bg;
        CategoryLayout.SetBackgroundResource(color);
    }
}