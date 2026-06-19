using FoodstoreApi.Usecase.DTOs.Combo;

namespace FoodstoreApi.Usecase.Interfaces;

public interface IComboService
{
    Task<IEnumerable<ComboDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ComboDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ComboDto> CreateAsync(CreateComboDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Guid id, CreateComboDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
