using Android.Helper;
using Android.Runtime;

namespace Android;

[Application(UsesCleartextTraffic = true)]
public class WeatherApplication(IntPtr javaReference, JniHandleOwnership transfer)
    : Application(javaReference, transfer), Application.IActivityLifecycleCallbacks
{
    private static WeatherApplication _instance;

    public static WeatherApplication Instance => _instance;

    public override void OnCreate()
    {
        base.OnCreate();
        _instance = this;

        RegisterActivityLifecycleCallbacks(this);
    }

    public void OnActivityCreated(App.Activity activity, Bundle? savedInstanceState)
    {
    }

    public void OnActivityDestroyed(App.Activity activity)
    {
    }

    public void OnActivityPaused(App.Activity activity)
    {
    }

    public void OnActivityResumed(App.Activity activity)
    {
        ActivityHelper.CurrentActivity = activity;
    }

    public void OnActivitySaveInstanceState(App.Activity activity, Bundle outState)
    {
    }

    public void OnActivityStarted(App.Activity activity)
    {
    }

    public void OnActivityStopped(App.Activity activity)
    {
    }
}