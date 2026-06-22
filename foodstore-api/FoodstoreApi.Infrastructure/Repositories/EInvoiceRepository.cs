using FoodstoreApi.Core.Entities;
using FoodstoreApi.Infrastructure.Data;
using FoodstoreApi.Usecase.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodstoreApi.Infrastructure.Repositories;

public class EInvoiceRepository(StoreDbContext context) : IEInvoiceRepository
{
    private readonly StoreDbContext _context = context;

    public async Task<List<EInvoiceProvider>> GetAllProvidersAsync(CancellationToken ct = default)
    {
        return await _context.EInvoiceProviders
            .OrderBy(p => p.Name)
            .ToListAsync(ct);
    }

    public async Task<EInvoiceProvider?> GetProviderByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.EInvoiceProviders.FindAsync(new object[] { id }, ct);
    }

    public async Task<EInvoiceProvider> AddProviderAsync(EInvoiceProvider provider, CancellationToken ct = default)
    {
        await _context.EInvoiceProviders.AddAsync(provider, ct);
        await _context.SaveChangesAsync(ct);
        return provider;
    }

    public async Task UpdateProviderAsync(EInvoiceProvider provider, CancellationToken ct = default)
    {
        _context.EInvoiceProviders.Update(provider);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteProviderAsync(EInvoiceProvider provider, CancellationToken ct = default)
    {
        _context.EInvoiceProviders.Remove(provider);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<List<EInvoice>> GetAllInvoicesAsync(CancellationToken ct = default)
    {
        return await _context.EInvoices
            .Include(i => i.Order)
            .Include(i => i.Provider)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<EInvoice?> GetInvoiceByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.EInvoices
            .Include(i => i.Order)
            .Include(i => i.Provider)
            .FirstOrDefaultAsync(i => i.Id == id, ct);
    }

    public async Task<EInvoice?> GetInvoiceByOrderIdAsync(Guid orderId, CancellationToken ct = default)
    {
        return await _context.EInvoices
            .Include(i => i.Provider)
            .FirstOrDefaultAsync(i => i.OrderId == orderId, ct);
    }

    public async Task<EInvoice> AddInvoiceAsync(EInvoice invoice, CancellationToken ct = default)
    {
        await _context.EInvoices.AddAsync(invoice, ct);
        await _context.SaveChangesAsync(ct);
        return invoice;
    }

    public async Task UpdateInvoiceAsync(EInvoice invoice, CancellationToken ct = default)
    {
        _context.EInvoices.Update(invoice);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<EInvoiceSetting?> GetSettingsAsync(CancellationToken ct = default)
    {
        return await _context.EInvoiceSettings
            .Include(s => s.DefaultProvider)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<EInvoiceSetting> AddOrUpdateSettingsAsync(EInvoiceSetting settings, CancellationToken ct = default)
    {
        var existing = await _context.EInvoiceSettings.FirstOrDefaultAsync(ct);
        if (existing == null)
        {
            await _context.EInvoiceSettings.AddAsync(settings, ct);
        }
        else
        {
            existing.DefaultProviderId = settings.DefaultProviderId;
            existing.AutoIssue = settings.AutoIssue;
            existing.DefaultTemplateCode = settings.DefaultTemplateCode;
            existing.DefaultSerialNumber = settings.DefaultSerialNumber;
            existing.DigitalSignatureConfig = settings.DigitalSignatureConfig;
            existing.UpdatedAt = settings.UpdatedAt;
            _context.EInvoiceSettings.Update(existing);
        }
        await _context.SaveChangesAsync(ct);
        return existing ?? settings;
    }

    public async Task<(int total, int draft, int issued, int failed, int cancelled)> GetInvoiceCountsAsync(CancellationToken ct = default)
    {
        var total = await _context.EInvoices.CountAsync(ct);
        var draft = await _context.EInvoices.CountAsync(i => i.Status == "draft", ct);
        var issued = await _context.EInvoices.CountAsync(i => i.Status == "issued", ct);
        var failed = await _context.EInvoices.CountAsync(i => i.Status == "failed", ct);
        var cancelled = await _context.EInvoices.CountAsync(i => i.Status == "cancelled", ct);
        return (total, draft, issued, failed, cancelled);
    }

    public async Task<int> GetActiveProviderCountAsync(CancellationToken ct = default)
    {
        return await _context.EInvoiceProviders.CountAsync(p => p.IsActive, ct);
    }
}
