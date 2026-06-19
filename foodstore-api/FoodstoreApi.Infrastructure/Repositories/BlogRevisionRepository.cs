using FoodstoreApi.Usecase.Interfaces;
using FoodstoreApi.Core.Entities;
using FoodstoreApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FoodstoreApi.Infrastructure.Repositories;

public class BlogRevisionRepository : IBlogRevisionRepository
{
    private readonly StoreDbContext _context;

    public BlogRevisionRepository(StoreDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BlogPostRevision>> GetByPostIdAsync(Guid postId, CancellationToken ct = default)
    {
        return await _context.Set<BlogPostRevision>()
            .Where(r => r.PostId == postId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<BlogPostRevision?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Set<BlogPostRevision>()
            .FirstOrDefaultAsync(r => r.Id == id, ct);
    }

    public async Task<BlogPostRevision> CreateAsync(BlogPostRevision revision, CancellationToken ct = default)
    {
        _context.Set<BlogPostRevision>().Add(revision);
        await _context.SaveChangesAsync(ct);
        return revision;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var revision = await _context.Set<BlogPostRevision>().FindAsync(new object[] { id }, ct);
        if (revision == null) return false;
        _context.Set<BlogPostRevision>().Remove(revision);
        return await _context.SaveChangesAsync(ct) > 0;
    }

    public async Task<int> GetRevisionCountAsync(Guid postId, CancellationToken ct = default)
    {
        return await _context.Set<BlogPostRevision>()
            .CountAsync(r => r.PostId == postId, ct);
    }
}
