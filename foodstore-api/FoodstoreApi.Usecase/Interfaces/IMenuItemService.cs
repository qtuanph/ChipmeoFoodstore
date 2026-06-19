using FoodstoreApi.Usecase.DTOs.MenuItem;

namespace FoodstoreApi.Usecase.Interfaces;

public interface IMenuItemService
{
    Task<IEnumerable<MenuItemDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<MenuItemDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<MenuItemDto> CreateAsync(CreateMenuItemDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Guid id, CreateMenuItemDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
