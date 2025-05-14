using Android.Attribute;
using Android.Component;
using Android.Helper;
using Android.Views;
using AnyLibrary.Constants;
using AnyLibrary.Helper;
using Bumptech.Glide;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;

namespace Android.Holder;

[ViewClassBind(Layout.item_ingredient)]
public class ItemIngredientHolder(View view) : ViewHolder<View>(view)
{
    [ViewBind(Id.id_ingredient_cover_image)]
    public ImageView CoverImageView;

    [ViewBind(Id.id_ingredient_like_count)]
    public TextView LikeCountTextView;

    [ViewBind(Id.id_ingredient_max_category_or_mod_date)]
    public TextView MaxCategoryOrModDateTextView;

    [ViewBind(Id.id_ingredient_title_text)]
    public TextView TitleTextView;

    [ViewHolderBind(Id.id_ingredient_com_macro_nutrient_chart)]
    public MacroNutrientChartHolder MacroNutrientChartHolder;

    [ViewBind(Id.id_ingredient_author_name)]
    public TextView AuthorNameTextView;

    [ViewBind(Id.id_ingredient_author_avatar)]
    public ImageView AuthorAvatarImageView;

    [ViewBind(Id.id_ingredient_islike)] public ImageView Islike;

    protected override void Init()
    {
    }

    public void Bind(App.Activity activity, IngredientInfo info, Func<int, long, bool> action)
    {
        Glide.With(Root).Load(info.Ingredient.FileUrl).Into(CoverImageView);
        Glide.With(Root).Load(info.User.AuthorFileUrl).Into(AuthorAvatarImageView);
        AuthorAvatarImageView.Tag = info.User.UserId;
        AuthorNameTextView.Text = info.User.AuthorUName;
        AuthorNameTextView.Tag = info.User.UserId;

        LikeCountTextView.Text = info.Ingredient.FavoriteCount.ToString();
        MaxCategoryOrModDateTextView.Text = info.User.ModifyDate.TimeStr2();
        TitleTextView.Text = info.Ingredient.IName;

        info.Ingredient.InitNutritional();
        MacroNutrientChartHolder.Bind(activity, info.Ingredient.NutrientContent);

        Root.CallClick(() => { action?.Invoke(IdCategory.Ingredient, info.Ingredient.IngredientId); });


        Islike.SetImageResource(info.Ingredient.IsLike ? Drawable.ic_collect : Drawable.ic_no_collect);
    }
}