using FoodstoreApi.Core.Entities;

namespace FoodstoreApi.Usecase.Interfaces;

public interface IBlogRevisionRepository
{
    Task<IEnumerable<BlogPostRevision>> GetByPostIdAsync(Guid postId, CancellationToken ct = default);
    Task<BlogPostRevision?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<BlogPostRevision> CreateAsync(BlogPostRevision revision, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<int> GetRevisionCountAsync(Guid postId, CancellationToken ct = default);
}
