using FoodstoreApi.Core.Entities;

namespace FoodstoreApi.Usecase.Interfaces;

public interface IBlogRepository
{
    Task<IEnumerable<BlogPost>> GetAllAsync(string? status = null, string? categorySlug = null, string? tagSlug = null);
    Task<BlogPost?> GetByIdAsync(Guid id);
    Task<BlogPost?> GetBySlugAsync(string slug);
    Task<BlogPost> AddAsync(BlogPost post);
    Task UpdateAsync(BlogPost post);
    Task DeleteAsync(BlogPost post);
    Task<IEnumerable<BlogPost>> GetFeaturedAsync(int limit = 5);
    Task<IEnumerable<BlogPost>> GetPublishedAsync(int page = 1, int limit = 10, string? categorySlug = null, string? tagSlug = null);
    Task<int> GetPublishedCountAsync(string? categorySlug = null, string? tagSlug = null);
}
