using Android.Attribute;
using Android.Helper;
using Android.ViewModel;
using Android.Views;
using AnyLibrary.Constants;
using AnyLibrary.Helper;
using Bumptech.Glide;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;

namespace Android.Holder;

[ViewClassBind(Layout.item_recipe_small)]
public class ItemRecipeSmallHolder(View root) : ViewHolder<View>(root)
{
    [ViewBind(Id.id_smail_cover_image)] public ImageView SmallCoverImageView;

    [ViewBind(Id.id_smail_like_count)] public TextView LikeCountTextView;

    [ViewBind(Id.id_smail_title_text)] public TextView TitleTextView;

    [ViewBind(Id.id_smail_author_avatar)] public ImageView AuthorAvatarImageView;

    [ViewBind(Id.id_smail_author_name)] public TextView AuthorNameTextView;

    [ViewBind(Id.id_max_category)] public TextView MaxCategoryTextView;

    [ViewBind(Id.id_smail_islike)] public ImageView IsLike;

    [ViewBind(Id.id_smail_max_category_or_mod_date)]
    public TextView MaxCategoryOrModDate;

    protected override void Init()
    {
    }

    public void Bind(RecipeInfoViewModel model, Func<int, long, bool> action)
    {
        Glide.With(root).Load(model.FileUrl).Into(SmallCoverImageView);
        Glide.With(root).Load(model.AuthorFileUrl).Into(AuthorAvatarImageView);
        LikeCountTextView.Text = model.FavoriteCount.ToString();
        TitleTextView.Text = model.Title;
        AuthorNameTextView.Text = model.AuthorUName;
        MaxCategoryTextView.Visibility = ViewStates.Gone;

        Root.CallClick(() => { action?.Invoke(IdCategory.Recipe, model.RecipeId); });

        IsLike.SetImageResource(model.IsLike ? Drawable.ic_collect : Drawable.ic_no_collect);

        MaxCategoryOrModDate.Text = model.ModifyDate.TimeStr2();
    }
}