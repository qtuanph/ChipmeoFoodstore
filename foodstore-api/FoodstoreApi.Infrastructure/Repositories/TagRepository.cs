using FoodstoreApi.Usecase.Interfaces;
using FoodstoreApi.Core.Entities;
using FoodstoreApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FoodstoreApi.Infrastructure.Repositories;

public class TagRepository : ITagRepository
{
    private readonly StoreDbContext _context;

    public TagRepository(StoreDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Tag>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Tags
            .Include(t => t.BlogPostTags)
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Tag?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Tags
            .Include(t => t.BlogPostTags)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<Tag?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _context.Tags
            .Include(t => t.BlogPostTags)
            .FirstOrDefaultAsync(t => t.Slug == slug, cancellationToken);
    }

    public async Task<Tag> CreateAsync(Tag tag, CancellationToken cancellationToken = default)
    {
        _context.Tags.Add(tag);
        await _context.SaveChangesAsync(cancellationToken);
        return tag;
    }

    public async Task<bool> UpdateAsync(Tag tag, CancellationToken cancellationToken = default)
    {
        _context.Tags.Update(tag);
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tag = await _context.Tags.FindAsync(new object[] { id }, cancellationToken);
        if (tag == null) return false;

        _context.Tags.Remove(tag);
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<IEnumerable<Tag>> GetByPostIdAsync(Guid postId, CancellationToken cancellationToken = default)
    {
        return await _context.BlogPostTags
            .Where(bpt => bpt.PostId == postId)
            .Select(bpt => bpt.Tag)
            .ToListAsync(cancellationToken);
    }

    public async Task AddTagToPostAsync(Guid postId, Guid tagId, CancellationToken cancellationToken = default)
    {
        var exists = await _context.BlogPostTags
            .AnyAsync(bpt => bpt.PostId == postId && bpt.TagId == tagId, cancellationToken);

        if (!exists)
        {
            _context.BlogPostTags.Add(new BlogPostTag { PostId = postId, TagId = tagId });
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task RemoveTagFromPostAsync(Guid postId, Guid tagId, CancellationToken cancellationToken = default)
    {
        var blogPostTag = await _context.BlogPostTags
            .FirstOrDefaultAsync(bpt => bpt.PostId == postId && bpt.TagId == tagId, cancellationToken);

        if (blogPostTag != null)
        {
            _context.BlogPostTags.Remove(blogPostTag);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task SetPostTagsAsync(Guid postId, IEnumerable<Guid> tagIds, CancellationToken cancellationToken = default)
    {
        var existingTags = await _context.BlogPostTags
            .Where(bpt => bpt.PostId == postId)
            .ToListAsync(cancellationToken);

        _context.BlogPostTags.RemoveRange(existingTags);

        var newTags = tagIds.Select(tagId => new BlogPostTag { PostId = postId, TagId = tagId });
        _context.BlogPostTags.AddRange(newTags);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
