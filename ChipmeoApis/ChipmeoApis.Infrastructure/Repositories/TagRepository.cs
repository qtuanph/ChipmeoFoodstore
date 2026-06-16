using ChipmeoApis.Usecase.Interfaces;
using ChipmeoApis.Core.Entities;
using ChipmeoApis.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ChipmeoApis.Infrastructure.Repositories;

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

    public async Task<Tag?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
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

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var tag = await _context.Tags.FindAsync(new object[] { id }, cancellationToken);
        if (tag == null) return false;

        _context.Tags.Remove(tag);
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<IEnumerable<Tag>> GetByPostIdAsync(int postId, CancellationToken cancellationToken = default)
    {
        return await _context.BlogPostTags
            .Where(bpt => bpt.PostId == postId)
            .Select(bpt => bpt.Tag)
            .ToListAsync(cancellationToken);
    }

    public async Task AddTagToPostAsync(int postId, int tagId, CancellationToken cancellationToken = default)
    {
        var exists = await _context.BlogPostTags
            .AnyAsync(bpt => bpt.PostId == postId && bpt.TagId == tagId, cancellationToken);

        if (!exists)
        {
            _context.BlogPostTags.Add(new BlogPostTag { PostId = postId, TagId = tagId });
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task RemoveTagFromPostAsync(int postId, int tagId, CancellationToken cancellationToken = default)
    {
        var blogPostTag = await _context.BlogPostTags
            .FirstOrDefaultAsync(bpt => bpt.PostId == postId && bpt.TagId == tagId, cancellationToken);

        if (blogPostTag != null)
        {
            _context.BlogPostTags.Remove(blogPostTag);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task SetPostTagsAsync(int postId, IEnumerable<int> tagIds, CancellationToken cancellationToken = default)
    {
        // Remove existing tags
        var existingTags = await _context.BlogPostTags
            .Where(bpt => bpt.PostId == postId)
            .ToListAsync(cancellationToken);

        _context.BlogPostTags.RemoveRange(existingTags);

        // Add new tags
        var newTags = tagIds.Select(tagId => new BlogPostTag { PostId = postId, TagId = tagId });
        _context.BlogPostTags.AddRange(newTags);

        await _context.SaveChangesAsync(cancellationToken);
    }
}




