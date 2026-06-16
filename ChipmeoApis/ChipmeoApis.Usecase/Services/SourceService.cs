using ChipmeoApis.Usecase.Interfaces;
using ChipmeoApis.Usecase.DTOs.Source;
using ChipmeoApis.Core.Entities;
using ChipmeoApis.Core.Utils;

namespace ChipmeoApis.Usecase.Services;

public class SourceService(ISourceRepository repository) : ISourceService
{
    private readonly ISourceRepository _repository = repository;

    public async Task<IEnumerable<SourceDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var sources = await _repository.GetAllAsync(cancellationToken);
        return sources.Select(MapToDto);
    }

    public async Task<SourceDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var source = await _repository.GetByIdAsync(id, cancellationToken);
        return source == null ? null : MapToDto(source);
    }

    public async Task<SourceDto> CreateAsync(CreateSourceDto dto, CancellationToken cancellationToken = default)
    {
        var source = new Source
        {
            Name = dto.Name,
            IsActive = dto.IsActive,
            CreatedAt = TimeUtils.GetVietnamTime()
        };

        var created = await _repository.CreateAsync(source, cancellationToken);
        return MapToDto(created);
    }

    public async Task<bool> UpdateAsync(int id, CreateSourceDto dto, CancellationToken cancellationToken = default)
    {
        var source = await _repository.GetByIdAsync(id, cancellationToken);
        if (source == null) return false;

        source.Name = dto.Name;
        source.IsActive = dto.IsActive;

        return await _repository.UpdateAsync(source, cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _repository.DeleteAsync(id, cancellationToken);
    }

    private static SourceDto MapToDto(Source source)
    {
        return new SourceDto
        {
            Id = source.Id,
            Name = source.Name,
            IsActive = source.IsActive ?? true,
            CreatedAt = source.CreatedAt ?? TimeUtils.GetVietnamTime()
        };
    }
}




