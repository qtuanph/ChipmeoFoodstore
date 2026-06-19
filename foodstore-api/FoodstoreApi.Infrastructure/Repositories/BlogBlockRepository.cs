using FoodstoreApi.Usecase.Interfaces;
using FoodstoreApi.Core.Entities;
using FoodstoreApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FoodstoreApi.Infrastructure.Repositories;

public class BlogBlockRepository : IBlogBlockRepository
{
    private readonly StoreDbContext _context;

    public BlogBlockRepository(StoreDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BlogPostBlock>> GetByPostIdAsync(Guid postId, CancellationToken ct = default)
    {
        return await _context.Set<BlogPostBlock>()
            .Where(b => b.PostId == postId)
            .OrderBy(b => b.SortOrder)
            .ToListAsync(ct);
    }

    public async Task<BlogPostBlock?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Set<BlogPostBlock>()
            .FirstOrDefaultAsync(b => b.Id == id, ct);
    }

    public async Task<BlogPostBlock> CreateAsync(BlogPostBlock block, CancellationToken ct = default)
    {
        _context.Set<BlogPostBlock>().Add(block);
        await _context.SaveChangesAsync(ct);
        return block;
    }

    public async Task<bool> UpdateAsync(BlogPostBlock block, CancellationToken ct = default)
    {
        _context.Set<BlogPostBlock>().Update(block);
        return await _context.SaveChangesAsync(ct) > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var block = await _context.Set<BlogPostBlock>().FindAsync(new object[] { id }, ct);
        if (block == null) return false;
        _context.Set<BlogPostBlock>().Remove(block);
        return await _context.SaveChangesAsync(ct) > 0;
    }

    public async Task DeleteByPostIdAsync(Guid postId, CancellationToken ct = default)
    {
        var blocks = await _context.Set<BlogPostBlock>()
            .Where(b => b.PostId == postId).ToListAsync(ct);
        _context.Set<BlogPostBlock>().RemoveRange(blocks);
        await _context.SaveChangesAsync(ct);
    }

    public async Task SetPostBlocksAsync(Guid postId, List<BlogPostBlock> blocks, CancellationToken ct = default)
    {
        var existing = await _context.Set<BlogPostBlock>()
            .Where(b => b.PostId == postId).ToListAsync(ct);
        _context.Set<BlogPostBlock>().RemoveRange(existing);
        _context.Set<BlogPostBlock>().AddRange(blocks);
        await _context.SaveChangesAsync(ct);
    }
}
