using FoodstoreApi.Usecase.DTOs.Blog;
using FoodstoreApi.Usecase.Interfaces;
using FoodstoreApi.Core.Entities;

namespace FoodstoreApi.Usecase.Services;

public class BlogSettingService : IBlogSettingService
{
    private readonly IBlogSettingRepository _repo;

    public BlogSettingService(IBlogSettingRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<BlogSettingDto>> GetAllAsync(CancellationToken ct = default)
    {
        var settings = await _repo.GetAllAsync(ct);
        return settings.Select(s => new BlogSettingDto
        {
            Id = s.Id,
            Key = s.Key,
            Value = s.Value,
            Description = s.Description,
            UpdatedAt = s.UpdatedAt,
        }).ToList();
    }

    public async Task<BlogSettingDto?> GetByKeyAsync(string key, CancellationToken ct = default)
    {
        var setting = await _repo.GetByKeyAsync(key, ct);
        return setting == null ? null : new BlogSettingDto
        {
            Id = setting.Id,
            Key = setting.Key,
            Value = setting.Value,
            Description = setting.Description,
            UpdatedAt = setting.UpdatedAt,
        };
    }

    public async Task<BlogSettingDto> UpsertAsync(string key, UpdateBlogSettingDto dto, CancellationToken ct = default)
    {
        var setting = new BlogSetting { Key = key, Value = dto.Value, Description = dto.Description };
        var result = await _repo.UpsertAsync(setting, ct);
        return new BlogSettingDto
        {
            Id = result.Id,
            Key = result.Key,
            Value = result.Value,
            Description = result.Description,
            UpdatedAt = result.UpdatedAt,
        };
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        return await _repo.DeleteAsync(id, ct);
    }
}
