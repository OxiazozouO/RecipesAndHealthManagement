using Android.Helper;
using Android.Holder;
using Android.HttpClients;
using Android.Views;

namespace Android.Adapter;

public class PageHomeAdapter(Android.App.Activity activity, List<HomeMenuItem> menuItems)
    : SimplePagerAdapter<HomeMenuItem>(menuItems)
{
    public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
    {
        var item = menuItems[position];
        View v = null;
        switch (item.Id)
        {
            case HomeMenuFlags.Ingredients:
                var holder = new PageHomeIngredientHolder(activity);
                holder.Bind((flag, count) => ApiService.GetIngredientInfoList(flag, count + 1),
                    (idCategory, id) =>
                    {
                        ActivityHelper.GotoIngredient(id);
                        return true;
                    }
                );
                v = holder.Root;
                break;
            case HomeMenuFlags.Recipe:
                var rholder = new PageHomeRecipeHolder(activity);
                rholder.Bind(count => ApiService.GetRecipeList(count + 1),
                    (idCategory, id) =>
                    {
                        ActivityHelper.GotoRecipe(id);
                        return true;
                    });
                v = rholder.Root;
                break;
            case HomeMenuFlags.Collection:
                var cholder = new PageHomeCollectionHolder(activity);
                cholder.Bind(count => ApiService.GetCollectionList(count + 1),
                    (idCategory, id) =>
                    {
                        ActivityHelper.GotoCollection(id);
                        return true;
                    });
                v = cholder.Root;
                break;
        }

        if (v is not null)
            container.AddView(v); //添加到父控件
        return v;
    }
}