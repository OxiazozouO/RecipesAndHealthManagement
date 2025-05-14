using Android.Attribute;
using Android.Configurations;
using Android.Helper;
using Android.HttpClients;
using Android.ViewModel;
using Android.Views;
using AnyLibrary.Helper;
using Bumptech.Glide;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;

namespace Android.Holder;

[ViewClassBind(Layout.activity_login)]
public class ActivityLoginHolder(App.Activity activity) : ViewHolder<View>(activity)
{
    [ViewBind(Id.id_login_img)] public ImageView Img;
    [ViewBind(Id.id_login_username)] public EditText Username;

    [ViewBind(Id.id_login_password)] public EditText Password;

    [ViewBind(Id.id_login_goto_register)] public LinearLayout GotoRegister;

    [ViewBind(Id.id_login_sub)] public TextView LoginSub;


    protected override void Init()
    {
        GotoRegister.BindTo(ActivityHelper.GotoRegister);

        var viewModel = new LoginViewModel();
        Username.BindTo(s =>
        {
            viewModel.Identifier = s;
            Glide.With(activity).Load(Drawable.app).Into(Img);
            if (string.IsNullOrEmpty(s)) return;

            var req = ApiEndpoints.LoginImg(new { Identifier = s });

            req.Execute(res =>
            {
                if (res.Code == 1)
                {
                    var img = res.Data.ToString();
                    if (img.Contains("http"))
                    {
                        Glide.With(activity).Load(img).Error(Drawable.app).Into(Img);
                    }
                    else
                    {
                        Glide.With(activity).Load(Drawable.logout).Into(Img);
                    }
                }
            });
        });
        Password.BindTo(s => viewModel.Password = s);
        LoginSub.CallClick(viewModel.Login);
    }
}