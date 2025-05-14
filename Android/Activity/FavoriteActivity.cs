using Android.Content;
using Android.Holder;

namespace Android.Activity;

[Activity]
public class FavoriteActivity : App.Activity
{
    private ActivityFavoriteHolder holder;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        if (Intent?.Extras is { } b)
        {
            int idCategory = b.GetInt("idCategory");
            string name = b.GetString("name");
            long tid = b.GetLong("tid");
            string file = b.GetString("fileUrl");
            holder = new ActivityFavoriteHolder(this);
            holder.Bind(idCategory, name, tid, file);
            SetContentView(holder.Root);
            holder.Adapter.ReInitModels();
        }
        else
        {
            Finish();
        }
    }

    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        base.OnActivityResult(requestCode, resultCode, data);
        holder.Adapter.ReInitModels();
    }
}