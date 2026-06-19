using FoodstoreApi.Usecase.DTOs.Blog;

namespace FoodstoreApi.Usecase.Interfaces;

public interface IBlogBlockService
{
    Task<List<BlogPostBlockDto>> GetByPostIdAsync(Guid postId, CancellationToken ct = default);
    Task<BlogPostBlockDto> CreateAsync(Guid postId, CreateBlogBlockDto dto, CancellationToken ct = default);
    Task<bool> UpdateAsync(Guid blockId, UpdateBlogBlockDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid blockId, CancellationToken ct = default);
    Task<List<BlogPostBlockDto>> SetPostBlocksAsync(Guid postId, List<CreateBlogBlockDto> blocks, CancellationToken ct = default);
}
