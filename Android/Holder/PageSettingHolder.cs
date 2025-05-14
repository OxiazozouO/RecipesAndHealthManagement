using Android.Attribute;
using Android.Helper;
using Android.Views;
using Bumptech.Glide;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;
using static Android.Helper.AppConfigHelper;

namespace Android.Holder;

[ViewClassBind(Layout.page_settings)]
public class PageSettingHolder(App.Activity activity) : ViewHolder<View>(activity)
{
    [ViewBind(Id.id_setting_head_layout)] public LinearLayout UserLayout;

    [ViewBind(Id.id_setting_head_img)] public ImageView UserImg;

    [ViewBind(Id.tv_username_mine)] public TextView Username;

    [ViewBind(Id.id_settings_my_collection)]
    public RelativeLayout MyCollection;

    [ViewBind(Id.id_settings_change_password)]
    public RelativeLayout ChangePassword;

    [ViewBind(Id.id_settings_break)] public RelativeLayout Break;

    [ViewBind(Id.id_settings_logout)] public RelativeLayout Logout;

    protected override void Init()
    {
        UserLayout.Click += UserLayoutOnClick;
        MyCollection.Click += OnClick;
        ChangePassword.Click += OnClick;
        Break.Click += OnClick;
        Logout.Click += OnClick;
        AppConfigChanged += (_, _) => { Bind(); };
        Bind();
    }

    public void Bind()
    {
        if (string.IsNullOrEmpty(MyInfo?.UserName)) return;
        Username.Text = MyInfo.UserName;
        Glide.With(Root)
            .Load(MyInfo.FileUrl)
            .Error(Drawable.ic_no_head)
            .Into(UserImg);
    }

    protected void UserLayoutOnClick(object? sender, EventArgs e)
    {
        ActivityHelper.GotoUserInfo();
    }

    private static void OnClick(object? sender, EventArgs e)
    {
        if (sender is not View v) return;
        switch (v.Id)
        {
            case Id.id_settings_my_collection:
                ActivityHelper.GotoFavoriteList();
                break;
            case Id.id_settings_change_password:
                ActivityHelper.GotoChangePassword();
                break;
            case Id.id_settings_break:
                MsgBoxHelper.Builder("确定退出？").OkCancel(() =>
                {
                    Del();
                    ActivityHelper.GotoLogin();
                });
                break;
            case Id.id_settings_logout:
                ActivityHelper.GotoLogout();
                break;
        }
    }
}