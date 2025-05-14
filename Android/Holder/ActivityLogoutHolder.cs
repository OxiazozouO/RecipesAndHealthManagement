using Android.Attribute;
using Android.Helper;
using Android.ViewModel;
using Android.Views;
using Bumptech.Glide;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;

namespace Android.Holder;

[ViewClassBind(Layout.activity_logout)]
public class LogoutHolder(Android.App.Activity activity) : ViewHolder<View>(activity)
{
    [ViewBind(Id.id_logout_user_url)]
    public ImageView UserUrl;

    [ViewBind(Id.id_logout_email)]
    public EditText Email;

    [ViewBind(Id.id_logout_password)]
    public EditText Password;

    [ViewBind(Id.id_logout_sub)] public TextView Sub;

    protected override void Init()
    {
        var model = new LogoutViewModel();
        Glide.With(Root).Load(AppConfigHelper.MyInfo.FileUrl).Into(UserUrl);
        Email.BindTo(s => model.Identifier = s);
        Password.BindTo(s => model.Password = s);
        Sub.CallClick(() =>
        {
            MsgBoxHelper.Builder("确定注销？").OkCancel(() =>
            {
                if (model.Logout()) ActivityHelper.GotoLogin();
            });
        });
    }
}