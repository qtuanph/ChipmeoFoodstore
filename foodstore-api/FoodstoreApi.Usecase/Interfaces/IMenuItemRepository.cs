namespace FoodstoreApi.Usecase.Interfaces;

using FoodstoreApi.Core.Entities;

public interface IMenuItemRepository
{
    Task<IEnumerable<MenuItem>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<MenuItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<MenuItem> AddAsync(MenuItem item, CancellationToken cancellationToken = default);
    Task UpdateAsync(MenuItem item, CancellationToken cancellationToken = default);
    Task DeleteAsync(MenuItem item, CancellationToken cancellationToken = default);
}




