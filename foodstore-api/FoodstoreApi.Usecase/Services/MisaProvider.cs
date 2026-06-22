using FoodstoreApi.Usecase.Interfaces;

namespace FoodstoreApi.Usecase.Services;

public class MisaProvider : IEInvoiceProvider
{
    public string ProviderType => "misa";

    public Task<IssueResult> CreateAndIssueInvoiceAsync(object invoiceData, CancellationToken ct = default)
    {
        // Stub — implement MISA meInvoice API calls here
        return Task.FromResult(new IssueResult(
            true,
            "INV-MOCK-001",
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
