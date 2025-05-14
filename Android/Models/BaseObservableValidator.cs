using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace Android.Models;

public abstract class BaseObservableValidator : ObservableValidator
{
    [JsonIgnore]
    public string Error
    {
        get
        {
            ValidateAllProperties();
            return string.Join('\n', GetErrors());
        }
    }
}