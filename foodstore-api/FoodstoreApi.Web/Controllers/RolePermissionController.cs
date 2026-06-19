using FoodstoreApi.Usecase.Interfaces;
using FoodstoreApi.Web.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FoodstoreApi.Web.ApiResponse;

namespace FoodstoreApi.Web.Controllers;

[ApiController]
[Route("api/admin/roles/{roleId}/permissions")]
[Authorize]
public class RolePermissionController(IPermissionService permissionService) : ControllerBase
{
    private readonly IPermissionService _permissionService = permissionService;

    [HttpGet]
    [RequirePermission("role.view")]
    public async Task<IActionResult> GetRolePermissions(Guid roleId, CancellationToken cancellationToken)
    {
        var permissionCodes = await _permissionService.GetRolePermissionCodesAsync(roleId, cancellationToken);
        return ApiResult.Success(permissionCodes);
    }

    [HttpPost("{permissionCode}")]
    [RequirePermission("role.update")]
    public async Task<IActionResult> AssignPermission(Guid roleId, string permissionCode, CancellationToken cancellationToken)
    {
        await _permissionService.AssignPermissionToRoleAsync(roleId, permissionCode, cancellationToken);
        return ApiResult.Success(new { message = "Permission assigned successfully" });
    }

    [HttpDelete("{permissionCode}")]
    [RequirePermission("role.update")]
    public async Task<IActionResult> RemovePermission(Guid roleId, string permissionCode, CancellationToken cancellationToken)
    {
        await _permissionService.RemovePermissionFromRoleAsync(roleId, permissionCode, cancellationToken);
        return ApiResult.Success(new { message = "Permission removed successfully" });
    }

    [HttpPut]
    [RequirePermission("role.update")]
    public async Task<IActionResult> BulkUpdatePermissions(
        Guid roleId,
        [FromBody] List<string> permissionCodes,
        CancellationToken cancellationToken)
    {
        try
        {
            await _permissionService.BulkUpdateRolePermissionsAsync(roleId, permissionCodes, cancellationToken);
            return ApiResult.Success(new { message = "Permissions updated successfully", count = permissionCodes.Count });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Failure(new ErrorDetail { Code = "INTERNAL_ERROR", Message = ex.Message }));
        }
    }
}

[ApiController]
[Route("api/admin/permissions")]
[Authorize]
public class PermissionController(IPermissionService permissionService) : ControllerBase
{
    private readonly IPermissionService _permissionService = permissionService;

    [HttpGet]
    public async Task<IActionResult> GetAllPermissions(CancellationToken cancellationToken)
    {
        var permissions = await _permissionService.GetAllPermissionsGroupedByModuleAsync(cancellationToken);
        return ApiResult.Success(permissions);
    }
}
