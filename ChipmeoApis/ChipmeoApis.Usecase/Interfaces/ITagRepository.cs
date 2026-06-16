using ChipmeoApis.Core.Entities;

namespace ChipmeoApis.Usecase.Interfaces;

public interface ITagRepository
{
    Task<IEnumerable<Tag>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Tag?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Tag?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<Tag> CreateAsync(Tag tag, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Tag tag, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Tag>> GetByPostIdAsync(int postId, CancellationToken cancellationToken = default);
    Task AddTagToPostAsync(int postId, int tagId, CancellationToken cancellationToken = default);
    Task RemoveTagFromPostAsync(int postId, int tagId, CancellationToken cancellationToken = default);
    Task SetPostTagsAsync(int postId, IEnumerable<int> tagIds, CancellationToken cancellationToken = default);
}




