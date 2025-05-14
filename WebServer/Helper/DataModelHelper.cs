using AnyLibrary.Constants;
using AnyLibrary.Helper;
using Microsoft.EntityFrameworkCore;
using WebServer.DatabaseModel;
using WebServer.DTOs;
using Collection = WebServer.DatabaseModel.Collection;

namespace WebServer.Helper;

public static class DataModelHelper
{
    public static Ingredient MapTo(this AddIngredientModel model, Func<Ingredient> fun)
    {
        var i = fun();
        i.IName = model.IName;
        i.FileUrl = model.FileUrl;
        i.Refer = model.Refer;
        i.Unit = model.Unit;
        i.Quantity = model.Quantity?.ToJson() ?? "{}";
        i.Allergy = model.Allergy;
        i.Content = model.Content;
        return i;
    }

    public static Recipe MapTo(this AddRecipeModel model, Func<Recipe> fun)
    {
        var recipe = fun();
        recipe.Title = model.Title;
        recipe.RName = model.RName;
        recipe.FileUrl = model.FileUrl;
        recipe.Summary = model.Summary;
        return recipe;
    }

    public static RecipeItem MapTo(this AddRecipeIngredientModel model, Func<RecipeItem> fun)
    {
        var ri = fun();
        ri.IngredientId = model.IngredientId;
        ri.Dosage = model.Dosage;
        return ri;
    }

    public static PreparationStep MapTo(this AddRecipeStepModel model, Func<PreparationStep> fun)
    {
        var step = fun();
        step.Title = model.Title;
        step.FileUrl = model.FileUrl;
        step.Refer = model.Refer;
        step.RequiredTime = model.RequiredTime;
        step.RequiredIngredient = model.RequiredIngredient ?? "||";
        step.Summary = model.Summary;
        return step;
    }

    public static Collection MapTo(this AddCollectionModel model, Func<Collection> fun)
    {
        var collection = fun();
        collection.FileUrl = model.FileUrl;
        collection.Title = model.Title;
        collection.Summary = model.Summary;
        collection.Content = model.Content.ToJson();
        return collection;
    }

    public static Release MapTo(this AddIngredientModel model, Func<Release> fun)
    {
        var i = fun();
        i.Content = model.ToJson();
        i.Title = model.IName;
        i.FileUrl = model.FileUrl;
        return i;
    }

    public static Release MapTo(this AddRecipeModel model, Func<Release> fun)
    {
        var i = fun();
        i.Content = model.ToJson();
        i.Title = model.Title;
        i.FileUrl = model.FileUrl;
        return i;
    }

    public static Release MapTo(this AddCollectionModel model, Func<Release> fun)
    {
        var i = fun();
        i.Content = model.ToJson();
        i.Title = model.Title;
        i.FileUrl = model.FileUrl;
        return i;
    }

    public static List<long> GetCategoryIds(this DbSet<Category> categories, List<string>? names)
    {
        if (names is null) return [];
        return (from c in categories
            where names.Contains(c.CName) && c.TypeId == CategoryType.Category
            select c.CategoryId).ToList();
    }
}

public class DataUpdate<T, TV> where T : class
{
    public DbSet<T> Tab;
    public Func<T, bool> Filter;
    public Func<T, TV> Maper;


    private readonly List<T> _add = [];
    private readonly List<T> _up = [];
    private List<T> _del;
    private Dictionary<TV, T> _dir;
    private List<Action> ids = [];

    public DataUpdate<T, TV> Build()
    {
        _del = Tab.Where(Filter).ToList();
        _dir = _del.ToDictionary(Maper, n => n);
        return this;
    }

    public void Append(T i) => _add.Add(i);


    public void Append(Action i) => ids.Add(i);

    public bool IsUpdate(TV key, out T item)
    {
        var result = _dir.TryGetValue(key, out item);
        if (!result) return result;

        _del.Remove(item);
        _up.Add(item);

        return result;
    }

    public void RunSql()
    {
        Tab.UpdateRange(_up);
        Tab.RemoveRange(_del);
        Tab.AddRange(_add);
    }

    public void UpdateId()
    {
        ids.ForEach(i => i());
    }
}