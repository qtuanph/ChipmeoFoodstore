using FoodstoreApi.Usecase.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FoodstoreApi.Web.ApiResponse;

namespace FoodstoreApi.Web.Controllers;

/// <summary>
/// Controller for managing role-permission assignments
/// </summary>
[ApiController]
[Route("api/admin/roles/{roleId}/permissions")]
[Authorize(Roles = "Admin")]
public class RolePermissionController : ControllerBase
{
    private readonly IPermissionService _permissionService;

    public RolePermissionController(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    /// <summary>
    /// Get all permissions for a specific role
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetRolePermissions(int roleId, CancellationToken cancellationToken)
    {
        var permissionIds = await _permissionService.GetRolePermissionIdsAsync(roleId, cancellationToken);
        return ApiResult.Success(permissionIds);
    }

    /// <summary>
    /// Assign a single permission to a role
    /// </summary>
    [HttpPost("{permissionId}")]
    public async Task<IActionResult> AssignPermission(int roleId, int permissionId, CancellationToken cancellationToken)
    {
        await _permissionService.AssignPermissionToRoleAsync(roleId, permissionId, cancellationToken);
        return ApiResult.Success(new { message = "Permission assigned successfully" });
    }

    /// <summary>
    /// Remove a permission from a role
    /// </summary>
    [HttpDelete("{permissionId}")]
    public async Task<IActionResult> RemovePermission(int roleId, int permissionId, CancellationToken cancellationToken)
    {
        await _permissionService.RemovePermissionFromRoleAsync(roleId, permissionId, cancellationToken);
        return ApiResult.Success(new { message = "Permission removed successfully" });
    }

    /// <summary>
    /// Bulk update role permissions (replaces all permissions for the role)
    /// </summary>
    [HttpPut]
    public async Task<IActionResult> BulkUpdatePermissions(
        int roleId,
        [FromBody] List<int> permissionIds,
        CancellationToken cancellationToken)
    {
        try
        {
            await _permissionService.BulkUpdateRolePermissionsAsync(roleId, permissionIds, cancellationToken);
            return ApiResult.Success(new { message = "Permissions updated successfully", count = permissionIds.Count });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Failure(new ErrorDetail { Code = "INTERNAL_ERROR", Message = ex.Message }));
        }
    }
}

/// <summary>
/// Controller for managing all permissions (read-only for now)
/// </summary>
[ApiController]
[Route("api/admin/permissions")]
[Authorize]
public class PermissionController : ControllerBase
{
    private readonly IPermissionService _permissionService;

    public PermissionController(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    /// <summary>
    /// Get all permissions grouped by module
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllPermissions(CancellationToken cancellationToken)
    {
        var permissions = await _permissionService.GetAllPermissionsGroupedByModuleAsync(cancellationToken);
        return ApiResult.Success(permissions);
    }
}




