using _Microsoft.Android.Resource.Designer;
using Android.Attribute;
using Android.Helper;
using Android.HttpClients;
using Android.Models;
using Android.Text;
using Android.Views;
using AnyLibrary.Constants;
using Bumptech.Glide;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;
using Uri = Android.Net.Uri;

namespace Android.Holder;

[ViewClassBind(ResourceConstant.Layout.activity_favorite_operation)]
public class FavoriteOperationActivityHolder(App.Activity activity) : ViewHolder<View>(activity)
{
    [ViewBind(Id.id_favorite_operation_file_url)]
    public ImageView FileUrl;

    [ViewBind(Id.id_favorite_operation_category)]
    public TextView Category;

    [ViewBind(Id.id_favorite_operation_name)]
    public TextView Name;

    [ViewBind(Id.id_favorite_operation_ref)]
    public TextView Ref;

    [ViewBind(Id.id_favorite_operation_name_layout)]
    public LinearLayout NameLayout;

    [ViewBind(Id.id_favorite_operation_ref_layout)]
    public LinearLayout RefLayout;

    [ViewBind(Id.id_favorite_operation_sub)]
    public TextView Sub;

    public FavoriteModel model;

    protected override void Init()
    {
        model = new FavoriteModel();
        Name.BindTo(s => model.FName = s);
        Ref.BindTo(s => model.Refer = s);

        NameLayout.CallClick(() =>
        {
            MsgBoxHelper
                .Builder("", "收藏夹名称")
                .AddEditText(Name.Text, InputTypes.ClassText, 20, "请输入收藏夹名称")
                .Show(date => Name.Text = (string)date[0]);
        });

        RefLayout.CallClick(() =>
        {
            MsgBoxHelper
                .Builder("", "收藏夹简介")
                .AddEditText(Ref.Text, InputTypes.ClassText, 200, "请输入收藏夹简介")
                .Show(date => Ref.Text = (string)date[0]);
        });

        FileUrl.CallClick(() => activity.SelectImage());
    }

    public void Bind(int flag)
    {
        model.Flag = (sbyte)flag;
        Category.Text = IdCategory.GetName(model.Flag);
    }

    public void Bind(FavoriteModel mod)
    {
        model.FavoriteId = mod.FavoriteId;
        Bind(mod.Flag);
        Name.Text = mod.FName;
        Ref.Text = mod.Refer;
        

        Glide
            .With(activity)
            .Load(mod.FileUrl)
            .Into(FileUrl);
    }

    public void SetFileUrl(Uri uri)
    {
        if (!activity.ContentResolver.FileUpload(uri, out string outFileName)) return;
        model.FileUrl = outFileName;
        Glide
            .With(activity)
            .Load(uri)
            .Into(FileUrl);
    }
}