using Android.Attribute;
using Android.Helper;
using Android.HttpClients;
using Android.ViewModel;
using Android.Views;
using Bumptech.Glide;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;
using Uri = Android.Net.Uri;

namespace Android.Holder;

[ViewClassBind(Layout.activity_register)]
public class ActivityRegisterHolder(App.Activity activity) : ViewHolder<View>(activity)
{
    [ViewBind(Id.id_register_user_img)]
    public ImageView UserImg;

    [ViewBind(Id.id_register_username)]
    public EditText Username;

    [ViewBind(Id.id_register_email)]
    public EditText Email;

    [ViewBind(Id.id_register_phone)]
    public EditText Phone;

    [ViewBind(Id.id_register_password)]
    public EditText Password;

    [ViewBind(Id.id_register_confirm_password)]
    public EditText ConfirmPassword;

    [ViewBind(Id.id_register_goto_login)]
    public LinearLayout GotoLogin;

    [ViewBind(Id.id_register_sub)]
    public TextView Sub;

    private RegisterViewModel viewModel = new RegisterViewModel();

    protected override void Init()
    {
        Username.BindTo(s => viewModel.Name = s);
        Email.BindTo(s => viewModel.Email = s);
        Phone.BindTo(s => viewModel.Phone = s);
        Password.BindTo(s => viewModel.Password = s);
        ConfirmPassword.BindTo(s => viewModel.ConfirmPassword = s);

        Sub.CallClick(viewModel.Register);
        GotoLogin.BindTo(ActivityHelper.GotoLogin);

        UserImg.CallClick(() => activity.SelectImage());
    }

    public void Bind(Uri? uri)
    {
        if (!activity.ContentResolver.FileUpload(uri, out string outFileName)) return;
        viewModel.UserImgUrl = outFileName;
        Glide.With(Root).Load(uri).Into(UserImg);
    }
}