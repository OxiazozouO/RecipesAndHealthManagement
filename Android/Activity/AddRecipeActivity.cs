using Android.Content;
using Android.Helper;
using Android.Holder;
using Android.HttpClients;
using Android.ViewModel;
using AnyLibrary.Constants;
using AnyLibrary.Helper;

namespace Android.Activity;

[Activity]
public class AddRecipeActivity : App.Activity
{
    private ActivityAddRecipeHolder holder;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        try
        {
            holder = new ActivityAddRecipeHolder(this);
            SetContentView(holder.Root);
            if (Intent?.Extras is not { } b) return;
            long releaseId = b.GetLong("releaseId", -1);
            long tid = b.GetLong("tid", 0);
            holder.Tid = tid;
            holder.ReleaseId = releaseId;

            var req = ApiEndpoints.GetReleaseRecipe(new
            {
                Id = AppConfigHelper.AppConfig.Id,
                ReleaseId = releaseId
            });
            if (req.Execute(out var res))
            {
                var recipe = res.Data.ToEntity<RecipeViewModel>();
                holder.model = recipe;
                holder.Put();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        base.OnActivityResult(requestCode, resultCode, data);
        if (resultCode != Result.Ok || data == null) return;
        switch (requestCode)
        {
            case 0:
                try
                {
                    int idCategory = data.GetIntExtra("idCategory", 0);
                    long id = data.GetLongExtra("id", 0);
                    if (idCategory == IdCategory.Ingredient)
                    {
                        holder.Bind(id);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                break;
            case 1:
                holder.SetFileUrl(data.Data);
                break;
            default:
                holder.StepHolders[requestCode - 2].SetFileUrl(data.Data);
                break;
        }
    }
}