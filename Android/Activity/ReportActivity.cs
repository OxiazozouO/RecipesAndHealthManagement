using Android.Holder;

namespace Android.Activity;

[Activity]
public class ReportActivity : App.Activity
{
    private ActivityReportHolder holder;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        try
        {
            var bundle = Intent.Extras;
            long tid = bundle.GetLong("tid");
            int category = bundle.GetInt("category");
            string name = bundle.GetString("name");
            holder = new ActivityReportHolder(this);
            holder.Bind(tid, category, name);
            SetContentView(holder.Root);
            return;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        Finish();
    }
}