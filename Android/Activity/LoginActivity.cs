using Android.Helper;
using Android.Holder;

namespace Android.Activity;

[Activity]
public class LoginActivity : Android.App.Activity
{
    private ActivityLoginHolder holder;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        AppConfigHelper.Del();
        holder = new ActivityLoginHolder(this);
        SetContentView(holder.Root);
    }
}