using FoodstoreApi.Core.Entities;

namespace FoodstoreApi.Usecase.Interfaces;

public interface IBlogBlockRepository
{
    Task<IEnumerable<BlogPostBlock>> GetByPostIdAsync(Guid postId, CancellationToken ct = default);
    Task<BlogPostBlock?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<BlogPostBlock> CreateAsync(BlogPostBlock block, CancellationToken ct = default);
    Task<bool> UpdateAsync(BlogPostBlock block, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
    Task DeleteByPostIdAsync(Guid postId, CancellationToken ct = default);
    Task SetPostBlocksAsync(Guid postId, List<BlogPostBlock> blocks, CancellationToken ct = default);
}
