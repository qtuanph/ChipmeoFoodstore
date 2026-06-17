using FoodstoreApi.Usecase.DTOs.Permission;
using FoodstoreApi.Core.Entities;

namespace FoodstoreApi.Usecase.Interfaces;

public interface IPermissionService
{
    Task<Dictionary<string, List<PermissionDto>>> GetAllPermissionsGroupedByModuleAsync(CancellationToken cancellationToken = default);
    Task<List<int>> GetRolePermissionIdsAsync(int roleId, CancellationToken cancellationToken = default);
    Task AssignPermissionToRoleAsync(int roleId, int permissionId, CancellationToken cancellationToken = default);
    Task RemovePermissionFromRoleAsync(int roleId, int permissionId, CancellationToken cancellationToken = default);
    Task BulkUpdateRolePermissionsAsync(int roleId, List<int> permissionIds, CancellationToken cancellationToken = default);
}




