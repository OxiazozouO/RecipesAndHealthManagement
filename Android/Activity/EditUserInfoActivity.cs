using Android.Content;
using Android.Holder;

namespace Android.Activity;

[Activity]
public class EditUserInfoActivity : App.Activity
{
    public ActivityEditUserInfoHolder holder;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        holder = new ActivityEditUserInfoHolder(this);
        SetContentView(holder.Root);
    }

    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        base.OnActivityResult(requestCode, resultCode, data);
        if (requestCode != 1 || resultCode != Result.Ok || data == null) return;
        holder.SetFileUrl(data.Data);
    }
}