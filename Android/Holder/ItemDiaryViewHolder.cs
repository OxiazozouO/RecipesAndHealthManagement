using Android.Adapter;
using Android.Attribute;
using Android.Helper;
using Android.HttpClients;
using Android.ViewModel;
using Android.Views;
using AnyLibrary.Helper;
using Bumptech.Glide;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;

namespace Android.Holder;

[ViewClassBind(Layout.item_diary_view)]
public class ItemDiaryViewHolder(App.Activity activity) : ViewHolder<View>(activity)
{
    [ViewBind(Id.id_diary_file_url)] public ImageView FileUrl;

    [ViewBind(Id.id_diary_name)] public TextView Name;

    [ViewBind(Id.id_diary_kcal)] public TextView Kcal;

    [ViewBind(Id.id_diary_time)] public TextView Time;
    [ViewBind(Id.id_diary_dosage)] public TextView Dosage;
    [ViewBind(Id.id_diary_remove)] public ImageView Remove;

    protected override void Init()
    {
    }

    public void Bind(DataPakedAdapter adapter, EatingDiaryAtViewModel diaryAt)
    {
        Glide.With(Root).Load(diaryAt.EatingDiary.FileUrl).Into(FileUrl);
        Name.Text = diaryAt.EatingDiary.Name;
        Kcal.Text = $"{diaryAt.Energy:0.00} kcal/天";
        Time.Text = diaryAt.EatingDiary.TieUpDate.TimeStr1();
        Dosage.Text = $"{diaryAt.Dosage:0.000} {diaryAt.OutUnit}";

        Remove.CallClick(() =>
        {
            MsgBoxHelper.Builder("确定删除本条记录？").OkCancel(() =>
            {
                ApiEndpoints.DeleteUserEatingDiary(new
                {
                    Id = AppConfigHelper.AppConfig.Id,
                    EdId = diaryAt.EatingDiary.EdId,
                    Flag = diaryAt.EatingDiary.Flag
                }).Execute(res =>
                {
                    adapter.BindView();
                    MsgBoxHelper.ShowToast("删除成功");
                });
            });
        });
    }
}