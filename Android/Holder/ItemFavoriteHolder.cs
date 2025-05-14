using Android.Attribute;
using Android.Models;
using Android.Views;
using AnyLibrary.Constants;
using AnyLibrary.Helper;
using Bumptech.Glide;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;

namespace Android.Holder;

[ViewClassBind(Layout.item_favorite)]
public class ItemFavoriteHolder(App.Activity activity) : ViewHolder<View>(activity)
{
    [ViewBind(Id.id_favorite_file_url)] public ImageView FileUrl;

    [ViewBind(Id.id_favorite_title)] public TextView Title;

    [ViewBind(Id.id_favorite_count)] public TextView Count;

    [ViewBind(Id.id_favorite_mod_time)] public TextView ModTime;

    [ViewBind(Id.id_favorite_select)] public CheckBox Select;

    [ViewBind(Id.id_favorite_flag)] public TextView Flag;

    protected override void Init()
    {
    }

    public void Bind(FavoriteModel model)
    {
        Glide.With(Root).Load(model.FileUrl).Into(FileUrl);
        Title.Text = model.FName;
        Count.Text = model.ItemsCount.ToString();
        ModTime.Text = model.ModifyDate.TimeStr1();
        Flag.Text = IdCategory.GetName(model.Flag);
        
        int d = model.Flag switch
        {
            IdCategory.Ingredient => Drawable.shape_label5_bg,
            IdCategory.Recipe => Drawable.shape_label10_bg,
            IdCategory.Collection => Drawable.shape_label8_bg,
            _ => Drawable.shape_label9_bg
        };

        Flag.SetBackgroundResource(d);
    }
}