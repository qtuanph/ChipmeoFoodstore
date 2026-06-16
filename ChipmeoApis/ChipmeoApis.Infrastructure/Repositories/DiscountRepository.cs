using ChipmeoApis.Usecase.Interfaces;
using ChipmeoApis.Core.Entities;
using ChipmeoApis.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ChipmeoApis.Infrastructure.Repositories;

public class DiscountRepository : IDiscountRepository
{
    private readonly StoreDbContext _context;

    public DiscountRepository(StoreDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Discount>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Discounts.ToListAsync(cancellationToken);
    }

    public async Task<Discount?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Discounts.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<Discount?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.Discounts
            .FirstOrDefaultAsync(d => d.Code == code, cancellationToken);
    }

    public async Task<Discount> CreateAsync(Discount discount, CancellationToken cancellationToken = default)
    {
        _context.Discounts.Add(discount);
        await _context.SaveChangesAsync(cancellationToken);
        return discount;
    }

    public async Task<bool> UpdateAsync(Discount discount, CancellationToken cancellationToken = default)
    {
        _context.Discounts.Update(discount);
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var discount = await _context.Discounts.FindAsync(new object[] { id }, cancellationToken);
        if (discount == null) return false;
        
        _context.Discounts.Remove(discount);
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }
}




