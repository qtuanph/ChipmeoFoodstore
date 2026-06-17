using FoodstoreApi.Usecase.DTOs.Addon;

namespace FoodstoreApi.Usecase.Interfaces;

public interface IAddonService
{
    Task<IEnumerable<AddonDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<AddonDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<AddonDto> CreateAsync(CreateAddonDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, CreateAddonDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}




