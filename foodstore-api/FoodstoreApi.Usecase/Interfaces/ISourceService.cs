using FoodstoreApi.Usecase.DTOs.Source;

namespace FoodstoreApi.Usecase.Interfaces;

public interface ISourceService
{
    Task<IEnumerable<SourceDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<SourceDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<SourceDto> CreateAsync(CreateSourceDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, CreateSourceDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}




