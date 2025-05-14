using Android.Holder;

namespace Android.Activity;

[Activity]
public class SearchActivity : Android.App.Activity
{
    private SearchPageHolder _searchPageHolder;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        var bundle = Intent.Extras;
        SearchFlag flag = (SearchFlag)bundle.GetInt("flag");
        _searchPageHolder = new SearchPageHolder(this, flag);

        SetContentView(_searchPageHolder.Root);
    }
}

[Flags]
public enum SearchFlag
{
    All = 1,
    Open = 2,
    Ingredient = 1024,
    Recipe = 2048,
    Colletion = 4096
}