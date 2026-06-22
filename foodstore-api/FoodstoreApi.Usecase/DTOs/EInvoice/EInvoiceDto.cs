namespace FoodstoreApi.Usecase.DTOs.EInvoice;

public record EInvoiceDto(
    Guid Id,
    Guid OrderId,
    string? OrderCode,
    Guid? ProviderId,
    string? ProviderName,
    string? InvoiceNumber,
    string? TemplateCode,
    string? SerialNumber,
    string Status,
    decimal TotalAmount,
    decimal VatAmount,
    string? BuyerName,
    string? BuyerTaxCode,
    string? BuyerAddress,
    string? PdfUrl,
    string? XmlUrl,
    string? ErrorMessage,
    DateTime? IssuedAt,
    DateTime? CancelledAt,
    string? CancelReason,
    DateTime CreatedAt
);

public record IssueInvoiceDto(
    Guid? ProviderId,
    string? TemplateCode,
    string? SerialNumber,
    string? BuyerName,
    string? BuyerTaxCode,
    string? BuyerAddress
);

public record CancelInvoiceDto(
    string Reason
);
