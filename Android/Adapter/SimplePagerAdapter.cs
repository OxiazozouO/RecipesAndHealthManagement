using Android.Views;
using AndroidX.ViewPager.Widget;

namespace Android.Adapter;

public abstract class SimplePagerAdapter<T>(List<T> models) : PagerAdapter
{
    public List<T> Models { get; set; } = models;
    public override int Count => Models.Count;

    public override bool IsViewFromObject(View view, Java.Lang.Object obj)
    {
        return view == obj;
    }

    public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object obj)
    {
        container.RemoveView((View)obj);
    }

    public EventHandler<int>? OnItemClick;
    public EventHandler<View>? OnItemScaleBegin;
}