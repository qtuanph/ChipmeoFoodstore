namespace FoodstoreApi.Usecase.Interfaces;

public record IssueResult(
    bool Success,
    string? InvoiceNumber,
    string? PdfUrl,
    string? XmlUrl,
    string? RawResponse,
    string? ErrorMessage
);

public record CheckStatusResult(
    string Status,
    string? PdfUrl,
    string? XmlUrl,
    string? RawResponse,
    string? ErrorMessage
);

public interface IEInvoiceProvider
{
    string ProviderType { get; }
    Task<IssueResult> CreateAndIssueInvoiceAsync(object invoiceData, CancellationToken ct = default);
    Task<CheckStatusResult> CheckStatusAsync(string invoiceNumber, string? templateCode, CancellationToken ct = default);
    Task<IssueResult> CancelInvoiceAsync(string invoiceNumber, string? templateCode, string? serialNumber, string reason, CancellationToken ct = default);
    Task<bool> TestConnectionAsync(CancellationToken ct = default);
}
