using FoodstoreApi.Usecase.DTOs.Source;

namespace FoodstoreApi.Usecase.Interfaces;

public interface ISourceService
{
    Task<IEnumerable<SourceDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<SourceDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<SourceDto> CreateAsync(CreateSourceDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Guid id, CreateSourceDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
