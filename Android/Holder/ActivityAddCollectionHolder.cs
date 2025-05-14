using Android.Activity;
using Android.Attribute;
using Android.Component;
using Android.Helper;
using Android.HttpClients;
using Android.ViewModel;
using Android.Views;
using AnyLibrary.Helper;
using Bumptech.Glide;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;
using static Android.Text.InputTypes;
using Uri = Android.Net.Uri;

namespace Android.Holder;

[ViewClassBind(Layout.activity_add_collection)]
public class ActivityAddCollectionHolder(App.Activity activity) : ViewHolder<View>(activity)
{
    [ViewBind(Id.id_add_collection_img)] public ImageView Img;

    [ViewBind(Id.id_add_collection_title)] public TextView Title;

    [ViewBind(Id.id_add_collection_ref)] public TextView Refer;

    [ViewBind(Id.id_add_collection_search_add)]
    public ImageView SearchAdd;

    [ViewBind(Id.id_add_collection_editor)]
    public RichEditor Editor;

    [ViewBind(Id.id_add_collection_preview)]
    public EditText Preview;

    [ViewBind(Id.id_add_collection_release_info)]
    public TextView ReleaseInfo;

    [ViewBind(Id.id_add_collection_sub)] public TextView Sub;

    [ViewHolderBind(Id.id_add_collection_top_menus)]
    public CommRichEditorTopMenusHolder EditorTopMenuHolder;

    [ViewHolderBind(Id.id_add_collection_menus)]
    public RichEditorMenusHolder EditorMenuHolder;

    [ViewHolderBind(Id.id_add_collection_bottom_menus)]
    public CommRichEditorBottomMenusHolder EditorBottomMenuHolder;

    private AddCollectionViewModel model;
    public long TId;
    public long ReleaseId;

    public void Put(CollectionModel model2)
    {
        model.FileUrl = model2.FileUrl;
        model.Summary = model2.Refer;
        model.Title = model2.Title;
        Editor.AfterInitialLoad += (sender, b) =>
        {
            Editor.Html = model2.Html;
            model.Content.Html = model2.Html;
        };
        Update();
    }

    public void Update()
    {
        Glide.With(Root).Load(model.FileUrl).Into(Img);
        Title.Text = model.Title;
        Refer.Text = model.Summary;
        ReleaseInfo.Text = model.ReleaseInfo;
    }

    protected override void Init()
    {
        model = new AddCollectionViewModel { Content = new HtmlData() };


        Img.CallClick(() => { activity.SelectImage(100); });

        Title.CallClick(() =>
        {
            MsgBoxHelper.Builder()
                .AddEditText(model.Title, ClassText, 30, "请输入合集标题（最多30字）")
                .Show(list =>
                {
                    model.Title = (string)list[0];
                    Title.Text = model.Title;
                });
        });

        Refer.CallClick(() =>
        {
            MsgBoxHelper.Builder()
                .AddEditText(model.Summary, ClassText, 200, "请输入合集的简介（最多200字）")
                .Show(list =>
                {
                    model.Summary = (string)list[0];
                    Refer.Text = model.Summary;
                });
        });

        Editor.AfterInitialLoad += (sender, b) =>
        {
            Editor.EditorHeight = 200;
            Editor.EditorFontSize = 15;
            Editor.SetPadding(10, 10, 10, 10);
            Editor.Placeholder = "请编辑合集内容：";
        };

        Editor.TextChanged += (sender, s) =>
        {
            try
            {
                model.Content.Html = s;
                // Preview.Text = model.Content.Html;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        };
        ReleaseInfo.CallClick(() =>
        {
            MsgBoxHelper.Builder()
                .AddEditText(model.ReleaseInfo, ClassText, 200, "请输入参考资料来源（最多200字）")
                .Show(list =>
                {
                    model.ReleaseInfo = (string)list[0];
                    ReleaseInfo.Text = model.ReleaseInfo;
                });
        });


        EditorTopMenuHolder.Bind(Editor, EditorMenuHolder);
        EditorMenuHolder.Bind(activity, Editor, EditorTopMenuHolder);
        EditorBottomMenuHolder.Bind(Editor);

        EditorBottomMenuHolder.InsertImage.CallClick(() => { activity.SelectImage(); });
        EditorBottomMenuHolder.InsertLink.CallClick(() =>
        {
            MsgBoxHelper.Builder("", "添加链接")
                .AddEditText("", ClassText, 100, "请输入文本")
                .AddEditText("", ClassText, 100, "请输入链接")
                .Show(list =>
                {
                    var text = (string)list[0];
                    var link = (string)list[1];
                    if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(link))
                        return;
                    Editor.InsertLink(link, text);
                });
        });

        SearchAdd.CallClick(() => { ActivityHelper.GotoSearch(SearchFlag.All); });

        Sub.CallClick(() =>
        {
            if (model.AddCollection(ReleaseId, TId)) activity.Finish();
        });
    }

    public void InsertTab(int idCategory, long id)
    {
        ApiEndpoints.IndexSearchBaseInfo(new
        {
            AppConfigHelper.AppConfig.Id,
            Flag = idCategory,
            TId = id
        }).Execute(res =>
        {
            var item = res.Data.ToEntity<CollectionTabModel>();
            Editor.InsertTab(item.FileUrl, item.Title, item.Refer, item.IdCategory, item.Id);
        });
    }

    public void SetEditorFileUrl(Uri? uri)
    {
        if (!activity.ContentResolver.FileUpload(uri, out var outFileName, out var url)) return;
        Editor.InsertImage(outFileName, url, "", 400);
    }

    public void SetFileUrl(Uri? uri)
    {
        if (!activity.ContentResolver.FileUpload(uri, out var outFileName)) return;
        model.FileUrl = outFileName;
        Glide.With(Root).Load(uri).Into(Img);
    }
}