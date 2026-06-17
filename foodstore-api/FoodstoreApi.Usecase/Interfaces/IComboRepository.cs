using FoodstoreApi.Core.Entities;

namespace FoodstoreApi.Usecase.Interfaces;

public interface IComboRepository
{
    Task<IEnumerable<Combo>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Combo?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Combo> CreateAsync(Combo combo, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Combo combo, CancellationToken cancellationToken = default);
    Task<bool> UpdateWithItemsAsync(Combo combo, IEnumerable<ComboItem> newItems, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}




