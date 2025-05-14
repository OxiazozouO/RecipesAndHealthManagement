using Android.Holder;
using Android.Models;
using Android.Views;
using Object = Java.Lang.Object;

namespace Android.Adapter;

public class PageReleaseListAdapter(App.Activity activity) : SimpleAdapter<ReleaseModel>(activity)
{
    public Dictionary<long, int> Status = [];

    public override bool CreateItem(App.Activity activity, int pos, out Object holder, out View root)
    {
        var itemHolder = new PageReleaseListItemHolder(activity);
        holder = itemHolder;
        root = itemHolder.Root;
        return true;
    }

    public override void Bind(ReleaseModel item, Object holder)
    {
        if (holder is PageReleaseListItemHolder iHolder)
            iHolder.Bind(item, Status.GetValueOrDefault(item.TId, -1));
    }
}