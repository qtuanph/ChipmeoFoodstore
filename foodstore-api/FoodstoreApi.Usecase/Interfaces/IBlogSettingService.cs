using FoodstoreApi.Usecase.DTOs.Blog;

namespace FoodstoreApi.Usecase.Interfaces;

public interface IBlogSettingService
{
    Task<List<BlogSettingDto>> GetAllAsync(CancellationToken ct = default);
    Task<BlogSettingDto?> GetByKeyAsync(string key, CancellationToken ct = default);
    Task<BlogSettingDto> UpsertAsync(string key, UpdateBlogSettingDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
