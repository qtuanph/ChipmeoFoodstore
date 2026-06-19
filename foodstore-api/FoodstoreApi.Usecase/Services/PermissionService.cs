using System.Security.Claims;
using FoodstoreApi.Core.Constants;
using FoodstoreApi.Core.Entities.Identity;
using FoodstoreApi.Usecase.DTOs.Permission;
using FoodstoreApi.Usecase.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace FoodstoreApi.Usecase.Services;

public class PermissionService(RoleManager<ApplicationRole> roleManager) : IPermissionService
{
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;

    public Task<Dictionary<string, List<PermissionDto>>> GetAllPermissionsGroupedByModuleAsync(CancellationToken cancellationToken = default)
    {
        var grouped = Permissions.All
            .GroupBy(p => p.Module)
            .ToDictionary(
                g => g.Key,
                g => g.Select(p => new PermissionDto
                {
                    Code = p.Code,
                    Name = p.Name,
                    Description = p.Description,
                    Module = p.Module
                }).ToList()
            );

        return Task.FromResult(grouped);
    }

    public async Task<List<string>> GetRolePermissionCodesAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var role = await _roleManager.FindByIdAsync(roleId.ToString());
        if (role == null) return new List<string>();

        var claims = await _roleManager.GetClaimsAsync(role);
        return claims
            .Where(c => c.Type == "Permission")
            .Select(c => c.Value)
            .ToList();
    }

    public async Task AssignPermissionToRoleAsync(Guid roleId, string permissionCode, CancellationToken cancellationToken = default)
    {
        var role = await _roleManager.FindByIdAsync(roleId.ToString());
        if (role == null) return;

        var existingClaims = await _roleManager.GetClaimsAsync(role);
        if (!existingClaims.Any(c => c.Type == "Permission" && c.Value == permissionCode))
        {
            await _roleManager.AddClaimAsync(role, new Claim("Permission", permissionCode));
        }
    }

    public async Task RemovePermissionFromRoleAsync(Guid roleId, string permissionCode, CancellationToken cancellationToken = default)
    {
        var role = await _roleManager.FindByIdAsync(roleId.ToString());
        if (role == null) return;

        var claims = await _roleManager.GetClaimsAsync(role);
        var claim = claims.FirstOrDefault(c => c.Type == "Permission" && c.Value == permissionCode);
        if (claim != null)
        {
            await _roleManager.RemoveClaimAsync(role, claim);
        }
    }

    public async Task BulkUpdateRolePermissionsAsync(Guid roleId, List<string> permissionCodes, CancellationToken cancellationToken = default)
    {
        var role = await _roleManager.FindByIdAsync(roleId.ToString());
        if (role == null) return;

        var existingClaims = await _roleManager.GetClaimsAsync(role);
        var existingPermissions = existingClaims.Where(c => c.Type == "Permission").ToList();
        var requestedCodes = permissionCodes.Distinct().ToHashSet();

        // Remove
        foreach (var claim in existingPermissions)
        {
            if (!requestedCodes.Contains(claim.Value))
            {
                await _roleManager.RemoveClaimAsync(role, claim);
            }
        }

        // Add
        foreach (var code in requestedCodes)
        {
            if (!existingPermissions.Any(c => c.Value == code))
            {
                await _roleManager.AddClaimAsync(role, new Claim("Permission", code));
            }
        }
    }
}
