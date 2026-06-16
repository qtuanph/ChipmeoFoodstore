using ChipmeoApis.Usecase.DTOs.Role;

namespace ChipmeoApis.Usecase.Interfaces;

public interface IRoleService
{
    Task<IEnumerable<RoleDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<RoleDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<RoleDto> CreateAsync(CreateRoleDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, CreateRoleDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> AssignPermissionsAsync(int roleId, AssignPermissionsDto dto, CancellationToken cancellationToken = default);
}




