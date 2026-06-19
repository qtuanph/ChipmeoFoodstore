using FoodstoreApi.Usecase.DTOs.Blog;

namespace FoodstoreApi.Usecase.Interfaces;

public interface IBlogCategoryService
{
    Task<List<BlogCategoryDto>> GetAllAsync(CancellationToken ct = default);
    Task<BlogCategoryDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<BlogCategoryDto> CreateAsync(CreateBlogCategoryDto dto, CancellationToken ct = default);
    Task<BlogCategoryDto?> UpdateAsync(Guid id, UpdateBlogCategoryDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
