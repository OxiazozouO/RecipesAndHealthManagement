using WebServer.DatabaseModel;

namespace WebServer.Controllers.Service;

public static class NutrientContentService
{
    public static Dictionary<int, Dictionary<string, Dictionary<string, Dictionary<string, double>>>> get(
        RecipeAndHealthSystemContext db)
    {
        return (from nc in db.Dris
                join n in db.Nutrients on nc.NutrientId equals n.Id
                select new
                {
                    Gender = nc.Gender ? 0 : 1,
                    nc.Flag,
                    nc.AgeL,
                    nc.AgeR,
                    n.Name,
                    nc.Value
                }).ToList()
            .GroupBy(nc => nc.Gender)
            .ToDictionary(
                g => g.Key, // Gender
                g => g.GroupBy(nc => nc.Flag) // 先按 Flag 分组
                    .ToDictionary(
                        g2 => g2.Key, // Flag
                        g2 => g2.GroupBy(nc => new { nc.AgeL, nc.AgeR }) // 再按 AgeL 和 AgeR 分组
                            .ToDictionary(
                                g3 => $"{g3.Key.AgeL}-{g3.Key.AgeR}", // { AgeL, AgeR }
                                g3 => g3.ToDictionary(n => n.Name, n => n.Value)
                            )
                    )
            );
    }
}