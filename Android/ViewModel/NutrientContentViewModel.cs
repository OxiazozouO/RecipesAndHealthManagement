namespace Android.ViewModel;

public class NutrientContentViewModel
{
    public int[] Colors;
    public int Icon;
    public string Str1;
    public string Str2;
    public decimal Max;
    public decimal Pos;
    public float Rate => Max < 0.001m ? 0 : (float)(Pos * 100 / Max);
}