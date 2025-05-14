using Android.Attribute;
using Android.ViewModel;
using Android.Views;
using Bumptech.Glide;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;

namespace Android.Holder;

[ViewClassBind(Layout.sr_ingredient_item)]
public class SrIngredientHolder(App.Activity activity) : ViewHolder<View>(activity)
{
    [ViewBind(Id.id_cover_image)] public ImageView CoverImage;

    [ViewBind(Id.id_title_text)] public TextView TitleText;

    [ViewBind(Id.id_author_avatar)]
    public ImageView AuthorAvatar;

    [ViewBind(Id.id_author_name)] public TextView AuthorName;

    [ViewBind(Id.id_like_count)] public TextView LikeCount;

    public void Bind(HomeInfoViewModel model)
    {
        Glide.With(Root).Load(model.FileUrl).Into(CoverImage);
        Glide.With(Root).Load(model.AuthorFileUrl).Into(AuthorAvatar);
        TitleText.Text = model.Title;
        AuthorName.Text = model.UserName;
        LikeCount.Text = model.LikeCount;
    }

    protected override void Init()
    {
    }
}