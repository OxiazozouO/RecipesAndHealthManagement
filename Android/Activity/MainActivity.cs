using Android.Helper;
using Android.Holder;
using static Android.Helper.AppConfigHelper;

namespace Android.Activity;

[Activity(MainLauncher = true)]
public class MainActivity : BaseActivity
{
    private MainPageHolder holder;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        
        if (string.IsNullOrEmpty(AppConfig?.Jwt))
        {
            ActivityHelper.GotoLogin();
            Finish();
        }

        holder = new MainPageHolder(this);
        SetContentView(holder.Root);
    }
}