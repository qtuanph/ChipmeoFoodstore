using ChipmeoApis.Usecase.Interfaces;
using ChipmeoApis.Usecase.DTOs.Source;
using ChipmeoApis.Usecase.Utils;
using ChipmeoApis.Core.Constants;
using ChipmeoApis.Core.Entities;
using ChipmeoApis.Core.Utils;
using Microsoft.Extensions.Caching.Distributed;

namespace ChipmeoApis.Usecase.Services;

public class SourceService : ISourceService
{
    private readonly ISourceRepository _repository;
    private readonly IDistributedCache _cache;

    public SourceService(ISourceRepository repository, IDistributedCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<IEnumerable<SourceDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _cache.GetOrSetAsync(CacheKeys.Sources.All, async () =>
        {
            var sources = await _repository.GetAllAsync(cancellationToken);
            return sources.Select(MapToDto).ToList();
        }, TimeSpan.FromMinutes(30), cancellationToken);
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
        await _cache.RemoveAsync(CacheKeys.Sources.All, cancellationToken);
        return MapToDto(created);
    }

    public async Task<bool> UpdateAsync(int id, CreateSourceDto dto, CancellationToken cancellationToken = default)
    {
        var source = await _repository.GetByIdAsync(id, cancellationToken);
        if (source == null) return false;

        source.Name = dto.Name;
        source.IsActive = dto.IsActive;

        var result = await _repository.UpdateAsync(source, cancellationToken);
        if (result)
        {
            await _cache.RemoveAsync(CacheKeys.Sources.All, cancellationToken);
            await _cache.RemoveAsync(CacheKeys.Sources.ById(id), cancellationToken);
        }
        return result;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var result = await _repository.DeleteAsync(id, cancellationToken);
        if (result)
        {
            await _cache.RemoveAsync(CacheKeys.Sources.All, cancellationToken);
            await _cache.RemoveAsync(CacheKeys.Sources.ById(id), cancellationToken);
        }
        return result;
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
