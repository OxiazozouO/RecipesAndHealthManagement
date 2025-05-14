using Android.Holder;

namespace Android.Activity;

[Activity]
public class FavoriteInfoActivity : App.Activity
{
    private ActivityFavoriteInfoHolder holder;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        try
        {
            if (Intent?.Extras is not { } b) return;
            long favoriteId = b.GetLong("favoriteId");
            holder = new ActivityFavoriteInfoHolder(this);
            SetContentView(holder.Root);
            holder.Bind(favoriteId);
            return;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        Finish();
    }

    public override void OnBackPressed()
    {
        if (holder.Adapter.IsMultiple)
        {
            holder.OpenSel.CallOnClick();
        }
        else
        {
            base.OnBackPressed();
        }
    }

    protected override void OnRestart()
    {
        base.OnRestart();
        holder.Update();
    }
}