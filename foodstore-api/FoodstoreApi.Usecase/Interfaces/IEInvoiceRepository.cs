using FoodstoreApi.Core.Entities;

namespace FoodstoreApi.Usecase.Interfaces;

public interface IEInvoiceRepository
{
    // Providers
    Task<List<EInvoiceProvider>> GetAllProvidersAsync(CancellationToken ct = default);
    Task<EInvoiceProvider?> GetProviderByIdAsync(Guid id, CancellationToken ct = default);
    Task<EInvoiceProvider> AddProviderAsync(EInvoiceProvider provider, CancellationToken ct = default);
    Task UpdateProviderAsync(EInvoiceProvider provider, CancellationToken ct = default);
    Task DeleteProviderAsync(EInvoiceProvider provider, CancellationToken ct = default);

    // Invoices
    Task<List<EInvoice>> GetAllInvoicesAsync(CancellationToken ct = default);
    Task<EInvoice?> GetInvoiceByIdAsync(Guid id, CancellationToken ct = default);
    Task<EInvoice?> GetInvoiceByOrderIdAsync(Guid orderId, CancellationToken ct = default);
    Task<EInvoice> AddInvoiceAsync(EInvoice invoice, CancellationToken ct = default);
    Task UpdateInvoiceAsync(EInvoice invoice, CancellationToken ct = default);

    // Settings
    Task<EInvoiceSetting?> GetSettingsAsync(CancellationToken ct = default);
    Task<EInvoiceSetting> AddOrUpdateSettingsAsync(EInvoiceSetting settings, CancellationToken ct = default);

    // Dashboard counts
    Task<(int total, int draft, int issued, int failed, int cancelled)> GetInvoiceCountsAsync(CancellationToken ct = default);
    Task<int> GetActiveProviderCountAsync(CancellationToken ct = default);
}
