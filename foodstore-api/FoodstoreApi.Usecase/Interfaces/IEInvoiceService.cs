using FoodstoreApi.Usecase.DTOs.EInvoice;

namespace FoodstoreApi.Usecase.Interfaces;

public interface IEInvoiceService
{
    // Providers
    Task<List<EInvoiceProviderDto>> GetAllProvidersAsync(CancellationToken ct = default);
    Task<EInvoiceProviderDto?> GetProviderByIdAsync(Guid id, CancellationToken ct = default);
    Task<EInvoiceProviderDto> CreateProviderAsync(CreateEInvoiceProviderDto dto, Guid userId, CancellationToken ct = default);
    Task UpdateProviderAsync(Guid id, UpdateEInvoiceProviderDto dto, Guid userId, CancellationToken ct = default);
    Task DeleteProviderAsync(Guid id, CancellationToken ct = default);
    Task<bool> TestProviderConnectionAsync(Guid id, CancellationToken ct = default);

    // Invoices
    Task<List<EInvoiceDto>> GetAllInvoicesAsync(CancellationToken ct = default);
    Task<EInvoiceDto?> GetInvoiceByIdAsync(Guid id, CancellationToken ct = default);
    Task<EInvoiceDto> IssueInvoiceAsync(Guid orderId, IssueInvoiceDto dto, Guid userId, CancellationToken ct = default);
    Task<EInvoiceDto> CancelInvoiceAsync(Guid id, CancelInvoiceDto dto, CancellationToken ct = default);

    // Settings
    Task<EInvoiceSettingDto?> GetSettingsAsync(CancellationToken ct = default);
    Task<EInvoiceSettingDto> UpdateSettingsAsync(UpdateEInvoiceSettingDto dto, CancellationToken ct = default);

    // Dashboard
    Task<EInvoiceDashboardDto> GetDashboardAsync(CancellationToken ct = default);
}
