using System.Text.Json;
using FoodstoreApi.Core.Entities;
using FoodstoreApi.Usecase.DTOs.EInvoice;
using FoodstoreApi.Usecase.Interfaces;

namespace FoodstoreApi.Usecase.Services;

public class EInvoiceService(
    IEInvoiceRepository repository,
    IEInvoiceProviderFactory providerFactory,
    IOrderRepository orderRepository
) : IEInvoiceService
{
    private readonly IEInvoiceRepository _repository = repository;
    private readonly IEInvoiceProviderFactory _providerFactory = providerFactory;
    private readonly IOrderRepository _orderRepository = orderRepository;

    public async Task<List<EInvoiceProviderDto>> GetAllProvidersAsync(CancellationToken ct = default)
    {
        var providers = await _repository.GetAllProvidersAsync(ct);
        return providers.Select(p => new EInvoiceProviderDto(
            p.Id, p.Name, p.ProviderType, p.IsActive,
            JsonSerializer.Deserialize<object>(p.ConfigJson) ?? new { },
            p.Description, p.CreatedAt, p.UpdatedAt
        )).ToList();
    }

    public async Task<EInvoiceProviderDto?> GetProviderByIdAsync(Guid id, CancellationToken ct = default)
    {
        var p = await _repository.GetProviderByIdAsync(id, ct);
        if (p == null) return null;
        return new EInvoiceProviderDto(
            p.Id, p.Name, p.ProviderType, p.IsActive,
            JsonSerializer.Deserialize<object>(p.ConfigJson) ?? new { },
            p.Description, p.CreatedAt, p.UpdatedAt
        );
    }

    public async Task<EInvoiceProviderDto> CreateProviderAsync(CreateEInvoiceProviderDto dto, Guid userId, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var provider = new EInvoiceProvider
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            ProviderType = dto.ProviderType,
            IsActive = dto.IsActive,
            ConfigJson = JsonSerializer.Serialize(dto.Config),
            Description = dto.Description,
            CreatedAt = now,
            UpdatedAt = now,
            CreatedBy = userId,
            UpdatedBy = userId
        };
        var created = await _repository.AddProviderAsync(provider, ct);
        return new EInvoiceProviderDto(
            created.Id, created.Name, created.ProviderType, created.IsActive,
            JsonSerializer.Deserialize<object>(created.ConfigJson) ?? new { },
            created.Description, created.CreatedAt, created.UpdatedAt
        );
    }

    public async Task UpdateProviderAsync(Guid id, UpdateEInvoiceProviderDto dto, Guid userId, CancellationToken ct = default)
    {
        var existing = await _repository.GetProviderByIdAsync(id, ct)
            ?? throw new Exception("Provider not found");

        existing.Name = dto.Name;
        existing.ProviderType = dto.ProviderType;
        existing.IsActive = dto.IsActive;
        existing.ConfigJson = JsonSerializer.Serialize(dto.Config);
        existing.Description = dto.Description;
        existing.UpdatedAt = DateTime.UtcNow;
        existing.UpdatedBy = userId;

        await _repository.UpdateProviderAsync(existing, ct);
    }

    public async Task DeleteProviderAsync(Guid id, CancellationToken ct = default)
    {
        var existing = await _repository.GetProviderByIdAsync(id, ct)
            ?? throw new Exception("Provider not found");
        await _repository.DeleteProviderAsync(existing, ct);
    }

    public async Task<bool> TestProviderConnectionAsync(Guid id, CancellationToken ct = default)
    {
        var provider = await _repository.GetProviderByIdAsync(id, ct)
            ?? throw new Exception("Provider not found");

        var impl = _providerFactory.GetProvider(provider.ProviderType, provider.ConfigJson);
        if (impl == null) throw new Exception($"No implementation for provider type '{provider.ProviderType}'");

        return await impl.TestConnectionAsync(ct);
    }

    public async Task<List<EInvoiceDto>> GetAllInvoicesAsync(CancellationToken ct = default)
    {
        var invoices = await _repository.GetAllInvoicesAsync(ct);
        return invoices.Select(MapInvoice).ToList();
    }

    public async Task<EInvoiceDto?> GetInvoiceByIdAsync(Guid id, CancellationToken ct = default)
    {
        var invoice = await _repository.GetInvoiceByIdAsync(id, ct);
        return invoice == null ? null : MapInvoice(invoice);
    }

    public async Task<EInvoiceDto> IssueInvoiceAsync(Guid orderId, IssueInvoiceDto dto, Guid userId, CancellationToken ct = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, ct)
            ?? throw new Exception("Order not found");

        var existing = await _repository.GetInvoiceByOrderIdAsync(orderId, ct);
        if (existing != null)
            throw new Exception("Invoice already exists for this order");

        var settings = await _repository.GetSettingsAsync(ct);
        var providerId = dto.ProviderId ?? settings?.DefaultProviderId;
        EInvoiceProvider? provider = null;

        if (providerId.HasValue)
        {
            provider = await _repository.GetProviderByIdAsync(providerId.Value, ct);
            if (provider == null || !provider.IsActive)
                throw new Exception("Provider not found or inactive");
        }

        var now = DateTime.UtcNow;
        var invoice = new EInvoice
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            ProviderId = provider?.Id,
            Status = "draft",
            TotalAmount = order.TotalAmount ?? 0,
            VatAmount = order.VatAmount ?? 0,
            BuyerName = dto.BuyerName,
            BuyerTaxCode = dto.BuyerTaxCode,
            BuyerAddress = dto.BuyerAddress,
            CreatedAt = now,
            UpdatedAt = now,
            CreatedBy = userId,
            UpdatedBy = userId
        };

        // If we have a provider, try to issue
        if (provider != null)
        {
            var impl = _providerFactory.GetProvider(provider.ProviderType, provider.ConfigJson);
            if (impl != null)
            {
                var invoiceData = new
                {
                    OrderCode = order.OrderCode,
                    TotalAmount = order.TotalAmount,
                    VatAmount = order.VatAmount,
                    BuyerName = dto.BuyerName ?? order.Customer?.User?.Name,
                    BuyerTaxCode = dto.BuyerTaxCode,
                    BuyerAddress = dto.BuyerAddress,
                    TemplateCode = dto.TemplateCode ?? settings?.DefaultTemplateCode,
                    SerialNumber = dto.SerialNumber ?? settings?.DefaultSerialNumber
                };

                var result = await impl.CreateAndIssueInvoiceAsync(invoiceData, ct);
                invoice.Status = result.Success ? "issued" : "failed";
                invoice.InvoiceNumber = result.InvoiceNumber;
                invoice.PdfUrl = result.PdfUrl;
                invoice.XmlUrl = result.XmlUrl;
                invoice.ProviderResponse = result.RawResponse;
                invoice.ErrorMessage = result.ErrorMessage;
                if (result.Success) invoice.IssuedAt = now;
            }
        }

        invoice = await _repository.AddInvoiceAsync(invoice, ct);
        return MapInvoice(invoice);
    }

    public async Task<EInvoiceDto> CancelInvoiceAsync(Guid id, CancelInvoiceDto dto, CancellationToken ct = default)
    {
        var invoice = await _repository.GetInvoiceByIdAsync(id, ct)
            ?? throw new Exception("Invoice not found");

        if (invoice.Status != "issued")
            throw new Exception("Only issued invoices can be cancelled");

        if (invoice.ProviderId != null)
        {
            var provider = await _repository.GetProviderByIdAsync(invoice.ProviderId.Value, ct);
            if (provider != null)
            {
                var impl = _providerFactory.GetProvider(provider.ProviderType, provider.ConfigJson);
                if (impl != null)
                {
                    var result = await impl.CancelInvoiceAsync(
                        invoice.InvoiceNumber ?? "",
                        invoice.TemplateCode,
                        invoice.SerialNumber,
                        dto.Reason, ct);

                    if (!result.Success)
                        throw new Exception(result.ErrorMessage ?? "Failed to cancel invoice");
                }
            }
        }

        invoice.Status = "cancelled";
        invoice.CancelledAt = DateTime.UtcNow;
        invoice.CancelReason = dto.Reason;
        invoice.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateInvoiceAsync(invoice, ct);
        return MapInvoice(invoice);
    }

    public async Task<EInvoiceSettingDto?> GetSettingsAsync(CancellationToken ct = default)
    {
        var settings = await _repository.GetSettingsAsync(ct);
        if (settings == null) return null;
        return new EInvoiceSettingDto(
            settings.Id,
            settings.DefaultProviderId,
            settings.DefaultProvider?.Name,
            settings.AutoIssue,
            settings.DefaultTemplateCode,
            settings.DefaultSerialNumber,
            string.IsNullOrEmpty(settings.DigitalSignatureConfig)
                ? null
                : JsonSerializer.Deserialize<object>(settings.DigitalSignatureConfig),
            settings.UpdatedAt
        );
    }

    public async Task<EInvoiceSettingDto> UpdateSettingsAsync(UpdateEInvoiceSettingDto dto, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var settings = new EInvoiceSetting
        {
            Id = Guid.NewGuid(),
            DefaultProviderId = dto.DefaultProviderId,
            AutoIssue = dto.AutoIssue,
            DefaultTemplateCode = dto.DefaultTemplateCode,
            DefaultSerialNumber = dto.DefaultSerialNumber,
            DigitalSignatureConfig = dto.DigitalSignatureConfig != null
                ? JsonSerializer.Serialize(dto.DigitalSignatureConfig)
                : null,
            CreatedAt = now,
            UpdatedAt = now
        };

        var saved = await _repository.AddOrUpdateSettingsAsync(settings, ct);

        return new EInvoiceSettingDto(
            saved.Id,
            saved.DefaultProviderId,
            saved.DefaultProvider?.Name,
            saved.AutoIssue,
            saved.DefaultTemplateCode,
            saved.DefaultSerialNumber,
            string.IsNullOrEmpty(saved.DigitalSignatureConfig)
                ? null
                : JsonSerializer.Deserialize<object>(saved.DigitalSignatureConfig),
            saved.UpdatedAt
        );
    }

    public async Task<EInvoiceDashboardDto> GetDashboardAsync(CancellationToken ct = default)
    {
        var counts = await _repository.GetInvoiceCountsAsync(ct);
        var activeProviders = await _repository.GetActiveProviderCountAsync(ct);
        return new EInvoiceDashboardDto(
            counts.total, counts.draft, counts.issued,
            counts.failed, counts.cancelled, activeProviders
        );
    }

    private static EInvoiceDto MapInvoice(EInvoice i) => new(
        i.Id, i.OrderId, i.Order?.OrderCode, i.ProviderId, i.Provider?.Name,
        i.InvoiceNumber, i.TemplateCode, i.SerialNumber, i.Status,
        i.TotalAmount, i.VatAmount, i.BuyerName, i.BuyerTaxCode, i.BuyerAddress,
        i.PdfUrl, i.XmlUrl, i.ErrorMessage,
        i.IssuedAt, i.CancelledAt, i.CancelReason, i.CreatedAt
    );
}
