using Android.Holder;

namespace Android.Activity;

[Activity]
public class LogoutActivity : Android.App.Activity
{
    private LogoutHolder holder;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        holder = new LogoutHolder(this);
        SetContentView(holder.Root);
    }
}