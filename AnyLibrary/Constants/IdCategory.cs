namespace AnyLibrary.Constants;

public static class IdCategory
{
    public const sbyte Ingredient = 1;
    public const sbyte Recipe = 2;
    public const sbyte Collection = 3;
    public const sbyte Comment = 4;
    public const sbyte All = 100;

    public static string GetName(int id)
    {
        return id switch
        {
            1 => "食材",
            2 => "食谱",
            3 => "合集",
            4 => "评论",
            _ => ""
        };
    }
}