namespace Android.Helper;

public static class DataTimeHelper
{
    public static int GetAge(this DateTime birthDate)
    {
        return (int)((DateTime.Now - birthDate).TotalDays / 365.2425);
    }

    public static DateTime MondayOfThisWeek()
    {
        var today = DateTime.Today;
        // 计算本周周一的日期
        int daysSinceMonday = ((int)today.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
        return today.AddDays(-daysSinceMonday);
    }

    public static DateTime ToDate(this int date)
    {
        return DateOnly.FromDayNumber(date).ToDateTime(TimeOnly.MinValue).Date;
    }
}