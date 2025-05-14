const urls = {
    Login: {
        url: '/Login', method: 'POST', isJwt: false
    },
    GetUsers: {
        url: 'User/GetUsers', method: 'POST', isJwt: true
    },
    GetUser: {
        url: 'User/GetUser', method: 'POST', isJwt: true
    },
    Ban: {
        url: 'User/Ban', method: 'POST', isJwt: true
    },
    ChangeInfo: {
        url: 'User/ChangeInfo', method: 'POST', isJwt: true
    },
    ChangePassword: {
        url: 'User/ChangePassword', method: 'POST', isJwt: true
    },
    GetAdmins: {
        url: '/GetAdmins', method: 'POST', isJwt: true
    },
    GetRoles: {
        url: 'Role/GetRoles', method: 'POST', isJwt: true
    },
    GetPermissions: {
        url: 'Permission/GetPermissions', method: 'POST', isJwt: true
    },
    GetSimpleRoles: {
        url: 'Role/GetSimpleRoles', method: 'POST', isJwt: true
    },
    ChangeAdminInfo: {
        url: '/ChangeAdminInfo', method: 'POST', isJwt: true
    },
    AddAdmin: {
        url: '/AddAdmin', method: 'POST', isJwt: true
    },
    AdminLogout: {
        url: '/Logout', method: 'POST', isJwt: true
    },
    GetRolePermissions: {
        url: 'Permission/GetRolePermissions', method: 'POST', isJwt: true
    },
    AddPermission: {
        url: 'Permission/AddPermission', method: 'POST', isJwt: true
    },
    EditPermission: {
        url: 'Permission/EditPermission', method: 'POST', isJwt: true
    },
    DeletePermission: {
        url: 'Permission/DeletePermission', method: 'POST', isJwt: true
    },
    EditRolePermissions: {
        url: 'Role/EditRolePermissions', method: 'POST', isJwt: true
    },
    AddRole: {
        url: 'Role/AddRole', method: 'POST', isJwt: true
    },
    EditRole: {
        url: 'Role/EditRole', method: 'POST', isJwt: true
    },
    GetIngredients: {
        url: 'Ingredient/GetIngredients', method: 'POST', isJwt: true
    },
    GetIngredient: {
        url: 'Ingredient/GetIngredient', method: 'POST', isJwt: true
    },
    AddIngredient: {
        url: 'Ingredient/AddIngredient', method: 'POST', isJwt: true
    },
    EditIngredient: {
        url: 'Ingredient/EditIngredient', method: 'POST', isJwt: true
    },
    GetNutrients: {
        url: 'Ingredient/GetNutrients', method: 'POST', isJwt: true
    },
    ReverseIngredientStatus: {
        url: 'Ingredient/ReverseIngredientStatus', method: 'POST', isJwt: true
    },
    GetRecipes: {
        url: 'Recipe/GetRecipes', method: 'POST', isJwt: true
    },
    GetRecipe: {
        url: 'Recipe/GetRecipe', method: 'POST', isJwt: true
    },
    GetRecipeIngredients: {
        url: 'Recipe/GetRecipeIngredients', method: 'POST', isJwt: true
    },
    EditRecipe: {
        url: 'Recipe/EditRecipe', method: 'POST', isJwt: true
    },
    AddRecipe: {
        url: 'Recipe/AddRecipe', method: 'POST', isJwt: true
    },
    GetCategories: {
        url: 'Category/GetCategories', method: 'POST', isJwt: true
    },
    AddCategory: {
        url: 'Category/AddCategory', method: 'POST', isJwt: true
    },
    RemoveCategory: {
        url: 'Category/RemoveCategory', method: 'POST', isJwt: true
    },
    GetCategoryIngredients: {
        url: 'Category/GetCategoryIngredients', method: 'POST', isJwt: true
    },
    GetIngredientCategoryItems: {
        url: 'Category/GetIngredientCategoryItems', method: 'POST', isJwt: true
    },
    GetRecipeCategoryItems: {
        url: 'Category/GetRecipeCategoryItems', method: 'POST', isJwt: true
    },
    GetCategoryRecipes: {
        url: 'Category/GetCategoryRecipes', method: 'POST', isJwt: true
    },
    EditCategoryName: {
        url: 'Category/EditCategoryName', method: 'POST', isJwt: true
    },
    UpdateIngredientCategoryItems: {
        url: 'Category/UpdateIngredientCategoryItems', method: 'POST', isJwt: true
    },
    UpdateRecipeCategoryItems: {
        url: 'Category/UpdateRecipeCategoryItems', method: 'POST', isJwt: true
    },
    GetReleases: {
        url: 'Release/GetReleases', method: 'POST', isJwt: true
    },
    NeedEditRelease: {
        url: 'Release/NeedEditRelease', method: 'POST', isJwt: true
    },
    RemoveRelease: {
        url: 'Release/RemoveRelease', method: 'POST', isJwt: true
    },
    GetRelease: {
        url: 'Release/GetRelease', method: 'POST', isJwt: true
    },
    ConfirmRelease: {
        url: 'Release/ConfirmRelease', method: 'POST', isJwt: true
    },
    ApproveRelease: {
        url: 'Release/ApproveRelease', method: 'POST', isJwt: true
    },
    RejectRelease: {
        url: 'Release/RejectRelease', method: 'POST', isJwt: true
    },
    ReverseRecipeStatus: {
        url: 'Recipe/ReverseRecipeStatus', method: 'POST', isJwt: true
    },
    GetReleaseRecipe: {
        url: 'Release/GetReleaseRecipe', method: 'POST', isJwt: true
    },
    GetReleaseIngredient: {
        url: 'Release/GetReleaseIngredient', method: 'POST', isJwt: true
    },
    DeleteIngredient: {
        url: 'Ingredient/DeleteIngredient', method: 'POST', isJwt: true
    },
    DeleteRecipe: {
        url: 'Recipe/DeleteRecipe', method: 'POST', isJwt: true
    },
    GetComments: {
        url: 'Comment/GetComments', method: 'POST', isJwt: true
    },
    DeleteComment: {
        url: 'Comment/DeleteComment', method: 'POST', isJwt: true
    },
    ReverseCommentStatus: {
        url: 'Comment/ReverseCommentStatus', method: 'POST', isJwt: true
    },
    GetDashboardData: {
        url: 'Dashboard/GetDashboardData', method: 'POST', isJwt: true
    },
    GetReports: {
        url: 'Report/GetReports', method: 'POST', isJwt: true
    },
    ReportOff: {
        url: 'Report/ReportOff', method: 'POST', isJwt: true
    },
    RejectReport: {
        url: 'Report/RejectReport', method: 'POST', isJwt: true
    },
    DeleteReject: {
        url: 'Report/DeleteReject', method: 'POST', isJwt: true
    },
    GetConfigs: {
        url: 'Config/GetConfigs', method: 'POST', isJwt: true
    },
    EditConfig: {
        url: 'Config/EditConfig', method: 'POST', isJwt: true
    },
    ApplyChanges: {
        url: 'Config/ApplyChanges', method: 'POST', isJwt: true
    },
    Backup: {
        url: 'Config/Backup', method: 'POST', isJwt: true
    },
    Restore: {
        url: 'Config/Restore', method: 'POST', isJwt: true
    },
    DeleteUselessFiles: {
        url: 'Config/DeleteUselessFiles', method: 'POST', isJwt: true
    },
    GetSimpleRolePermissions: {
        url: 'Permission/GetSimpleRolePermissions', method: 'POST', isJwt: true
    },
    Reset: {
        url: 'Config/Reset', method: 'POST', isJwt: true
    },
    GetBackupInfo: {
        url: 'Config/GetBackupInfo', method: 'POST', isJwt: true
    },
    DeleteSqlFile: {
        url: 'Config/DeleteSqlFile', method: 'POST', isJwt: true
    },
    GetCollections: {
        url: 'Collection/GetCollections', method: 'POST', isJwt: true
    },
    GetCollection: {
        url: 'Collection/GetCollection', method: 'POST', isJwt: true
    },
    AddCollection: {
        url: 'Collection/AddCollection', method: 'POST', isJwt: true
    },
    EditCollection: {
        url: 'Collection/EditCollection', method: 'POST', isJwt: true
    },
    DeleteCollection: {
        url: 'Collection/DeleteCollection', method: 'POST', isJwt: true
    },
    ReverseCollectionStatus: {
        url: 'Collection/ReverseCollectionStatus', method: 'POST', isJwt: true
    },
    GetTabs: {
        url: 'Collection/GetTabs', method: 'POST', isJwt: true
    },
    GetReleaseCollection: {
        url: 'Release/GetReleaseCollection', method: 'POST', isJwt: true
    },
    GetCategoryCollections: {
        url: 'Category/GetCategoryCollections', method: 'POST', isJwt: true
    },
    GetCollectionCategoryItems: {
        url: 'Category/GetCollectionCategoryItems', method: 'POST', isJwt: true
    },
    UpdateCollectionCategoryItems: {
        url: 'Category/UpdateCollectionCategoryItems', method: 'POST', isJwt: true
    },
}

