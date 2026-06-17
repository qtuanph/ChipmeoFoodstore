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

    public async Task<IEnumerable<BlogPost>> GetAllAsync(string? status = null)
    {
        var query = _context.BlogPosts
            .Include(p => p.Author)
            .Include(p => p.BlogPostTags)
                .ThenInclude(pt => pt.Tag)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(p => p.Status == status);
        }

        return await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
    }

    public async Task<BlogPost?> GetByIdAsync(int id)
    {
        return await _context.BlogPosts
            .Include(p => p.Author)
            .Include(p => p.BlogPostTags)
                .ThenInclude(pt => pt.Tag)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<BlogPost?> GetBySlugAsync(string slug)
    {
        return await _context.BlogPosts
            .Include(p => p.Author)
            .Include(p => p.BlogPostTags)
                .ThenInclude(pt => pt.Tag)
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
}




