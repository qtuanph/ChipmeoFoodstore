using FoodstoreApi.Usecase.DTOs.Discount;

namespace FoodstoreApi.Usecase.Interfaces;

public interface IDiscountService
{
    Task<IEnumerable<DiscountDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<DiscountDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<DiscountDto> CreateAsync(CreateDiscountDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Guid id, CreateDiscountDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
