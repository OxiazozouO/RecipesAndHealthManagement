using Android.Attribute;
using Android.ViewModel;
using Android.Views;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;

namespace Android.Holder;

[ViewClassBind(Layout.linear_recipe)]
public class LinearRecipeHolder(App.Activity activity) : ViewHolder<View>(activity)
{
    [ViewHolderArrayBind([
        Id.id_item_recipe_1,
        Id.id_item_recipe_2,
        Id.id_item_recipe_3,
        Id.id_item_recipe_4,
        Id.id_item_recipe_5,
        Id.id_item_recipe_6,
        Id.id_item_recipe_7,
        Id.id_item_recipe_8,
        Id.id_item_recipe_9,
        Id.id_item_recipe_10
    ])]
    public List<ItemRecipeSmallHolder> Holders;

    protected override void Init()
    {
    }

    public void Bind(List<RecipeInfoViewModel> models, Func<int, long, bool> action)
    {
        int num = models.Count;
        int pos = 0;
        if (num % 2 == 0)
        {
            Holders[0].Root.Visibility = ViewStates.Gone;
            pos = 1;
        }

        int i = 0;
        for (; i < num; i++)
            Holders[i + pos].Bind(models[i], action);

        for (i += pos; i < Holders.Count; i++)
            Holders[i].Root.Visibility = ViewStates.Gone;
    }
}