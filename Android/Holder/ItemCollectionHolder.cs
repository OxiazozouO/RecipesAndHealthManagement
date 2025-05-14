using Android.Attribute;
using Android.Helper;
using Android.ViewModel;
using Android.Views;
using AnyLibrary.Constants;
using AnyLibrary.Helper;
using Bumptech.Glide;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;

namespace Android.Holder;

[ViewClassBind(Layout.item_collection)]
public class ItemCollectionHolder(App.Activity activity) : ViewHolder<View>(activity)
{
    [ViewBind(Id.id_collection_cover_image)]
    public ImageView CoverImageView;

    [ViewBind(Id.id_collection_like_count)]
    public TextView LikeCountTextView;

    [ViewBind(Id.id_collection_max_category_or_mod_date)]
    public TextView MaxCategoryOrModDateTextView;

    [ViewBind(Id.id_collection_title_text)]
    public TextView TitleTextView;

    [ViewBind(Id.id_collection_author_name)]
    public TextView AuthorNameTextView;

    [ViewBind(Id.id_collection_author_avatar)]
    public ImageView AuthorAvatarImageView;

    [ViewBind(Id.id_collection_islike)] public ImageView Islike;

    protected override void Init()
    {
    }

    public void Bind(CollectionInfo info, Func<int, long, bool> action)
    {
        Glide.With(Root).Load(info.Collection.FileUrl).Into(CoverImageView);
        Glide.With(Root).Load(info.User.AuthorFileUrl).Into(AuthorAvatarImageView);
        AuthorAvatarImageView.Tag = info.User.UserId;
        AuthorNameTextView.Text = info.User.AuthorUName;
        AuthorNameTextView.Tag = info.User.UserId;

        LikeCountTextView.Text = info.Collection.FavoriteCount.ToString();
        MaxCategoryOrModDateTextView.Text = info.User.ModifyDate.TimeStr2();
        TitleTextView.Text = info.Collection.Title;

        Root.CallClick(() => { action?.Invoke(IdCategory.Collection, info.Collection.CollectionId); });


        Islike.SetImageResource(info.Collection.IsLike ? Drawable.ic_collect : Drawable.ic_no_collect);
    }
}

public class CollectionInfo
{
    public CollectionModel Collection { set; get; }
    public UserInfo User { set; get; }

    public class UserInfo
    {
        public int UserId { set; get; }
        public string AuthorUName { set; get; }
        public string AuthorFileUrl { set; get; }

        public DateTime ModifyDate { set; get; }
    }
}