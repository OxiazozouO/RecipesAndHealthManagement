using Android.Holder;
using Android.HttpClients;
using Android.Models;
using Android.Views;

namespace Android.Adapter;

public class FavoriteListAdapter(App.Activity activity, ListView root, int idCategory)
    : MultipleAdapter<FavoriteModel, ItemFavoriteHolder>(root)
{
    public int IdCategory = idCategory;

    public void ReInitModels()
    {
        var models = ApiService.GetFavorites(IdCategory) ?? [];
        ReSet(models);
    }

    public override void CreateItem(out ItemFavoriteHolder holder, out View root)
    {
        holder = new ItemFavoriteHolder(activity);
        root = holder.Root;
    }

    public override void Bind(int pos, FavoriteModel item, ItemFavoriteHolder holder)
    {
        holder.Select.Visibility = IsMultiple
            ? ViewStates.Visible
            : ViewStates.Gone;

        holder.Select.Checked = _checkedMap.Get(pos);

        holder.Bind(item);
    }
}