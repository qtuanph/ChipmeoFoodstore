using FoodstoreApi.Usecase.DTOs.Permission;

namespace FoodstoreApi.Usecase.Interfaces;

public interface IPermissionService
{
    Task<Dictionary<string, List<PermissionDto>>> GetAllPermissionsGroupedByModuleAsync(CancellationToken cancellationToken = default);
    Task<List<string>> GetRolePermissionCodesAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task AssignPermissionToRoleAsync(Guid roleId, string permissionCode, CancellationToken cancellationToken = default);
    Task RemovePermissionFromRoleAsync(Guid roleId, string permissionCode, CancellationToken cancellationToken = default);
    Task BulkUpdateRolePermissionsAsync(Guid roleId, List<string> permissionCodes, CancellationToken cancellationToken = default);
}
