using Android.Component;
using Android.Views;
using Bumptech.Glide;
using Object = Java.Lang.Object;

namespace Android.Adapter;

public class ImageAdapter(App.Activity activity, List<string> files)
    : SimplePagerAdapter<string>(files)
{
    public override Object InstantiateItem(ViewGroup container, int position)
    {
        var url = Models[position];
        ZoomImageView v = new ZoomImageView(activity);
        v.Click += (sender, e) => { OnItemClick?.Invoke(v, position); };
        v.ScaleBegin += (_, _) => { OnItemScaleBegin?.Invoke(this, v); };
        v.Id = position;
        container.AddView(v); //添加到父控件
        Glide.With(activity).Load(url).Into(v);
        return v;
    }
}