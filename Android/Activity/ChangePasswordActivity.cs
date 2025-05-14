using Android.Holder;

namespace Android.Activity;

[Activity]
public class ChangePasswordActivity : App.Activity
{
    private ActivityChangePasswordHolder holder;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        holder = new ActivityChangePasswordHolder(this);
        SetContentView(holder.Root);
    }
}