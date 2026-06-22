namespace FoodstoreApi.Core.Entities;

public partial class EInvoiceSetting
{
    public Guid Id { get; set; }
    public Guid? DefaultProviderId { get; set; }
    public bool AutoIssue { get; set; }
    public string? DefaultTemplateCode { get; set; }
    public string? DefaultSerialNumber { get; set; }
    public string? DigitalSignatureConfig { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public virtual EInvoiceProvider? DefaultProvider { get; set; }
}
