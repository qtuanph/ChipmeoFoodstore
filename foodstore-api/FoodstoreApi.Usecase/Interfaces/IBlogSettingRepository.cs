using FoodstoreApi.Core.Entities;

namespace FoodstoreApi.Usecase.Interfaces;

public interface IBlogSettingRepository
{
    Task<IEnumerable<BlogSetting>> GetAllAsync(CancellationToken ct = default);
    Task<BlogSetting?> GetByKeyAsync(string key, CancellationToken ct = default);
    Task<BlogSetting> UpsertAsync(BlogSetting setting, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
