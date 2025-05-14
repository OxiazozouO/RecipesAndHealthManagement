using Android.Helper;
using Android.Holder;
using Android.HttpClients;
using Android.Models;
using Android.Views;
using AndroidX.ViewPager.Widget;
using static _Microsoft.Android.Resource.Designer.ResourceConstant;

namespace Android.Adapter;

public class DataPakedAdapter(App.Activity activity, View view)
    : SimplePagerAdapter<LinearEntDateHolder>([])
{
    public List<ItemEntDateHolder> ItemList;

    private ViewPager _pager;

    private static DateTime _mondayOfThisWeek;

    public void Init(ViewPager pager)
    {
        //两百年 100*365*2/7  73003天 10429页  第5215页 为今天所在的周
        Models = new List<LinearEntDateHolder>(10429);
        for (int i = 0; i < 10429; i++)
            Models.Add(null);

        ItemList = new(73004);
        for (int i = 0; i < 73004; i++)
            ItemList.Add(null);

        _pager = pager;
        pager.Adapter = this;
        pager.CurrentItem = 5215;

        _mondayOfThisWeek = DataTimeHelper.MondayOfThisWeek();
    }

    private bool _isInit = false;

    public LinearEntDateHolder CreateItem(int position)
    {
        var holder = new LinearEntDateHolder(activity);
        int p = position * 7;
        for (var i = 0; i < holder.Holders.Count; i++)
        {
            var item = holder.Holders[i];
            item.Root.Tag = item;
            int t = p + i;
            item.Root.CallClick(() => RootOnClick(t));
        }

        holder.AddRange(ItemList, p);

        return holder;
    }

    public void Goto(DateTime dateTime)
    {
        ToPos(dateTime, out var pos, out var page);
        _pager.CurrentItem = page;
        RootOnClick(pos);
        NotifyDataSetChanged();
    }

    public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
    {
        LinearEntDateHolder? holder = Models[position];
        if (holder is null)
        {
            holder = CreateItem(position);
            Models[position] = holder;

            BindView(position);
        }

        container.AddView(holder.Root);
        return holder.Root;
    }

    public Dictionary<DateTime, EatingDiaryViewModel> Diaries = new();


    public void BindView()
    {
        GetDateTime(SelectedPos, out int page, out DateTime date);
        BindView(page);
        Goto(date);
    }
    public void BindView(int page)
    {
        int pos = page * 7;
        GetDateTime(pos, out _, out var date);
        var diary = ApiService.GetEatingDiaries(date);

        Diaries[date] = diary;
        //初始化这七天
        for (int i = 0; i < 7; i++)
        {
            if (diary.EatingDiaryBar.TryGetValue(date.Date, out var energy))
            {
                ItemList[pos + i].Bind(date, diary.MaxEnergy, energy);
            }

            date = date.AddDays(1);
        }

        if (!_isInit)
        {
            Goto(DateTime.Now);
            _isInit = true;
        }
    }

    public DateTime GetDateTime(int pos, out int page, out DateTime date)
    {
        //5215为初始页  mondayOfThisWeek为初始页第一个  找到pos对应的第一个日期
        page = pos / 7;
        date = _mondayOfThisWeek.AddDays(pos - 5215 * 7);
        return date;
    }

    public static void ToPos(DateTime date, out int pos, out int page)
    {
        pos = (date.Date - _mondayOfThisWeek.Date).Days + 5215 * 7;
        page = pos / 7;
    }

    public int SelectedPos = 5215;
    

    private void RootOnClick(int pos)
    {
        if (ItemList[SelectedPos] != null)
            ItemList[SelectedPos].DateNum.SetBackgroundResource(Drawable.shape_round);
        SelectedPos = pos;
        if (ItemList[pos] != null)
            ItemList[pos].DateNum.SetBackgroundResource(Drawable.shape_round_pressed);
        OnItemClick?.Invoke(null, pos);
    }
}