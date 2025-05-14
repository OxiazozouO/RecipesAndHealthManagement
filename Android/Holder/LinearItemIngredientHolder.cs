using Android.Attribute;
using Android.Views;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;

namespace Android.Holder;

[ViewClassBind(Layout.linear_ingredient)]
public class LinearItemIngredientHolder(App.Activity activity) : ViewHolder<LinearLayout>(activity)
{
    [ViewHolderArrayBind([
        Id.id_item_ingredient_1,
        Id.id_item_ingredient_2,
        Id.id_item_ingredient_3,
        Id.id_item_ingredient_4,
        Id.id_item_ingredient_5,
        Id.id_item_ingredient_6,
        Id.id_item_ingredient_7,
        Id.id_item_ingredient_8,
        Id.id_item_ingredient_9,
        Id.id_item_ingredient_10
    ])]
    public List<ItemIngredientHolder> Holders;

    protected override void Init()
    {
    }

    public void Bind(List<IngredientInfo> models, Func<int, long, bool> action)
    {
        int num = models.Count;
        for (var i = 0; i < num; i++)
        {
            var model = models[i];
            Holders[i].Bind(activity, model, action);
        }

        if (num % 2 == 1)
        {
            Holders[num].Root.Visibility = ViewStates.Invisible;
            num++;
        }

        for (var i = num; i < Holders.Count; i++)
        {
            Holders[i].Root.Visibility = ViewStates.Gone;
        }
    }
}