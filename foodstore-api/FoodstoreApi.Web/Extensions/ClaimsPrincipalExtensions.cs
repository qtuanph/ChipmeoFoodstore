using System.Security.Claims;

namespace FoodstoreApi.Web.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(claim, out var id) ? id : Guid.Empty;
    }

    public static Guid GetEmployeeId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirst("EmployeeId")?.Value;
        return Guid.TryParse(claim, out var id) ? id : Guid.Empty;
    }

    public static List<string> GetPermissions(this ClaimsPrincipal user)
    {
        return user.FindAll("permission").Select(c => c.Value).ToList();
    }
}
