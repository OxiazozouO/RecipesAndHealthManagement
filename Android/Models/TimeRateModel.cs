using CommunityToolkit.Mvvm.ComponentModel;

namespace Android.Models;

public partial class TimeRateModel : ObservableObject
{
    [ObservableProperty] private int display;
    [ObservableProperty] private IngredientModel ingredient;
    [ObservableProperty] private float rate;
    [ObservableProperty] private TimeSpan time;

    [ObservableProperty] private StepModel root;


    partial void OnDisplayChanged(int oldValue, int newValue)
    {
        DisplayAndRateChanged();
    }

    partial void OnRateChanged(float oldValue, float newValue)
    {
        DisplayAndRateChanged();
    }

    partial void OnRootChanged(StepModel? oldValue, StepModel newValue)
    {
        DisplayAndRateChanged();
    }

    private void DisplayAndRateChanged()
    {
        if (Rate < 0.1) return;
        if (Display == 0)
        {
            Time = TimeSpan.Zero;
        }

        ConvertTime(Root.TimeRateModels, Root.RequiredTime);
    }


    public static void ConvertTime(List<TimeRateModel> list, TimeSpan? time)
    {
        list = list.Where(x => x.Display != 0).ToList();
        var sum = list.Sum(x => x.Rate);
        if (sum < 0.1) return;
        foreach (var item in list)
            item.Time = TimeSpan.FromTicks((long)((decimal)item.Rate * time.Value.Ticks / (decimal)sum));
    }

    public static string ToString(List<TimeRateModel>? list)
    {
        if (list is null || list.Count == 0) return "||";
        var list1 = new List<(long, int)>();
        var list2 = new List<(long, int)>();
        var list3 = new List<int>();
        string s1 = "";
        string s2 = "";
        string s3 = "";

        foreach (var item in list)
        {
            switch (item.Display)
            {
                case 1:
                    list1.Add((item.Ingredient.IngredientId, (int)item.Rate));
                    break;
                case 2:
                    list2.Add((item.Ingredient.IngredientId, (int)item.Rate));
                    break;
            }
        }

        if (list1.Count > 0)
            list1.Sort((a, b) => a.Item2);
        if (list2.Count > 0)
            list2.Sort((a, b) => a.Item2);
        if (list1.Count > 0)
            list3 = list1.Select(l => l.Item2).ToList();
        if (list2.Count > 0)
            list3.AddRange(list2.Select(l => l.Item2));
        if (list1.Count > 0)
            s1 = string.Join(".", list1.Select(l => l.Item1).ToList());
        if (list2.Count > 0)
            s2 = string.Join(".", list2.Select(l => l.Item1).ToList());
        if (list3.Count > 0)
            s3 = string.Join(".", list3);

        return $"{s1}|{s2}|{s3}";
    }
}