using FoodstoreApi.Core.Entities;

namespace FoodstoreApi.Usecase.Interfaces;

public interface IBlogCategoryRepository
{
    Task<IEnumerable<BlogCategory>> GetAllAsync(CancellationToken ct = default);
    Task<BlogCategory?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<BlogCategory?> GetBySlugAsync(string slug, CancellationToken ct = default);
    Task<BlogCategory> CreateAsync(BlogCategory category, CancellationToken ct = default);
    Task<bool> UpdateAsync(BlogCategory category, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<int> GetPostCountAsync(Guid categoryId, CancellationToken ct = default);
}
