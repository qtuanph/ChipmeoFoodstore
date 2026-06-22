using FoodstoreApi.Usecase.Interfaces;

namespace FoodstoreApi.Usecase.Services;

public class ViettelProvider : IEInvoiceProvider
{
    public string ProviderType => "viettel";

    public Task<IssueResult> CreateAndIssueInvoiceAsync(object invoiceData, CancellationToken ct = default)
    {
        // Stub — implement Viettel S-Invoice API calls here
        return Task.FromResult(new IssueResult(
            true,
            "VTT-MOCK-001",
            "https://pdf.url",
            "https://xml.url",
            """{"mock": true}""",
            null
        ));
    }

    public Task<CheckStatusResult> CheckStatusAsync(string invoiceNumber, string? templateCode, CancellationToken ct = default)
    {
        return Task.FromResult(new CheckStatusResult(
            "issued",
            "https://pdf.url",
            "https://xml.url",
            """{"status": "issued"}""",
            null
        ));
    }

    public Task<IssueResult> CancelInvoiceAsync(string invoiceNumber, string? templateCode, string? serialNumber, string reason, CancellationToken ct = default)
    {
        return Task.FromResult(new IssueResult(
            true,
            invoiceNumber,
            null,
            null,
            """{"cancelled": true}""",
            null
        ));
    }

    public Task<bool> TestConnectionAsync(CancellationToken ct = default)
    {
        return Task.FromResult(true);
    }
}
