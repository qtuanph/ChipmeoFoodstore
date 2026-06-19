using FoodstoreApi.Usecase.Interfaces;
using FoodstoreApi.Core.Entities;
using FoodstoreApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FoodstoreApi.Infrastructure.Repositories;

public class BlogRepository : IBlogRepository
{
    private readonly StoreDbContext _context;

    public BlogRepository(StoreDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BlogPost>> GetAllAsync(string? status = null, string? categorySlug = null, string? tagSlug = null)
    {
        var query = BuildBaseQuery();

        if (!string.IsNullOrEmpty(status))
            query = query.Where(p => p.Status == status);

        if (!string.IsNullOrEmpty(categorySlug))
            query = query.Where(p => p.BlogPostCategories.Any(pc => pc.Category.Slug == categorySlug));

        if (!string.IsNullOrEmpty(tagSlug))
            query = query.Where(p => p.BlogPostTags.Any(pt => pt.Tag.Slug == tagSlug));

        return await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
    }

    public async Task<BlogPost?> GetByIdAsync(Guid id)
    {
        return await BuildBaseQuery()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<BlogPost?> GetBySlugAsync(string slug)
    {
        return await BuildBaseQuery()
            .FirstOrDefaultAsync(p => p.Slug == slug);
    }

    public async Task<BlogPost> AddAsync(BlogPost post)
    {
        _context.BlogPosts.Add(post);
        await _context.SaveChangesAsync();
        return post;
    }

    public async Task UpdateAsync(BlogPost post)
    {
        _context.BlogPosts.Update(post);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(BlogPost post)
    {
        _context.BlogPosts.Remove(post);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<BlogPost>> GetFeaturedAsync(int limit = 5)
    {
        return await BuildBaseQuery()
            .Where(p => p.Status == "published" && p.IsFeatured)
            .OrderByDescending(p => p.PublishedAt)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<IEnumerable<BlogPost>> GetPublishedAsync(int page = 1, int limit = 10, string? categorySlug = null, string? tagSlug = null)
    {
        var query = BuildBaseQuery().Where(p => p.Status == "published");

        if (!string.IsNullOrEmpty(categorySlug))
            query = query.Where(p => p.BlogPostCategories.Any(pc => pc.Category.Slug == categorySlug));

        if (!string.IsNullOrEmpty(tagSlug))
            query = query.Where(p => p.BlogPostTags.Any(pt => pt.Tag.Slug == tagSlug));

        return await query
            .OrderByDescending(p => p.PublishedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<int> GetPublishedCountAsync(string? categorySlug = null, string? tagSlug = null)
    {
        var query = _context.BlogPosts.Where(p => p.Status == "published");

        if (!string.IsNullOrEmpty(categorySlug))
            query = query.Where(p => p.BlogPostCategories.Any(pc => pc.Category.Slug == categorySlug));

        if (!string.IsNullOrEmpty(tagSlug))
            query = query.Where(p => p.BlogPostTags.Any(pt => pt.Tag.Slug == tagSlug));

        return await query.CountAsync();
    }

    private IQueryable<BlogPost> BuildBaseQuery()
    {
        return _context.BlogPosts
            .Include(p => p.Author).ThenInclude(a => a!.User)
            .Include(p => p.ReviewedByNavigation).ThenInclude(r => r!.User)
            .Include(p => p.BlogPostTags).ThenInclude(pt => pt.Tag)
            .Include(p => p.BlogPostCategories).ThenInclude(pc => pc.Category)
            .Include(p => p.BlogPostBlocks);
    }
}
