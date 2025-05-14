using WebServer.DatabaseModel;

namespace WebServer.Controllers.Service;

public static class FavoriteItemsService
{
    public static void GetLikeCount(RecipeAndHealthSystemContext db,
        long userid,
        long tid,
        sbyte idCategory,
        out int favoriteCount,
        out bool isLike)
    {
        var list = (from fi in db.FavoriteItems
            join f in db.Favorites on fi.FavoriteId equals f.FavoriteId
            where fi.TId == tid && f.IdCategory == idCategory
            group new { f.UserId, fi.TId } by new { f.UserId, fi.TId }
            into g
            select g.Key).ToList();

        favoriteCount = list.Count;
        isLike = list.Any(l => l.UserId == userid);
    }
}