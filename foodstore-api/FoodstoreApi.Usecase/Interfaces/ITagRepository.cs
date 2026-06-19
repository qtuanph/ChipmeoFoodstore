using FoodstoreApi.Core.Entities;

namespace FoodstoreApi.Usecase.Interfaces;

public interface ITagRepository
{
    Task<IEnumerable<Tag>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Tag?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Tag?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<Tag> CreateAsync(Tag tag, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Tag tag, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Tag>> GetByPostIdAsync(Guid postId, CancellationToken cancellationToken = default);
    Task AddTagToPostAsync(Guid postId, Guid tagId, CancellationToken cancellationToken = default);
    Task RemoveTagFromPostAsync(Guid postId, Guid tagId, CancellationToken cancellationToken = default);
    Task SetPostTagsAsync(Guid postId, IEnumerable<Guid> tagIds, CancellationToken cancellationToken = default);
}
