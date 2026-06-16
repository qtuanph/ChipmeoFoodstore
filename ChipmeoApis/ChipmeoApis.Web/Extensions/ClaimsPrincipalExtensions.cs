using System.Security.Claims;

namespace ChipmeoApis.Web.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(claim, out var id) ? id : 0;
    }

    public static List<string> GetPermissions(this ClaimsPrincipal user)
    {
        return user.FindAll("permission").Select(c => c.Value).ToList();
    }
}
