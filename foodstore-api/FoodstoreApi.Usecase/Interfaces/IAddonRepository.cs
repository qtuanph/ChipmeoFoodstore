using FoodstoreApi.Core.Entities;

namespace FoodstoreApi.Usecase.Interfaces;

public interface IAddonRepository
{
    Task<IEnumerable<Addon>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Addon?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Addon> CreateAsync(Addon addon, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Addon addon, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
