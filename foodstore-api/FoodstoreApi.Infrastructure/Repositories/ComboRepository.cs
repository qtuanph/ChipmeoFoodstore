using FoodstoreApi.Usecase.Interfaces;
using FoodstoreApi.Core.Entities;
using FoodstoreApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FoodstoreApi.Infrastructure.Repositories;

public class ComboRepository : IComboRepository
{
    private readonly StoreDbContext _context;

    public ComboRepository(StoreDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Combo>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Combos
            .Include(c => c.ComboItems)
                .ThenInclude(ci => ci.MenuItem)
            .ToListAsync(cancellationToken);
    }

    public async Task<Combo?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Combos
            .Include(c => c.ComboItems)
                .ThenInclude(ci => ci.MenuItem)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Combo> CreateAsync(Combo combo, CancellationToken cancellationToken = default)
    {
        _context.Combos.Add(combo);
        await _context.SaveChangesAsync(cancellationToken);
        return combo;
    }

    public async Task<bool> UpdateAsync(Combo combo, CancellationToken cancellationToken = default)
    {
        _context.Combos.Update(combo);
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> UpdateWithItemsAsync(Combo combo, IEnumerable<ComboItem> newItems, CancellationToken cancellationToken = default)
    {
        // Remove old items
        var oldItems = await _context.ComboItems.Where(ci => ci.ComboId == combo.Id).ToListAsync(cancellationToken);
        _context.ComboItems.RemoveRange(oldItems);

        // Add new items
        await _context.ComboItems.AddRangeAsync(newItems, cancellationToken);

        // Update combo details
        _context.Combos.Update(combo);
        
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var combo = await _context.Combos
            .Include(c => c.ComboItems)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        
        if (combo == null) return false;
        
        _context.Combos.Remove(combo);
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }
}
