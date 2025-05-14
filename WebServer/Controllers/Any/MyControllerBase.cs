using System.Transactions;
using Microsoft.AspNetCore.Mvc;
using WebServer.DatabaseModel;
using WebServer.Helper;
using WebServer.Http;

namespace WebServer.Controllers.Any;

public class MyControllerBase(RecipeAndHealthSystemContext db, JwtHelper jwt) : ControllerBase
{
    public readonly RecipeAndHealthSystemContext Db = db;
    public readonly JwtHelper Jwt = jwt;
}

public static class DbHelper
{
    public static IActionResult TransactionScope(this RecipeAndHealthSystemContext db, Action fun,
        string success, string error)
    {
        try
        {
            using var scope = new TransactionScope();
            fun();
            db.SaveChanges();
            scope.Complete();
            return ApiResponses.Success(success);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return ApiResponses.Error(error);
    }
}