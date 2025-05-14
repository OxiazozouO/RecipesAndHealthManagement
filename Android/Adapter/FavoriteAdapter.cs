using Android.Holder;
using Android.ViewModel;
using Android.Views;

namespace Android.Adapter;

public class FavoriteAdapter(App.Activity activity, ListView root)
    : MultipleAdapter<FavoriteAtModel.FavoriteItemViewModel, ItemFavoriteInfoHolder>(root)
{
    public override void CreateItem(out ItemFavoriteInfoHolder holder, out View root)
    {
        holder = new ItemFavoriteInfoHolder(activity);
        root = holder.Root;
    }

    public override void Bind(int pos, FavoriteAtModel.FavoriteItemViewModel item, ItemFavoriteInfoHolder holder)
    {
        holder.Check.Visibility = IsMultiple
            ? ViewStates.Visible
            : ViewStates.Gone;

        holder.Check.Checked = _checkedMap.Get(pos);

        holder.Bind(item);
    }
}