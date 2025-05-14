using Android.Attribute;
using Android.Helper;
using Android.HttpClients;
using Android.Models;
using Android.Views;
using AnyLibrary.Constants;
using AnyLibrary.Helper;
using Bumptech.Glide;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;
using static Android.Views.ViewStates;

namespace Android.Holder;

[ViewClassBind(Layout.page_release_list_item)]
public class PageReleaseListItemHolder(App.Activity activity) : ViewHolder<View>(activity)
{
    [ViewBind(Id.id_release_list_item_url)]
    public ImageView Url;

    [ViewBind(Id.id_release_list_item_time)]
    public TextView Time;

    [ViewBind(Id.id_release_list_item_status)]
    public TextView StatusView;

    [ViewBind(Id.id_release_list_item_title)]
    public TextView Title;

    [ViewBind(Id.id_release_list_item_ref)]
    public TextView RefText;

    [ViewBind(Id.id_release_list_item_off)]
    public ImageView Off;

    [ViewBind(Id.id_release_list_item_on)] public ImageView On;

    [ViewBind(Id.id_release_list_item_edit)]
    public ImageView Edit;

    [ViewBind(Id.id_release_list_item_close)]
    public ImageView Close;

    [ViewBind(Id.id_release_list_item_info_sub)]
    public LinearLayout InfoSub;

    [ViewBind(Id.id_add_recipe_nutritional_arrow)]
    public ImageView Arrow;

    [ViewBind(Id.id_release_list_item_info_main)]
    public LinearLayout InfoMain;

    private ReleaseModel item;
    private int status;


    public record ReleaseFlags(string Name, int Bg, ViewStates Edit, ViewStates On, ViewStates Off, ViewStates Deleted);

    public static readonly Dictionary<int, ReleaseFlags> Items = new()
    {
        [Status.Pending] = new ReleaseFlags("审核中", Drawable.shape_button_bg2, Visible, Gone, Gone, Visible),
        [Status.NeedEdit] = new ReleaseFlags("需修改", Drawable.shape_button_bg6, Visible, Gone, Gone, Visible),
        [Status.Confirm] = new ReleaseFlags("待通过", Drawable.shape_button_bg6, Gone, Gone, Gone, Gone),
        [Status.Approve] = new ReleaseFlags("已批准", Drawable.shape_button_bg, Gone, Gone, Gone, Gone),
        [Status.On] = new ReleaseFlags("已上架", Drawable.shape_button_bg3, Visible, Gone, Visible, Visible),
        [Status.Off] = new ReleaseFlags("已下架", Drawable.shape_button_bg4, Visible, Visible, Gone, Visible),
        [Status.ForceOff] = new ReleaseFlags("已被强制下架", Drawable.shape_button_bg4, Gone, Gone, Gone, Visible),
        [Status.ReportOff] = new ReleaseFlags("已被举报下架", Drawable.shape_button_bg4, Gone, Gone, Gone, Visible),
        [Status.Deleted] = new ReleaseFlags("已删除", Drawable.shape_button_bg5, Gone, Gone, Gone, Gone),
        [Status.Cancel] = new ReleaseFlags("已取消", Drawable.shape_button_bg5, Gone, Gone, Gone, Gone),
        [Status.Reject] = new ReleaseFlags("被驳回", Drawable.shape_button_bg5, Gone, Gone, Gone, Gone),
    };

    public void Bind(ReleaseModel item, int status)
    {
        this.item = item;
        this.status = status;
        if (!string.IsNullOrEmpty(item.FileUrl))
            Glide.With(Root).Load(item.FileUrl).Into(Url);

        Time.Text = item.CreateDate.TimeStr1();
        Title.Text = item.Title;
        RefText.Text = item.ReviewFeedback;
        int s = item.Status;
        get:
        if (Items.TryGetValue(s, out var flag))
        {
            StatusView.Text = flag.Name;
            if (s == Status.Approve)
            {
                s = status;
                goto get;
            }

            StatusView.SetBackgroundResource(flag.Bg);

            Off.Visibility = flag.Off;
            On.Visibility = flag.On;
            Edit.Visibility = flag.Edit;
            Close.Visibility = flag.Deleted;
        }


        InfoMain.RemoveAllViews();
        foreach (var history in item.ReleaseFlowHistories)
        {
            var holder = new ItemFlowHistoryHolder(activity);
            holder.Bind(history);
            InfoMain.AddView(holder.Root, 0);
        }
    }


    protected override void Init()
    {
        Off.CallClick(() =>
        {
            if (Off.Visibility == Gone) return;
            MsgBoxHelper.Builder("确定下架该内容？").OkCancel(() =>
            {
                if (ApiService.ReverseStatus(item.IdCategory, item.TId, status))
                {
                    MsgBoxHelper.Builder("下架成功").ShowDialog();
                }
                else
                {
                    MsgBoxHelper.Builder("下架失败").ShowDialog();
                }
            });
        });

        On.CallClick(() =>
        {
            if (On.Visibility == Gone) return;
            MsgBoxHelper.Builder("确定上架该内容？").OkCancel(() =>
            {
                if (ApiService.ReverseStatus(item.IdCategory, item.TId, status))
                {
                    MsgBoxHelper.Builder("上架成功").ShowDialog();
                }
                else
                {
                    MsgBoxHelper.Builder("上架失败").ShowDialog();
                }
            });
        });

        Edit.CallClick(() =>
        {
            if (Edit.Visibility == Gone) return;
            switch (item.IdCategory)
            {
                case IdCategory.Recipe:
                    ActivityHelper.GotoAddRecipe(item.ReleaseId, item.TId);
                    break;
                case IdCategory.Collection:
                    ActivityHelper.GotoAddCollection(item.ReleaseId, item.TId);
                    break;
            }
        });

        Close.CallClick(() =>
        {
            if (Close.Visibility == Gone) return;
            MsgBoxHelper.Builder("确定取消申请？").OkCancel(() =>
            {
                ApiEndpoints.DeleteRelease(new
                {
                    Id = AppConfigHelper.AppConfig.Id,
                    ReleaseId = item.ReleaseId
                }).Execute();
            });
        });

        InfoSub.CallClick(() =>
        {
            var b = Math.Abs(Arrow.Rotation - 90) < 0.01;
            Arrow.Rotation = b ? -90 : 90;
            InfoMain.Visibility = b ? Gone : Visible;
        });
    }
}