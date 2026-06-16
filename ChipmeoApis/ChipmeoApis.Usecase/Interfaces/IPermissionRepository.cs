using ChipmeoApis.Core.Entities;

namespace ChipmeoApis.Usecase.Interfaces;

public interface IPermissionRepository
{
    Task<IEnumerable<Permission>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Permission?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}




