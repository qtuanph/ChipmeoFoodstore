using FoodstoreApi.Core.Entities;

namespace FoodstoreApi.Usecase.Interfaces;

public interface ISourceRepository
{
    Task<IEnumerable<Source>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Source?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Source> CreateAsync(Source source, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Source source, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}




