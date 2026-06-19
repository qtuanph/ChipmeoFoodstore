using FoodstoreApi.Usecase.Interfaces;
using FoodstoreApi.Core.Entities;
using FoodstoreApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FoodstoreApi.Infrastructure.Repositories;

public class PaymentSettingRepository(StoreDbContext context) : IPaymentSettingRepository
{
    private readonly StoreDbContext _context = context;

    public async Task<PaymentSetting?> GetDefaultAsync(CancellationToken cancellationToken = default)
    {
        return await _context.PaymentSettings
            .Where(ps => ps.IsActive)
            .OrderByDescending(ps => ps.IsDefault)
            .ThenBy(ps => ps.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<PaymentSetting>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.PaymentSettings
            .OrderByDescending(ps => ps.IsDefault)
            .ThenBy(ps => ps.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<PaymentSetting?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.PaymentSettings.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<PaymentSetting> AddAsync(PaymentSetting setting, CancellationToken cancellationToken = default)
    {
        await _context.PaymentSettings.AddAsync(setting, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return setting;
    }

    public async Task UpdateAsync(PaymentSetting setting, CancellationToken cancellationToken = default)
    {
        _context.PaymentSettings.Update(setting);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(PaymentSetting setting, CancellationToken cancellationToken = default)
    {
        _context.PaymentSettings.Remove(setting);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task ClearOtherDefaultsAsync(Guid? excludeId, CancellationToken cancellationToken = default)
    {
        var defaultSettings = await _context.PaymentSettings
            .Where(ps => ps.IsDefault && (excludeId == null || ps.Id != excludeId))
            .ToListAsync(cancellationToken);

        foreach (var setting in defaultSettings)
        {
            setting.IsDefault = false;
        }

        if (defaultSettings.Any())
        {
            _context.PaymentSettings.UpdateRange(defaultSettings);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
