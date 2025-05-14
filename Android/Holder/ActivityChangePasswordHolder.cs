using Android.Attribute;
using Android.Helper;
using Android.ViewModel;
using Android.Views;
using Bumptech.Glide;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;

namespace Android.Holder;

[ViewClassBind(Layout.activity_change_password)]
public class ActivityChangePasswordHolder(App.Activity activity) : ViewHolder<View>(activity)
{
    [ViewBind(Id.id_change_user_url)]
    public ImageView UserUrl;

    [ViewBind(Id.id_change_email)]
    public EditText Email;

    [ViewBind(Id.id_change_old_password)]
    public EditText OldPassword;

    [ViewBind(Id.id_change_new_password)]
    public EditText NewPassword;

    [ViewBind(Id.id_change_confirm_password)]
    public EditText ConfirmPassword;

    [ViewBind(Id.id_change_sub)] public TextView Sub;

    protected override void Init()
    {
        var model = new ChangePasswordViewModel();
        Glide.With(Root).Load(AppConfigHelper.MyInfo.FileUrl).Into(UserUrl);
        Email.BindTo(s => model.Identifier = s);
        OldPassword.BindTo(s => model.Password = s);
        NewPassword.BindTo(s => model.NewPassword = s);
        ConfirmPassword.BindTo(s => model.ConfirmPassword = s);
        Sub.CallClick(() =>
        {
            bool b = model.ChangePassword();
            if (b) activity.Finish();
        });
    }
}