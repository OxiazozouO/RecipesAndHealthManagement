using Android.Attribute;
using Android.Views;
using Google.Android.Material.Search;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;

namespace Android.Holder;

[ViewClassBind(Layout.search_toolbar)]
public class SearchToolbarHolder : ViewHolder<View>
{
    [ViewBind(Id.id_search_edit_text)]
    public SearchBar SearchEditText;

    [ViewBind(Id.id_search_button)]
    public TextView SearchButton;

    protected override void Init()
    {
    }

    public SearchToolbarHolder(App.Activity activity) : base(activity)
    {
    }

    public SearchToolbarHolder(View root) : base(root)
    {
    }
}