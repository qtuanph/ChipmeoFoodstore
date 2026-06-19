using FoodstoreApi.Usecase.Interfaces;
using FoodstoreApi.Core.Entities;
using FoodstoreApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FoodstoreApi.Infrastructure.Repositories;

public class BlogSettingRepository : IBlogSettingRepository
{
    private readonly StoreDbContext _context;

    public BlogSettingRepository(StoreDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BlogSetting>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Set<BlogSetting>()
            .OrderBy(s => s.Key)
            .ToListAsync(ct);
    }

    public async Task<BlogSetting?> GetByKeyAsync(string key, CancellationToken ct = default)
    {
        return await _context.Set<BlogSetting>()
            .FirstOrDefaultAsync(s => s.Key == key, ct);
    }

    public async Task<BlogSetting> UpsertAsync(BlogSetting setting, CancellationToken ct = default)
    {
        var existing = await _context.Set<BlogSetting>()
            .FirstOrDefaultAsync(s => s.Key == setting.Key, ct);
        if (existing != null)
        {
            existing.Value = setting.Value;
            existing.Description = setting.Description;
            _context.Set<BlogSetting>().Update(existing);
        }
        else
        {
            setting.Id = Guid.NewGuid();
            _context.Set<BlogSetting>().Add(setting);
        }
        await _context.SaveChangesAsync(ct);
        return existing ?? setting;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var setting = await _context.Set<BlogSetting>().FindAsync(new object[] { id }, ct);
        if (setting == null) return false;
        _context.Set<BlogSetting>().Remove(setting);
        return await _context.SaveChangesAsync(ct) > 0;
    }
}
