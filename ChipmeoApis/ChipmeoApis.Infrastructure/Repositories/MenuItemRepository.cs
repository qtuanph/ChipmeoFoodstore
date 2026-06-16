using ChipmeoApis.Usecase.Interfaces;
using ChipmeoApis.Core.Entities;
using ChipmeoApis.Infrastructure.Data;
using ChipmeoApis.Core.Utils;
using Microsoft.EntityFrameworkCore;

namespace ChipmeoApis.Infrastructure.Repositories;

public class MenuItemRepository : IMenuItemRepository
{
    private readonly StoreDbContext _db;

    public MenuItemRepository(StoreDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<MenuItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _db.MenuItems.AsNoTracking()
            .Include(m => m.Category)
            .Include(m => m.MenuItemAddons).ThenInclude(ma => ma.Addon)
            .OrderBy(m => m.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<MenuItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _db.MenuItems
            .Include(m => m.Category)
            .Include(m => m.MenuItemAddons).ThenInclude(ma => ma.Addon)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<MenuItem> AddAsync(MenuItem item, CancellationToken cancellationToken = default)
    {
        item.CreatedAt = TimeUtils.GetVietnamTime();
        _db.MenuItems.Add(item);
        await _db.SaveChangesAsync(cancellationToken);
        return item;
    }

    public async Task UpdateAsync(MenuItem item, CancellationToken cancellationToken = default)
    {
        _db.MenuItems.Update(item);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(MenuItem item, CancellationToken cancellationToken = default)
    {
        _db.MenuItems.Remove(item);
        await _db.SaveChangesAsync(cancellationToken);
    }
}




