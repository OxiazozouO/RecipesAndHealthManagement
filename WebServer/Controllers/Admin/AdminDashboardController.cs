using AnyLibrary.Constants;
using AnyLibrary.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebServer.Controllers.Any;
using WebServer.DatabaseModel;
using WebServer.DTOs;
using WebServer.Helper;
using WebServer.Http;

namespace WebServer.Controllers.Admin;

[Route("admin/[controller]/[action]")]
[ApiController]
public class AdminDashboardController(RecipeAndHealthSystemContext db, JwtHelper jwt) : MyControllerBase(db, jwt)
{
    private const string ConfigKey = "DashboardCache";

    [HttpPost]
    [Authorize(Roles = nameof(JwtType.Admin))]
    public IActionResult GetDashboardData(AdminDto dto)
    {
        try
        {
            var cacheConfig = Db.SystemConfigs.FirstOrDefault(c => c.ConfigName == ConfigKey);
            if (cacheConfig?.UpdateDate > DateTime.UtcNow.AddMicroseconds(10))
                return ApiResponses.Success("请求成功", cacheConfig.ConfigValue.ToEntity<ResultDto>());

            var recipesCount = Db.Recipes.Count(r => r.Status != Status.Deleted);
            var ingredientsCount = Db.Ingredients.Count(i => i.Status != Status.Deleted);
            var collectionsCount = Db.Collections.Count(c => c.Status != Status.Deleted);

            // 获取所有审核流程的最新状态
            var latestStatusQuery =
                from h in Db.ReleaseFlowHistories
                group h by h.ReleaseId
                into g
                select new
                {
                    ReleaseId = g.Key,
                    LatestStatus = g.OrderByDescending(x => x.CreateDate).First().Status
                };

            // 执行分组统计
            var statusGroups = latestStatusQuery
                .GroupBy(x => x.LatestStatus)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToList();

            var pending = statusGroups.FirstOrDefault(x => x.Status == Status.Pending)?.Count ?? 0;
            var approved = statusGroups.FirstOrDefault(x => x.Status == Status.Approve)?.Count ?? 0;
            var rejected = statusGroups.FirstOrDefault(x => x.Status == Status.Reject)?.Count ?? 0;
            var locked = statusGroups.FirstOrDefault(x => x.Status == Status.Locked)?.Count ?? 0;
            var totalProcessed = approved + rejected;
            var approvalRate = totalProcessed > 0 ? (int)Math.Round((double)approved / totalProcessed * 100) : 0;

            var report = (from r in Db.Reports
                group r by r.RType
                into g
                orderby g.Count() descending
                select new ReportDto
                {
                    Name = ReportTypes.GetName(g.Key),
                    Value = g.Count()
                }).Take(7).ToList();

            var thresholdDate = DateTime.Now.AddDays(-30).Date;

            //活跃用户数
            var usersActive = (from u in Db.Users where u.CreateDate >= thresholdDate select u.UserId)
                .Concat(
                    from fi in Db.FavoriteItems //是否有收藏
                    where fi.CreateDate >= thresholdDate
                    join f in Db.Favorites on fi.FavoriteId equals f.FavoriteId
                    select f.UserId)
                .Concat(
                    from c in Db.Comments //是否有评论
                    where c.CreateDate >= thresholdDate
                    select c.UserId
                )
                .Concat(
                    from d in Db.DietaryRecords //是否有饮食记录
                    where d.TieUpDate >= thresholdDate
                    select d.UserId
                )
                .Distinct()
                .Count();

            // 生成日期范围（最近7天）
            var dates = Enumerable.Range(0, 7)
                .Select(i => DateTime.Today.AddDays(-6 + i))
                .ToList();

            // 获取食谱趋势
            var recipesTrend = (from r in Db.Recipes
                    where r.CreateDate >= dates.First()
                    group r by r.CreateDate.Date
                    into g
                    select new { Date = g.Key, Recipes = g.Count() })
                .ToDictionary(g => g.Date, g => g.Recipes);

            // 获取食材趋势
            var ingredientsTrend = (from i in Db.Ingredients
                    where i.CreateDate >= dates.First()
                    group i by i.CreateDate.Date
                    into g
                    select new { Date = g.Key, Ingredients = g.Count() })
                .ToDictionary(g => g.Date, g => g.Ingredients);

            // 获取合集趋势
            var collectionsTrend = (from c in Db.Collections
                    where c.CreateDate >= dates.First()
                    group c by c.CreateDate.Date
                    into g
                    select new { Date = g.Key, Collections = g.Count() })
                .ToDictionary(g => g.Date, g => g.Collections);

            var tags = (from ci in Db.CategoryItems
                    join c in Db.Categories on ci.CategoryId equals c.CategoryId
                    group new { c.CName, ci.TId } by new { c.CategoryId, c.CName }
                    into g
                    orderby g.Count() descending
                    select new TagsDto { Name = g.Key.CName, Value = g.Count() }).Take(10) // 取前N个
                .ToList();

            var result = new ResultDto
            {
                Metrics = new MetricsDto
                {
                    RecipesCount = recipesCount,
                    IngredientsCount = ingredientsCount,
                    CollectionsCount = collectionsCount,
                    ApprovalRate = approvalRate,
                    UsersActive = usersActive,
                    Audit = new AuditDto
                    {
                        Pending = pending,
                        Approved = approved,
                        Rejected = rejected,
                        Locked = locked,
                    }
                },
                Charts = new ChartsDto
                {
                    Report = report,
                    ContentTrend = new ContentTrendDto
                    {
                        RecipesTrend = recipesTrend,
                        IngredientsTrend = ingredientsTrend,
                        CollectionsTrend = collectionsTrend
                    },
                    Tags = tags
                }
            };

            // 更新缓存（保持原有逻辑）
            if (cacheConfig == null)
            {
                Db.SystemConfigs.Add(new SystemConfig
                {
                    ConfigName = ConfigKey,
                    ConfigValue = result.ToJson(),
                    UpdateDate = DateTime.UtcNow
                });
            }
            else
            {
                cacheConfig.ConfigValue = result.ToJson();
                cacheConfig.UpdateDate = DateTime.Now;
                Db.SystemConfigs.Update(cacheConfig);
            }

            Db.SaveChanges();

            return ApiResponses.Success("请求成功", result);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        return ApiResponses.ErrorResult;
    }
}

public class ResultDto
{
    public MetricsDto Metrics { get; set; }
    public ChartsDto Charts { get; set; }
}

public class MetricsDto
{
    public int RecipesCount { get; set; }
    public int IngredientsCount { get; set; }
    public int CollectionsCount { get; set; }
    public double ApprovalRate { get; set; }
    public int UsersActive { get; set; }
    public AuditDto Audit { get; set; }
}

public class AuditDto
{
    public int Pending { get; set; }
    public int Approved { get; set; }
    public int Rejected { get; set; }
    public int Locked { get; set; }
}

public class ChartsDto
{
    public List<ReportDto> Report { get; set; }
    public ContentTrendDto ContentTrend { get; set; }
    public List<TagsDto> Tags { get; set; } // 根据实际情况替换具体类型
}

public class ContentTrendDto
{
    public Dictionary<DateTime, int> RecipesTrend { get; set; } // 根据实际情况调整元素类型
    public Dictionary<DateTime, int> IngredientsTrend { get; set; }
    public Dictionary<DateTime, int> CollectionsTrend { get; set; }
}

public class TagsDto
{
    public string Name { get; set; }
    public int Value { get; set; }
}

public class ReportDto
{
    public string Name { get; set; }
    public int Value { get; set; }
}