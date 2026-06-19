using FoodstoreApi.Core.Entities;

namespace FoodstoreApi.Usecase.Interfaces;

public interface IDiscountRepository
{
    Task<IEnumerable<Discount>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Discount?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Discount?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Discount> CreateAsync(Discount discount, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Discount discount, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
