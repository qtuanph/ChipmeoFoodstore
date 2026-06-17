using FoodstoreApi.Usecase.Interfaces;
using FoodstoreApi.Core.Entities;
using FoodstoreApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FoodstoreApi.Infrastructure.Repositories;

public class AddonRepository : IAddonRepository
{
    private readonly StoreDbContext _context;

    public AddonRepository(StoreDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Addon>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Addons.ToListAsync(cancellationToken);
    }

    public async Task<Addon?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Addons.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<Addon> CreateAsync(Addon addon, CancellationToken cancellationToken = default)
    {
        _context.Addons.Add(addon);
        await _context.SaveChangesAsync(cancellationToken);
        return addon;
    }

    public async Task<bool> UpdateAsync(Addon addon, CancellationToken cancellationToken = default)
    {
        _context.Addons.Update(addon);
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var addon = await _context.Addons.FindAsync(new object[] { id }, cancellationToken);
        if (addon == null) return false;
        
        _context.Addons.Remove(addon);
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }
}




