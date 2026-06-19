using FoodstoreApi.Usecase.DTOs.Role;

namespace FoodstoreApi.Usecase.Interfaces;

public interface IRoleService
{
    Task<IEnumerable<RoleDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<RoleDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<RoleDto> CreateAsync(CreateRoleDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Guid id, CreateRoleDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> AssignPermissionsAsync(Guid roleId, AssignPermissionsDto dto, CancellationToken cancellationToken = default);
}
