using FoodstoreApi.Usecase.Interfaces;
using FoodstoreApi.Core.Entities;
using FoodstoreApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FoodstoreApi.Infrastructure.Repositories;

public class MediaRepository : IMediaRepository
{
    private readonly StoreDbContext _context;

    public MediaRepository(StoreDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Media>> GetAllAsync()
    {
        return await _context.Media.OrderByDescending(m => m.CreatedAt).ToListAsync();
    }

    public async Task<Media?> GetByIdAsync(int id)
    {
        return await _context.Media.FindAsync(id);
    }

    public async Task<Media?> GetByUrlAsync(string url)
    {
        return await _context.Media.FirstOrDefaultAsync(m => m.FileUrl == url);
    }

    public async Task<IEnumerable<Media>> GetByEntityAsync(string entityType, int entityId)
    {
        return await _context.Media
            .Where(m => m.EntityType == entityType && m.EntityId == entityId)
            .ToListAsync();
    }

    public async Task<Media> AddAsync(Media media)
    {
        _context.Media.Add(media);
        await _context.SaveChangesAsync();
        return media;
    }

    public async Task UpdateAsync(Media media)
    {
        _context.Media.Update(media);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Media media)
    {
        _context.Media.Remove(media);
        await _context.SaveChangesAsync();
    }
}




