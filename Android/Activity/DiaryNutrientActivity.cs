using Android.Holder;
using Android.Models;
using AnyLibrary.Helper;

namespace Android.Activity;

[Activity]
public class DiaryNutrientActivity : App.Activity
{
    private ActivityDiaryNutrientHolder holder;
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        if (Intent?.Extras is { } b)
        {
            string json = b.GetString("json");
            holder = new ActivityDiaryNutrientHolder(this);
            SetContentView(holder.Root);
            var model = json.ToEntity<NutrientContentRatioModel>();
            holder.Bind(model);
        }
        else
        {
            Finish();
        }
    }
}