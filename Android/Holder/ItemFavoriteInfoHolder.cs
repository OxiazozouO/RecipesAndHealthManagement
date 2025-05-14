using Android.Attribute;
using Android.Component;
using Android.ViewModel;
using Android.Views;
using AnyLibrary.Helper;
using Bumptech.Glide;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;

namespace Android.Holder;

[ViewClassBind(Layout.item_favorite_info)]
public class ItemFavoriteInfoHolder(App.Activity activity) : ViewHolder<View>(activity)
{
    [ViewBind(Id.id_favorite_info_cover_image)]
    public ImageView CoverImage;

    [ViewBind(Id.id_favorite_info_like_count)]
    public TextView LikeCount;

    [ViewBind(Id.id_favorite_info_max_category_or_mod_date)]
    public TextView MaxCategoryOrModDate;

    [ViewBind(Id.id_favorite_info_title_text)]
    public TextView TitleText;

    [ViewHolderBind(Id.id_favorite_info_com_macro_nutrient_chart)]
    public MacroNutrientChartHolder ChartHolder;

    [ViewBind(Id.id_favorite_info_author_avatar)]
    public ImageView AuthorAvatar;

    [ViewBind(Id.id_favorite_info_author_name)]
    public TextView AuthorName;

    [ViewBind(Id.id_favorite_info_refer)] public TextView Refer;
    [ViewBind(Id.id_favorite_info_check)] public CheckBox Check;

    protected override void Init()
    {
    }

    public void Bind(FavoriteAtModel.FavoriteItemViewModel item)
    {
        Glide.With(Root).Load(item.FavoriteItem.FileUrl).Into(CoverImage);
        LikeCount.Text = item.FavoriteItem.FavoriteCount.ToString();
        MaxCategoryOrModDate.Text = item.User.ModifyDate.TimeStr1();
        TitleText.Text = item.FavoriteItem.Name;
        ChartHolder.Bind(activity, item.FavoriteItem.NutrientContent);
        Glide.With(Root).Load(item.User.AuthorFileUrl).Into(AuthorAvatar);
        AuthorName.Text = item.User.AuthorUName;
        Refer.Text = item.FavoriteItem.Refer;
    }
}