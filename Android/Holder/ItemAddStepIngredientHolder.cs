using Android.Attribute;
using Android.Helper;
using Android.Models;
using Android.Views;
using AnyLibrary.Helper;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;
using static Android.Text.InputTypes;

namespace Android.Holder;

[ViewClassBind(Layout.item_add_step_ingredient)]
public class ItemAddStepIngredientHolder(App.Activity activity) : ViewHolder<View>(activity)
{
    [ViewBind(Id.id_add_step_ingredient_1)]
    public View C1;

    [ViewBind(Id.id_add_step_ingredient_2)]
    public View C2;

    [ViewBind(Id.id_add_step_ingredient_3)]
    public TextView C3;

    [ViewBind(Id.id_add_step_ingredient_4)]
    public TextView C4;

    [ViewBind(Id.id_add_step_ingredient_5)]
    public TextView C5;

    protected override void Init()
    {
        C1.CallClick(() =>
        {
            model.Display = model.Display == 1 ? 0 : 1;
            action.Invoke();
        });
        C2.CallClick(() =>
        {
            model.Display = model.Display == 2 ? 0 : 2;
            action.Invoke();
        });

        C4.CallClick(() =>
        {
            MsgBoxHelper.Builder()
                .AddEditText(model.Rate.ToString(), ClassNumber | NumberFlagDecimal, 200, "请输入技巧")
                .Show(list =>
                {
                    model.Rate = float.Parse((string)list[0]);
                    action.Invoke();
                });
        });
    }

    private TimeRateModel model;
    private Action action;


    public void Bind(TimeRateModel model, Action action)
    {
        this.model = model;
        this.action = action;
        Update();
    }

    public void Update()
    {
        switch (model.Display)
        {
            case 1:
                C1.SetBackgroundResource(Drawable.shape_button_bg);
                C2.SetBackgroundResource(Drawable.shape_button_bg5);
                break;
            case 2:
                C1.SetBackgroundResource(Drawable.shape_button_bg5);
                C2.SetBackgroundResource(Drawable.shape_button_bg);
                break;
            default:
                C1.SetBackgroundResource(Drawable.shape_button_bg5);
                C2.SetBackgroundResource(Drawable.shape_button_bg5);
                break;
        }

        C3.Text = model.Ingredient.IName;
        C4.Text = $"{model.Rate:f2}";
        C5.Text = model.Time.Convert();
    }
}