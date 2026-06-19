using FoodstoreApi.Usecase.Interfaces;
using FoodstoreApi.Core.Entities;
using FoodstoreApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FoodstoreApi.Infrastructure.Repositories;

public class BlogCategoryRepository : IBlogCategoryRepository
{
    private readonly StoreDbContext _context;

    public BlogCategoryRepository(StoreDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BlogCategory>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Set<BlogCategory>()
            .Include(c => c.Children)
            .OrderBy(c => c.SortOrder).ThenBy(c => c.Name)
            .ToListAsync(ct);
    }

    public async Task<BlogCategory?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Set<BlogCategory>()
            .Include(c => c.Children)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task<BlogCategory?> GetBySlugAsync(string slug, CancellationToken ct = default)
    {
        return await _context.Set<BlogCategory>()
            .FirstOrDefaultAsync(c => c.Slug == slug, ct);
    }

    public async Task<BlogCategory> CreateAsync(BlogCategory category, CancellationToken ct = default)
    {
        _context.Set<BlogCategory>().Add(category);
        await _context.SaveChangesAsync(ct);
        return category;
    }

    public async Task<bool> UpdateAsync(BlogCategory category, CancellationToken ct = default)
    {
        _context.Set<BlogCategory>().Update(category);
        return await _context.SaveChangesAsync(ct) > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var category = await _context.Set<BlogCategory>().FindAsync(new object[] { id }, ct);
        if (category == null) return false;
        _context.Set<BlogCategory>().Remove(category);
        return await _context.SaveChangesAsync(ct) > 0;
    }

    public async Task<int> GetPostCountAsync(Guid categoryId, CancellationToken ct = default)
    {
        return await _context.Set<BlogPostCategory>()
            .CountAsync(pc => pc.CategoryId == categoryId, ct);
    }
}
