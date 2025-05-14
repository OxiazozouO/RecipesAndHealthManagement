using Android.Attribute;
using Android.Helper;
using Android.Views;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;

namespace Android.Holder;

[ViewClassBind(Layout.search_toolbar2)]
public class SearchToolbar2Holder(View view) : ViewHolder<View>(view)
{
    [ViewBind(Id.id_image2_viewer_back)]
    public ImageView ImageViewerBack;

    [ViewBind(Id.id_search2_edit_text)]
    public EditText SearchEditText;

    [ViewBind(Id.id_search2_edit_text_close)]
    public ImageView CloseButton;

    [ViewBind(Id.id_search2_button)]
    public TextView SearchButton;

    protected override void Init()
    {
        CloseButton.CallClick(() => SearchEditText.Text = "");
        SearchEditText.BindTo(s =>
            CloseButton.Visibility = string.IsNullOrEmpty(s) ? ViewStates.Gone : ViewStates.Visible);
    }
}