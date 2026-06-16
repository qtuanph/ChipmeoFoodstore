using ChipmeoApis.Usecase.DTOs.Discount;

namespace ChipmeoApis.Usecase.Interfaces;

public interface IDiscountService
{
    Task<IEnumerable<DiscountDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<DiscountDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<DiscountDto> CreateAsync(CreateDiscountDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, CreateDiscountDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}




