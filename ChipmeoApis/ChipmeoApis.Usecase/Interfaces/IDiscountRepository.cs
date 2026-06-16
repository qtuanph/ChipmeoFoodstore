using ChipmeoApis.Core.Entities;

namespace ChipmeoApis.Usecase.Interfaces;

public interface IDiscountRepository
{
    Task<IEnumerable<Discount>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Discount?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Discount?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Discount> CreateAsync(Discount discount, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Discount discount, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}




