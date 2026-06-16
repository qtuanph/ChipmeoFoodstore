using ChipmeoApis.Usecase.Interfaces;
using ChipmeoApis.Usecase.DTOs.Role;
using ChipmeoApis.Core.Entities;
using ChipmeoApis.Core.Utils;

namespace ChipmeoApis.Usecase.Services;

public class RoleService(IRoleRepository repository, IPermissionRepository permissionRepository) : IRoleService
{
    private readonly IRoleRepository _repository = repository;
    private readonly IPermissionRepository _permissionRepository = permissionRepository;

    public async Task<IEnumerable<RoleDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _repository.GetAllAsync(cancellationToken);
        return roles.Select(MapToDto);
    }

    public async Task<RoleDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var role = await _repository.GetByIdAsync(id, cancellationToken);
        return role == null ? null : MapToDto(role);
    }

    public async Task<RoleDto> CreateAsync(CreateRoleDto dto, CancellationToken cancellationToken = default)
    {
        var role = new Role
        {
            Name = dto.Name,
            Description = dto.Description,
            DefaultRoute = dto.DefaultRoute,
            IsActive = dto.IsActive,
            CreatedAt = TimeUtils.GetVietnamTime()
        };

        var created = await _repository.CreateAsync(role, cancellationToken);

        // Seed all permissions as inactive for the new role
        var allPermissions = await _permissionRepository.GetAllAsync(cancellationToken);
        var rolePermissions = allPermissions.Select(p => new RolePermission
        {
            RoleId = created.Id,
            PermissionId = p.Id,
            IsActive = false,
            CreatedAt = TimeUtils.GetVietnamTime()
        });
        
        await _repository.AddRolePermissionsAsync(rolePermissions, cancellationToken);

        return MapToDto(created);
    }

    public async Task<bool> UpdateAsync(int id, CreateRoleDto dto, CancellationToken cancellationToken = default)
    {
        var role = await _repository.GetByIdAsync(id, cancellationToken);
        if (role == null) return false;

        role.Name = dto.Name;
        role.Description = dto.Description;
        role.DefaultRoute = dto.DefaultRoute;
        role.IsActive = dto.IsActive;

        return await _repository.UpdateAsync(role, cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _repository.DeleteAsync(id, cancellationToken);
    }

    public async Task<bool> AssignPermissionsAsync(int roleId, AssignPermissionsDto dto, CancellationToken cancellationToken = default)
    {
        var role = await _repository.GetByIdAsync(roleId, cancellationToken);
        if (role == null) return false;

        // Get all system permissions
        var allPermissions = await _permissionRepository.GetAllAsync(cancellationToken);
        
        // Get existing role permissions
        var existingRolePermissions = (await _repository.GetRolePermissionsAsync(roleId, cancellationToken)).ToList();

        var requestedPermissionIds = dto.PermissionIds.Distinct().ToHashSet();

        // 1. Update existing permissions
        var permissionsToUpdate = new List<RolePermission>();
        foreach (var rp in existingRolePermissions)
        {
            if (rp.IsActive != requestedPermissionIds.Contains(rp.PermissionId))
            {
                rp.IsActive = requestedPermissionIds.Contains(rp.PermissionId);
                permissionsToUpdate.Add(rp);
            }
        }
        
        if (permissionsToUpdate.Any())
        {
            await _repository.UpdateRolePermissionsAsync(permissionsToUpdate, cancellationToken);
        }

        // 2. Add missing permissions (if any new permissions were added to the system)
        var existingPermissionIds = existingRolePermissions.Select(rp => rp.PermissionId).ToHashSet();
        var missingPermissions = allPermissions.Where(p => !existingPermissionIds.Contains(p.Id));
        var permissionsToAdd = new List<RolePermission>();

        foreach (var p in missingPermissions)
        {
            permissionsToAdd.Add(new RolePermission
            {
                RoleId = roleId,
                PermissionId = p.Id,
                IsActive = requestedPermissionIds.Contains(p.Id),
                CreatedAt = TimeUtils.GetVietnamTime()
            });
        }

        if (permissionsToAdd.Any())
        {
            await _repository.AddRolePermissionsAsync(permissionsToAdd, cancellationToken);
        }

        return true;
    }

    private static RoleDto MapToDto(Role role)
    {
        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            DefaultRoute = role.DefaultRoute,
            IsActive = role.IsActive ?? true,
            CreatedAt = role.CreatedAt ?? TimeUtils.GetVietnamTime(),
            Permissions = role.RolePermissions?
                .Where(rp => rp.IsActive)
                .Select(rp => new PermissionDto
            {
                Id = rp.Permission.Id,
                Code = rp.Permission.Code,
                Name = rp.Permission.Name,
                Description = rp.Permission.Description,
                Module = rp.Permission.Module ?? ""
            }).ToList() ?? new List<PermissionDto>()
        };
    }
}




