using Android.Attribute;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;

namespace Android.Holder;

public class SrCategoryHolder(View root) : ViewHolder<View>(root)
{
    [ViewBind(Id.id_category_text)]
    public TextView CategoryTextView;

    [ViewBind(Id.id_more_text)] public TextView MoreTextView;

    [ViewBind(Id.sr_ingredient_one)]
    public LinearLayout OneItemView;

    [ViewBind(Id.id_recommend_list)]
    public RecyclerView RecommendationRecycler;

    protected override void Init()
    {
        // MoreTextView
    }
}