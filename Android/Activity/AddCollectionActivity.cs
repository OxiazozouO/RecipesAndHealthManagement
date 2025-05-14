using Android.Content;
using Android.Helper;
using Android.Holder;
using Android.HttpClients;
using Android.ViewModel;
using AnyLibrary.Helper;

namespace Android.Activity;

[Activity]
public class AddCollectionActivity : App.Activity
{
    private ActivityAddCollectionHolder holder;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        try
        {
            holder = new ActivityAddCollectionHolder(this);
            SetContentView(holder.Root);
            if (Intent?.Extras is not { } b) return;
            long releaseId = b.GetLong("releaseId", -1);
            long tid = b.GetLong("tid", 0);
            holder.ReleaseId = releaseId;
            holder.TId = tid;

            var req = ApiEndpoints.GetReleaseCollection(new
            {
                Id = AppConfigHelper.AppConfig.Id,
                ReleaseId = releaseId
            });
            req.Execute(res =>
            {
                var collection = res.Data.ToEntity<CollectionModel>();
                holder.Put(collection);
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        base.OnActivityResult(requestCode, resultCode, data);
        if (resultCode != Result.Ok || data == null) return;
        switch (requestCode)
        {
            case 0:
                try
                {
                    int idCategory = data.GetIntExtra("idCategory", 0);
                    long id = data.GetLongExtra("id", 0);
                    if (idCategory > 0 && id > 0)
                        holder.InsertTab(idCategory, id);
                    else
                        MsgBoxHelper
                            .Builder()
                            .TryError("发生未知错误");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                break;
            case 1:
                holder.SetEditorFileUrl(data.Data);
                break;
            case 100:
                holder.SetFileUrl(data.Data);
                break;
        }
    }
}