using ChipmeoApis.Core.Entities;

namespace ChipmeoApis.Usecase.Interfaces;

public interface IRoleRepository
{
    Task<IEnumerable<Role>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Role?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Role> CreateAsync(Role role, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Role role, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<RolePermission>> GetRolePermissionsAsync(int roleId, CancellationToken cancellationToken = default);
    Task AddRolePermissionsAsync(IEnumerable<RolePermission> permissions, CancellationToken cancellationToken = default);
    Task UpdateRolePermissionsAsync(IEnumerable<RolePermission> permissions, CancellationToken cancellationToken = default);
}




