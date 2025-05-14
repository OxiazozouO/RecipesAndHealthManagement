namespace Android.Models;

[Flags]
public enum ModelFlags : sbyte
{
    Ingredient = 1,
    Recipe = 2,
    Collection = 3,
    Menu = 4
}

public static class ModelFlagHelper
{
    public static string GetName(byte flag) =>
        (ModelFlags)flag switch
        {
            ModelFlags.Ingredient => "食材",
            ModelFlags.Recipe => "菜谱",
            ModelFlags.Collection => "收藏夹",
            ModelFlags.Menu => "菜单",
            _ => ""
        };
}