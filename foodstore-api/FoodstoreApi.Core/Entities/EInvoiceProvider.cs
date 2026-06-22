namespace FoodstoreApi.Core.Entities;

public partial class EInvoiceProvider : IAuditableEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string ProviderType { get; set; } = null!;
    public bool IsActive { get; set; } = true;
    public string ConfigJson { get; set; } = "{}";
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
}
