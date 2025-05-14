using Android.Content;
using Android.Helper;
using Android.Holder;

namespace Android.Activity;

[Activity]
public class RegisterActivity : Android.App.Activity
{
    public ActivityRegisterHolder holder;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        holder = new ActivityRegisterHolder(this);
        SetContentView(holder.Root);
    }

    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        base.OnActivityResult(requestCode, resultCode, data);
        if (requestCode != 1 || resultCode != Result.Ok || data == null) return;
        var uri = data.Data;
        holder.Bind(uri);
    }

    public override void OnBackPressed()
    {
        ActivityHelper.GotoLogin();
    }
}