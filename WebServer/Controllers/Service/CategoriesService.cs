using WebServer.DatabaseModel;

namespace WebServer.Controllers.Service;

public static class CategoriesService
{
    public static List<CategoriesDto> GetCategories(RecipeAndHealthSystemContext db, long userid, long tid,
        sbyte idCategory)
    {
        var categories = (from ci in db.CategoryItems
            join c in db.Categories on ci.CategoryId equals c.CategoryId
            where ci.IdCategory == idCategory && ci.TId == tid
            group new { c, ci } by c.CategoryId
            into g
            select new CategoriesDto
            {
                Id = g.Key,
                Name = g.First().c.CName,
                Count = g.Count(),
                TypeId = g.First().c.TypeId,
                IsLike = g.Any(f => f.ci.UserId == userid
                                                   && f.ci.IdCategory == idCategory
                                                   && f.ci.TId == tid
                                                   && f.ci.CategoryId == g.Key)
            }).ToList();
        return categories;
    }

    public static Action UpdateCategoryItems(this RecipeAndHealthSystemContext db,
        long tid, long userId, Dictionary<long, bool> items, sbyte idCategory)
    {
        var list = db.CategoryItems
            .Where(i => i.TId == tid && i.IdCategory == idCategory && i.UserId == userId)
            .ToList();

        var add = new List<CategoryItem>();
        var re = new List<CategoryItem>();

        foreach (var item in list)
        {
            if (items.TryGetValue(item.CategoryId, out var flag))
            {
                if (!flag) re.Add(item);
                items.Remove(item.CategoryId);
            }
            else re.Add(item);
        }

        foreach (var (key, value) in items)
        {
            if (!value) continue;
            add.Add(new CategoryItem
            {
                CategoryId = key,
                UserId = userId,
                TId = tid,
                IdCategory = idCategory
            });
        }

        return () =>
        {
            db.CategoryItems.RemoveRange(re);
            db.CategoryItems.AddRange(add);
        };
    }
}

public class CategoriesDto
{
    public long Id { set; get; }
    public string Name { set; get; }
    public int Count { set; get; }
    public int TypeId { set; get; }
    public bool IsLike { set; get; }
}