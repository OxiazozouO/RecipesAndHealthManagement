using _Microsoft.Android.Resource.Designer;
using Android.Attribute;
using Android.Helper;
using Android.HttpClients;
using Android.Models;
using Android.Text;
using Android.Views;
using AnyLibrary.Helper;
using Bumptech.Glide;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;

namespace Android.Holder;

[ViewClassBind(ResourceConstant.Layout.activity_edit_user_info)]
public class ActivityEditUserInfoHolder(App.Activity activity) : ViewHolder<View>(activity)
{
    public MyInfoModel UserInfo;

    [ViewBind(Id.id_edit_userinfo_sub)] public TextView Sub;

    [ViewBind(Id.id_edit_userinfo_user_url)]
    public ImageView UserUrl;

    [ViewBind(Id.id_edit_user_id_text)] public TextView UserId;

    [ViewBind(Id.id_edit_user_name_text)] public TextView Name;

    [ViewBind(Id.id_edit_userinfo_sex_text)]
    public TextView Sex;

    [ViewBind(Id.id_edit_userinfo_birthday_text)]
    public TextView Birthday;

    [ViewBind(Id.id_edit_back)] public ImageView Back;

    [ViewBind(Id.id_edit_user_url)] public LinearLayout Item1;

    [ViewBind(Id.id_edit_user_name)] public LinearLayout Item2;

    [ViewBind(Id.id_edit_userinfo_sex)] public LinearLayout Item3;

    [ViewBind(Id.id_edit_userinfo_birthday)]
    public LinearLayout Item4;

    protected override void Init()
    {
        UserId.Text = AppConfigHelper.AppConfig.Id.ToString();

        UserInfo = ApiService.GetInfo(AppConfigHelper.AppConfig.Id);
        Bind(UserInfo);
        Name.BindTo(s => UserInfo.UserName = s);
        Sex.BindTo(s => UserInfo.Gender = s == "男");
        Birthday.BindTo(s =>
        {
            try
            {
                UserInfo.BirthDate = DateTime.Parse(s);
            }
            catch (Exception)
            {
                // ignored
            }
        });

        Sub.CallClick(() =>
        {
            ApiEndpoints.ChangeInfo(new
            {
                Id = AppConfigHelper.AppConfig.Id,
                Name = UserInfo.UserName,
                Gender = UserInfo.Gender,
                BirthDate = UserInfo.BirthDate,
                FileUrl = UserInfo.FileUrl
            }).Execute(res =>
            {
                MsgBoxHelper.Builder(res.Message).OkCancel(() =>
                {
                    AppConfigHelper.InitMyInfo();
                    AppConfigHelper.Save();
                    activity.Finish();
                });
            });
        });

        Item1.Click += OnClick;
        Item2.Click += OnClick;
        Item3.Click += OnClick;
        Item4.Click += OnClick;

        Back.CallClick(activity.Finish);
    }

    private void OnClick(object? sender, EventArgs e)
    {
        if (sender is not View v) return;
        switch (v.Id)
        {
            case Id.id_edit_userinfo_birthday:
                MsgBoxHelper.Builder("请输入生日", "生日输入框")
                    .AddDatePicker(UserInfo.BirthDate)
                    .Show(date =>
                    {
                        UserInfo.BirthDate = (DateTime)date[0];
                        Birthday.Text = UserInfo.BirthDate.TimeStr2();
                    });
                break;
            case Id.id_edit_userinfo_sex:
                MsgBoxHelper
                    .Builder("请输入性别", "昵称输入框")
                    .AddLisView([
                        new MsgItem { Text = "男", Icon = Drawable.ic_male },
                        new MsgItem { Text = "女", Icon = Drawable.ic_female }
                    ], UserInfo.Gender ? 0 : 1)
                    .Show(date => Sex.Text = (int)date[0] == 0 ? "男" : "女");
                break;
            case Id.id_edit_user_name:
                MsgBoxHelper
                    .Builder("", "昵称输入框")
                    .AddEditText(Name.Text, InputTypes.ClassText, 20, "请输入昵称")
                    .Show(date => Name.Text = (string)date[0]);
                break;
            case Id.id_edit_user_url:
                activity.SelectImage();
                break;
        }
    }

    public void Bind(MyInfoModel model)
    {
        Name.Text = model.UserName;
        Glide.With(Root)
            .Load(model.FileUrl)
            .Error(Drawable.ic_no_login)
            .Into(UserUrl);
        Birthday.Text = model.BirthDate.ToString("yyyy-MM-dd");
        Sex.Text = model.Gender ? "男" : "女";
    }

    public void SetFileUrl(Android.Net.Uri? uri)
    {
        if (!activity.ContentResolver.FileUpload(uri, out string outFileName)) return;
        UserInfo.FileUrl = outFileName;
        Glide.With(Root).Load(uri).Into(UserUrl);
    }
}