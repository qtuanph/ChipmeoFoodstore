using Microsoft.AspNetCore.Authorization;

namespace ChipmeoApis.Web.Authorization;

public class RequirePermissionAttribute : AuthorizeAttribute
{
    public RequirePermissionAttribute(string permission)
    {
        Policy = $"Permission:{permission}";
    }
}




