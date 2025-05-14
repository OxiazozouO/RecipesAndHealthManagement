using Android.JsonConverter;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace Android.Models;

public partial class EatingDiaryModel : ObservableObject
{
    [ObservableProperty] private long edId;

    [ObservableProperty] private sbyte flag;
    [ObservableProperty] private string fileUrl;
    [ObservableProperty] private string name;
    [ObservableProperty] private long tId;

    [ObservableProperty] private DateTime tieUpDate;

    [JsonConverter(typeof(StringToObjectJsonConverter))]
    public Dictionary<int, decimal> Dosages { get; set; }

    [JsonConverter(typeof(StringToObjectJsonConverter))]
    public Dictionary<string, decimal> Nutrients { get; set; }
}