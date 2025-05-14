using Android.Attribute;
using Android.Helper;
using Android.HttpClients;
using Android.Models;
using Android.Views;
using AnyLibrary.Constants;
using AnyLibrary.Helper;
using Bumptech.Glide;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;

namespace Android.Component;

[ViewClassBind(Layout.com_comment)]
public class CommentHolder(View view) : ViewHolder<View>(view)
{
    [ViewBind(Id.id_comment_user_file_url)]
    public ImageView FileUrl;

    [ViewBind(Id.id_comment_user_input)] public EditText UserInput;

    [ViewBind(Id.id_comment_sub)] public TextView Sub;

    [ViewBind(Id.id_comment_main)] public LinearLayout Main;

    protected override void Init()
    {
    }

    private App.Activity activity;

    public void Bind(ScrollView scrollView, long tid, sbyte idCategory, App.Activity activity)
    {
        this.activity = activity;
        Glide.With(Root).Load(AppConfigHelper.MyInfo.FileUrl).Into(FileUrl);
        Sub.CallClick(() =>
        {
            var text = UserInput.Text;
            if (string.IsNullOrEmpty(text)) return;

            var req = ApiEndpoints.AddComment(new
            {
                TId = tid,
                TypeId = idCategory,
                Content = text,
                userId = AppConfigHelper.AppConfig.Id
            });

            if (req.Execute(out var res))
            {
                MsgBoxHelper.Builder().TryError(res.Message);
                UserInput.Text = "";
            }
        });

        scrollView.BindUp(pos =>
        {
            var list = ApiService.GetComments(tid, idCategory, pos);
            bool ret = list is null || list.Count == 0;
            if (ret)
            {
                var root = activity.LayoutInflater.Inflate(Layout.no_more_text_view, null);
                Main.AddView(root);
            }
            else
            {
                Bind(list);
            }

            return ret;
        });
    }

    private void Bind(List<CommentModel> models)
    {
        foreach (var item in models)
        {
            var holder = new ItemComCommentHolder(activity);
            holder.Bind(item);
            Main.AddView(holder.Root);
        }
    }
}

[ViewClassBind(Layout.item_com_comment)]
public class ItemComCommentHolder(App.Activity activity) : ViewHolder<View>(activity)
{
    [ViewBind(Id.id_comment_file_url)] public ImageView FileUrl;
    [ViewBind(Id.id_comment_name)] public TextView Name;
    [ViewBind(Id.id_comment_content)] public TextView Content;
    [ViewBind(Id.id_comment_time)] public TextView Time;
    [ViewBind(Id.id_comment_report)] public TextView Report;
    private CommentModel model;

    protected override void Init()
    {
        Report.CallClick(() =>
        {
            ActivityHelper.GotoReport(model.CommentId, IdCategory.Comment, model.Content);
        });
    }

    public void Bind(CommentModel model)
    {
        this.model = model;
        Glide.With(Root).Load(model.FileUrl).Into(FileUrl);
        Name.Text = model.UName;
        Content.Text = model.Content;
        Time.Text = model.CreateDate.TimeStr1();
        if (model.UserId == AppConfigHelper.AppConfig.Id)
        {
            Report.Visibility = ViewStates.Gone;
        }
    }
}