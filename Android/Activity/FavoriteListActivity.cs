using Android.Content;
using Android.Holder;

namespace Android.Activity;

[Activity]
public class FavoriteListActivity : App.Activity
{
    private ActivityFavoriteListHolder holder;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        holder = new ActivityFavoriteListHolder(this);
        SetContentView(holder.Root);
    }

    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        base.OnActivityResult(requestCode, resultCode, data);
        holder.Adapter.ReInitModels();
    }

    protected override void OnRestart()
    {
        base.OnRestart();
        holder.Adapter.ReInitModels();
    }
}