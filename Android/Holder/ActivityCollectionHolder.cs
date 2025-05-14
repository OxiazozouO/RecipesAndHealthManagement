using _Microsoft.Android.Resource.Designer;
using Android.Attribute;
using Android.Component;
using Android.Helper;
using Android.Models;
using Android.Util;
using Android.ViewModel;
using Android.Views;
using AnyLibrary.Constants;
using AnyLibrary.Helper;
using Bumptech.Glide;
using Google.Android.Flexbox;
using MikePhil.Charting.Charts;
using MikePhil.Charting.Data;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;
using static Android.Text.InputTypes;
using Color = Android.Graphics.Color;

namespace Android.Holder;

[ViewClassBind(Layout.activity_collection)]
public class ActivityCollectionHolder(App.Activity activity) : ViewHolder<ScrollView>(activity)
{
    [ViewBind(Id.id_collection_name)] public TextView Name;

    [ViewBind(Id.id_collection_emoji_layout)]
    public FlexboxLayout EmojiLayout;

    [ViewBind(Id.id_collection_category_layout)]
    public FlexboxLayout CategoryLayout;

    [ViewBind(Id.id_collection_add_emoji)] public ImageView AddEmoji;

    [ViewBind(Id.id_collection_add_category)]
    public ImageView AddCategory;


    [ViewHolderBind(Id.id_collection_comment)]
    public CommentHolder CommentHolder;

    [ViewBind(Id.id_collection_like)] public ImageView Like;

    [ViewBind(Id.id_collection_like_num)] public TextView LikeNum;

    [ViewBind(Id.id_collection_share)] public ImageView Share;

    [ViewBind(Id.id_collection_report)] public ImageView Report;
    [ViewBind(Id.id_collection_editor)] public RichEditor Editor;

    protected override void Init()
    {
        AddEmoji.CallClick(() =>
        {
            MsgBoxHelper.Builder()
                .AddEditText("", ClassText, 12, "请输入表情")
                .Show(list => { model.AddCategory((string)list[0], CategoryType.Emoji, BindCategory); });
        });

        AddCategory.CallClick(() =>
        {
            MsgBoxHelper.Builder()
                .AddEditText("", ClassText, 12, "请输入分类名称")
                .Show(list => { model.AddCategory((string)list[0], CategoryType.Category, BindCategory); });
        });

        Share.CallClick(() =>
        {
            // ApiService.Share
        });

        Report.CallClick(() => ActivityHelper.GotoReport(model.CollectionId, IdCategory.Collection, model.Title));
    }

    private CollectionModel model;

    public void Bind(CollectionModel model)
    {
        this.model = model;

        CommentHolder.Bind(Root, model.CollectionId, IdCategory.Collection, activity);
        LikeNum.Text = model.FavoriteCount.ToString();
        Like.SetImageResource(model.IsLike ? Drawable.ic_collect : Drawable.ic_no_collect);
        Editor.AfterInitialLoad += (sender, b) =>
        {
            Editor.EditorHeight = 200;
            Editor.EditorFontSize = 15;
            Editor.SetPadding(10, 10, 10, 10);
            Editor.SetReadOnly();
            Editor.Html = model.Html;
        };
        BindCategory();
    }

    public void BindCategory()
    {
        if (EmojiLayout.FlexItemCount > 1)
            EmojiLayout.RemoveViews(0, EmojiLayout.FlexItemCount - 1);
        if (CategoryLayout.FlexItemCount > 1)
            CategoryLayout.RemoveViews(0, CategoryLayout.FlexItemCount - 1);

        foreach (var t in model.Category)
        {
            var holder = new ItemCategoryHolder(activity);
            holder.Bind(t);
            switch (t.TypeId)
            {
                case CategoryType.Emoji:
                    EmojiLayout.AddView(holder.Root, 0);
                    break;
                case CategoryType.Category:
                    CategoryLayout.AddView(holder.Root, 0);
                    break;
            }

            holder.CategoryLayout.CallClick(() =>
            {
                t.OnLike(model.CollectionId, IdCategory.Collection);
                holder.Bind(t);
            });
        }

        Editor.Html = model.Html;
    }
}