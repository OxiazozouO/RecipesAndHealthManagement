using AnyLibrary.Constants;
using Microsoft.AspNetCore.Mvc;
using WebServer.Controllers.Any;
using WebServer.Http;

namespace WebServer.Controllers.Service;

public static class UserService
{
    public static bool CheckUserExistence(this MyControllerBase myController, string userid,
        out DatabaseModel.User user,
        out IActionResult message)
    {
        user = myController.Db.Users.FirstOrDefault(u => u.UserId.ToString() == userid);
        message = ApiResponses.NoPermission; //用户不存在

        if (user is null)
            return true;

        return user.Status is UserStatus.Logout;
    }


    public static bool CheckAdminRole(this MyControllerBase myController, int adminId,
        out IActionResult message, out DatabaseModel.Admin admin)
    {
        admin = null;
        message = ApiResponses.NoPermission;
        var adminId2 = myController.Jwt.DecodeToUser(myController.Request).UserId;
        if (adminId.ToString() != adminId2)
            return true;
        admin = myController.Db.Admins.FirstOrDefault(a => a.Id == adminId);
        if (admin is null)
            return true;
        
        if(admin.Status != UserStatus.Usable)
            return true;

        var controllerName = myController.ControllerContext.RouteData.Values["controller"]?.ToString();
        var actionName = myController.ControllerContext.RouteData.Values["action"]?.ToString();
        var route = myController.Url.Action(actionName, controllerName);
        var roleId = admin.RoleId;

        bool hasPermission = myController.Db.RolePermissions
            .Any(rp => rp.RoleId == roleId &&
                       myController.Db.Permissions.Any(p =>
                           p.Id == rp.PermissionId && (("/admin/Admin" + p.Name) == route)));

        return !hasPermission;
    }

    public static bool CheckUserRole(this MyControllerBase myController, string userId,
        out IActionResult message, out DatabaseModel.User user)
    {
        user = null;

        var jwt = myController.Jwt.DecodeToUser(myController.Request);
        if (jwt.UserId != userId)
        {
            message = ApiResponses.NoPermission;
            return true;
        }

        if (myController.CheckUserExistence(userId, out user, out message))
        {
            return true;
        }

        message = null;
        return false;
    }
}