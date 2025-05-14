namespace AnyLibrary.Constants;

/// <summary>
/// 系统用户 方便上传资源
/// </summary>
public class UserRoles
{
    public const sbyte IngredientAdmin = 1;
    public const sbyte RecipeAdmin = 2;
    public const sbyte CollectionAdmin = 3;
    public const sbyte CategoryAdmin = 4;
}

public static class UserStatus
{
    public const sbyte Usable = 0;
    public const sbyte Logout = 1;
    public const sbyte Ban = 2;
}

public static class UserTypes
{
    public const sbyte User = 0;
    public const sbyte Admin = 1;
}

public static class CategoryType
{
    public const sbyte Category = 1;
    public const sbyte Emoji = 2;
}

public static class CommentStatus
{
    public const sbyte Usable = 0;
    public const sbyte ForceOff = 1;
}