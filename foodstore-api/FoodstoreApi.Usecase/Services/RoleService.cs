using System.Security.Claims;
using FoodstoreApi.Core.Constants;
using FoodstoreApi.Core.Entities.Identity;
using FoodstoreApi.Usecase.DTOs.Role;
using FoodstoreApi.Usecase.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FoodstoreApi.Usecase.Services;

public class RoleService(RoleManager<ApplicationRole> roleManager) : IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;

    public async Task<IEnumerable<RoleDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _roleManager.Roles.ToListAsync(cancellationToken);
        var result = new List<RoleDto>();
        foreach (var role in roles)
        {
            var claims = await _roleManager.GetClaimsAsync(role);
            result.Add(MapToDto(role, claims));
        }
        return result;
    }

    public async Task<RoleDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var role = await _roleManager.FindByIdAsync(id.ToString());
        if (role == null) return null;
        var claims = await _roleManager.GetClaimsAsync(role);
        return MapToDto(role, claims);
    }

    public async Task<RoleDto> CreateAsync(CreateRoleDto dto, CancellationToken cancellationToken = default)
    {
        var role = new ApplicationRole
        {
            Name = dto.Name,
            Description = dto.Description,
            DefaultRoute = dto.DefaultRoute,

        };

        var result = await _roleManager.CreateAsync(role);
        if (!result.Succeeded)
            throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));

        return MapToDto(role, new List<Claim>());
    }

    private static readonly HashSet<string> ProtectedRoles = ["root", "customer"];

    public async Task<bool> UpdateAsync(Guid id, CreateRoleDto dto, CancellationToken cancellationToken = default)
    {
        var role = await _roleManager.FindByIdAsync(id.ToString());
        if (role == null) return false;
        if (ProtectedRoles.Contains(role.Name?.ToLowerInvariant() ?? ""))
            throw new InvalidOperationException($"Không thể sửa vai trò hệ thống \"{role.Name}\".");

        role.Name = dto.Name;
        role.Description = dto.Description;
        role.DefaultRoute = dto.DefaultRoute;

        var result = await _roleManager.UpdateAsync(role);
        return result.Succeeded;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var role = await _roleManager.FindByIdAsync(id.ToString());
        if (role == null) return false;
        if (ProtectedRoles.Contains(role.Name?.ToLowerInvariant() ?? ""))
            throw new InvalidOperationException($"Không thể xóa vai trò hệ thống \"{role.Name}\".");

        var result = await _roleManager.DeleteAsync(role);
        return result.Succeeded;
    }

    public async Task<bool> AssignPermissionsAsync(Guid roleId, AssignPermissionsDto dto, CancellationToken cancellationToken = default)
    {
        var role = await _roleManager.FindByIdAsync(roleId.ToString());
        if (role == null) return false;
        if (ProtectedRoles.Contains(role.Name?.ToLowerInvariant() ?? ""))
            throw new InvalidOperationException($"Không thể phân quyền cho vai trò hệ thống \"{role.Name}\".");

        var existingClaims = await _roleManager.GetClaimsAsync(role);
        var existingPermissions = existingClaims.Where(c => c.Type == "Permission").ToList();

        // Remove permissions not in the new list
        foreach (var claim in existingPermissions)
        {
            if (!dto.PermissionCodes.Contains(claim.Value))
            {
                await _roleManager.RemoveClaimAsync(role, claim);
            }
        }

        // Add new permissions
        foreach (var code in dto.PermissionCodes)
        {
            if (!existingPermissions.Any(c => c.Value == code))
            {
                await _roleManager.AddClaimAsync(role, new Claim("Permission", code));
            }
        }

        return true;
    }

    private static RoleDto MapToDto(ApplicationRole role, IList<Claim> claims)
    {
        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name ?? "",
            Description = role.Description,
            DefaultRoute = role.DefaultRoute,
            IsActive = true,
            CreatedAt = role.CreatedAt,
            UpdatedAt = role.UpdatedAt,
            CreatedBy = role.CreatedBy,
            UpdatedBy = role.UpdatedBy,
            PermissionCodes = claims
                .Where(c => c.Type == "Permission")
                .Select(c => c.Value)
                .ToList()
        };
    }
}
