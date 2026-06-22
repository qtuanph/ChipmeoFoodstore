namespace FoodstoreApi.Core.Entities;

public partial class EInvoice : IAuditableEntity
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid? ProviderId { get; set; }
    public string? InvoiceNumber { get; set; }
    public string? TemplateCode { get; set; }
    public string? SerialNumber { get; set; }
    public string Status { get; set; } = "draft";
    public decimal TotalAmount { get; set; }
    public decimal VatAmount { get; set; }
    public string? BuyerName { get; set; }
    public string? BuyerTaxCode { get; set; }
    public string? BuyerAddress { get; set; }
    public string? PdfUrl { get; set; }
    public string? XmlUrl { get; set; }
    public string? ProviderResponse { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime? IssuedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancelReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }

    public virtual Order Order { get; set; } = null!;
    public virtual EInvoiceProvider? Provider { get; set; }
}
