using FoodstoreApi.Usecase.DTOs.Blog;

namespace FoodstoreApi.Usecase.Interfaces;

public interface IBlogRevisionService
{
    Task<List<BlogPostRevisionDto>> GetByPostIdAsync(Guid postId, CancellationToken ct = default);
    Task<BlogPostRevisionDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<BlogPostRevisionDto> CreateSnapshotAsync(Guid postId, Guid? userId, CancellationToken ct = default);
    Task<BlogPostDto?> RestoreAsync(Guid revisionId, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