const httpTool = {}

httpTool.execute = async function (info, json) {
    if (info === null) {
        alert('接口不存在')
        return
    }
    const headers = {
        'accept': '*/*',
        'Content-Type': 'application/json'
    }


    if (info.isJwt) {
        const jwt = sessionStorage.getItem('jwt');
        if (!jwt) {
            alert('请先登录');
            window.location.href = '../login.html'
            return undefined
        }
        headers['Authorization'] = jwt;
    }

    try {
        const dj = JSON.stringify(json)
        const response = await fetch(`http://localhost:5256/admin/Admin` + info.url, {
            method: info.method,
            headers: headers,
            body: dj
        });

        if (!response.ok) {
            switch (response.status) {
                case 400:
                    alert('客户端请求错误')
                    break
                case 401:
                    sessionStorage.setItem('jwt', null)
                    alert('登录已过期')
                    window.location.href = '../login.html'
                    break
                case 403:
                    alert('未授权的接口')
                    break
                case 404:
                    alert('服务器不支持该请求')
                    break
                case 500:
                    alert('服务器内部错误')
                    break
                case 501:
                    alert('服务器不支持该请求')
                    break
                case 502:
                    alert('网关错误')
                    break
                default:
                    alert(`发生错误：${response.statusText}`)
                    break
            }
            return undefined
        }

        const data = await response.json();

        switch (data.code) {
            case 403:
                alert('无此操作的权限' + data.message)
                break
            case 401:
                alert('请重新登录')
                window.location.href = '../login.html'
                break
            case 400:
                alert(data.message)
                break
            case 0:
                alert(data.message)
                break
            case 1:
                return data;
        }
    } catch (error) {
        alert(`发生错误：${error.message}`);
    }
    return undefined
}

httpTool.fileUpload = async file => {
    if (file.size > 1024 * 1024 * 10) {
        alert("文件大小超过10MB");
        return undefined
    }

    const formData = new FormData()
    formData.append("file", file)

    try {
        const response = await fetch("http://localhost:5256/api/File/Upload", {
            method: "POST",
            body: formData
        });

        if (!response.ok) {
            alert(`上传失败：${response.statusText}`)
            return undefined
        }

        const res = await response.json()

        if (!res.data.newFileName) {
            alert("上传失败")
            return undefined
        }
        if (res.code === 1) {
            return {
                outFileName: res.data.newFileName,
                url: res.data.url
            }
        }
    } catch (error) {
        alert(`发生错误：${error.message}`)
    }
    return undefined
}