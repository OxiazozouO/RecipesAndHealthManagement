using Android.Attribute;
using Android.Helper;
using Android.HttpClients;
using Android.Models;
using Android.Views;
using AnyLibrary.Helper;
using Bumptech.Glide;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;
using static Android.Text.InputTypes;
using Uri = Android.Net.Uri;

namespace Android.Holder;

[ViewClassBind(Layout.item_add_recipe_step)]
public class ItemAddRecipeStepHolder(App.Activity activity) : ViewHolder<View>(activity)
{
    [ViewBind(Id.id_add_step_name)] public TextView Name;

    [ViewBind(Id.id_add_step_close)] public ImageView Close;

    [ViewBind(Id.id_add_step_file_url)] public ImageView FileUrl;

    [ViewBind(Id.id_add_step_time)] public TextView Time;

    [ViewBind(Id.id_add_step_ingredient_add)]
    public ImageView IngredientAdd;

    [ViewBind(Id.id_add_step_ingredient_list)]
    public LinearLayout IngredientList;

    [ViewBind(Id.id_add_step_refer)] public TextView Refer;

    [ViewBind(Id.id_add_step_summary)] public TextView Summary;

    public StepModel step;

    protected override void Init()
    {
        Name.CallClick(() =>
        {
            MsgBoxHelper.Builder()
                .AddEditText(step.Title, ClassText, 30, "请输入名字")
                .Show(list =>
                {
                    step.Title = (string)list[0];
                    Update();
                });
        });

        Time.CallClick(() =>
        {
            MsgBoxHelper.Builder()
                .AddEditText(step.RequiredTime.ToString(), ClassText, 8, "请输入用时")
                .Show(list =>
                {
                    step.RequiredTime = TimeSpan.Parse((string)list[0]);
                    Update();
                });
        });

        Refer.CallClick(() =>
        {
            MsgBoxHelper.Builder()
                .AddEditText(step.Refer, ClassText, 200, "请输入描述")
                .Show(list =>
                {
                    step.Refer = (string)list[0];
                    Update();
                });
        });

        Summary.CallClick(() =>
        {
            MsgBoxHelper.Builder()
                .AddEditText(step.Summary, ClassText, 200, "请输入技巧")
                .Show(list =>
                {
                    step.Summary = (string)list[0];
                    Update();
                });
        });

        IngredientAdd.CallClick(() =>
        {
            var states = IngredientList.Visibility;
            if (states == ViewStates.Gone)
            {
                IngredientAdd.SetImageResource(Drawable.ic_close);
                IngredientList.Visibility = ViewStates.Visible;
                step.InitTimeRateModels();
                Update();
            }
            else
            {
                IngredientAdd.SetImageResource(Drawable.ic_add);
                IngredientList.Visibility = ViewStates.Gone;
            }
        });
    }

    private Action action;

    public void Bind(StepModel step, Action action)
    {
        this.step = step;
        this.action = action;
        if (step.TimeRateModels is not null && step.TimeRateModels.Count > 0)
        {
            IngredientAdd.CallOnClick();
        }

        Update();
    }

    public void Update()
    {
        action.Invoke();

        Name.Text = step.Title;
        Time.Text = step.RequiredTime?.Convert();
        Refer.Text = step.Refer;
        Summary.Text = step.Summary;
        if (!string.IsNullOrEmpty(step.FileUrl) && step.FileUrl.Contains("http"))
            Glide.With(Root).Load(step.FileUrl).Into(FileUrl);
        else if (step.Uri is not null)
            Glide.With(Root).Load(step.Uri).Into(FileUrl);

        if (step.TimeRateModels is null || step.TimeRateModels.Count < 1) return;
        if (IngredientList.ChildCount > 1)
            IngredientList.RemoveViews(1, IngredientList.ChildCount - 1);

        TimeRateModel.ConvertTime(step.TimeRateModels, step.RequiredTime);

        var holders = new List<ItemAddStepIngredientHolder>();
        for (var index = step.TimeRateModels.Count - 1; index >= 0; index--)
        {
            var holder = new ItemAddStepIngredientHolder(activity);
            holders.Add(holder);
            IngredientList.AddView(holder.Root);
        }

        var ac = () =>
        {
            foreach (var holder in holders) holder.Update();
        };

        for (var i = 0; i < step.TimeRateModels.Count; i++)
        {
            holders[i].Bind(step.TimeRateModels[i], ac);
        }
    }

    public void SetFileUrl(Uri? uri)
    {
        if (!activity.ContentResolver.FileUpload(uri, out string outFileName)) return;
        step.FileUrl = outFileName;
        step.Uri = uri;
        Glide.With(Root).Load(uri).Into(FileUrl);
    }
}