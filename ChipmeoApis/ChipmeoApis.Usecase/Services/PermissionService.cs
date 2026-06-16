using ChipmeoApis.Usecase.DTOs.Permission;
using ChipmeoApis.Usecase.Interfaces;
using ChipmeoApis.Core.Entities;
using ChipmeoApis.Core.Utils;

namespace ChipmeoApis.Usecase.Services;

public class PermissionService(IPermissionRepository repository, IRoleRepository roleRepository) : IPermissionService
{
    private readonly IPermissionRepository _repository = repository;
    private readonly IRoleRepository _roleRepository = roleRepository;

    public async Task<IEnumerable<PermissionDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var permissions = await _repository.GetAllAsync(cancellationToken);
        return permissions.Select(p => new PermissionDto
        {
            Id = p.Id,
            Code = p.Code,
            Name = p.Name,
            Description = p.Description,
            Module = p.Module ?? ""
        });
    }

    public async Task<Dictionary<string, List<PermissionDto>>> GetAllPermissionsGroupedByModuleAsync(CancellationToken cancellationToken = default)
    {
        var permissions = await _repository.GetAllAsync(cancellationToken);
        return permissions
            .GroupBy(p => p.Module ?? "Other")
            .ToDictionary(
                g => g.Key,
                g => g.Select(p => new PermissionDto
                {
                    Id = p.Id,
                    Code = p.Code,
                    Name = p.Name,
                    Description = p.Description,
                    Module = p.Module ?? ""
                }).ToList()
            );
    }

    public async Task<List<int>> GetRolePermissionIdsAsync(int roleId, CancellationToken cancellationToken = default)
    {
        var rolePermissions = await _roleRepository.GetRolePermissionsAsync(roleId, cancellationToken);
        return rolePermissions
            .Where(rp => rp.IsActive)
            .Select(rp => rp.PermissionId)
            .ToList();
    }

    public async Task AssignPermissionToRoleAsync(int roleId, int permissionId, CancellationToken cancellationToken = default)
    {
        var rolePermissions = await _roleRepository.GetRolePermissionsAsync(roleId, cancellationToken);
        var existing = rolePermissions.FirstOrDefault(rp => rp.PermissionId == permissionId);

        if (existing != null)
        {
            if (!existing.IsActive)
            {
                existing.IsActive = true;
                await _roleRepository.UpdateRolePermissionsAsync(new[] { existing }, cancellationToken);
            }
        }
        else
        {
            await _roleRepository.AddRolePermissionsAsync(new[]
            {
                new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = permissionId,
                    IsActive = true,
                    CreatedAt = TimeUtils.GetVietnamTime()
                }
            }, cancellationToken);
        }
    }

    public async Task RemovePermissionFromRoleAsync(int roleId, int permissionId, CancellationToken cancellationToken = default)
    {
        var rolePermissions = await _roleRepository.GetRolePermissionsAsync(roleId, cancellationToken);
        var existing = rolePermissions.FirstOrDefault(rp => rp.PermissionId == permissionId);

        if (existing != null && existing.IsActive)
        {
            existing.IsActive = false;
            await _roleRepository.UpdateRolePermissionsAsync(new[] { existing }, cancellationToken);
        }
    }

    public async Task BulkUpdateRolePermissionsAsync(int roleId, List<int> permissionIds, CancellationToken cancellationToken = default)
    {
        var rolePermissions = (await _roleRepository.GetRolePermissionsAsync(roleId, cancellationToken)).ToList();
        var requestedIds = permissionIds.Distinct().ToHashSet();
        
        var toUpdate = new List<RolePermission>();
        var toAdd = new List<RolePermission>();

        // Update existing
        foreach (var rp in rolePermissions)
        {
            bool shouldBeActive = requestedIds.Contains(rp.PermissionId);
            if (rp.IsActive != shouldBeActive)
            {
                rp.IsActive = shouldBeActive;
                toUpdate.Add(rp);
            }
        }

        // Add new
        var existingIds = rolePermissions.Select(rp => rp.PermissionId).ToHashSet();
        foreach (var id in requestedIds)
        {
            if (!existingIds.Contains(id))
            {
                toAdd.Add(new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = id,
                    IsActive = true,
                    CreatedAt = TimeUtils.GetVietnamTime()
                });
            }
        }

        if (toUpdate.Any())
            await _roleRepository.UpdateRolePermissionsAsync(toUpdate, cancellationToken);
            
        if (toAdd.Any())
            await _roleRepository.AddRolePermissionsAsync(toAdd, cancellationToken);
    }
}




