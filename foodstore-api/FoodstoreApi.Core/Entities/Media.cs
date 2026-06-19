namespace FoodstoreApi.Core.Entities;

public partial class Media : IAuditableEntity
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = null!;
    public string Folder { get; set; } = "misc";
    public string FileUrl { get; set; } = null!;
    public string FileType { get; set; } = null!;
    public long? FileSize { get; set; }
    public string? AltText { get; set; }
    public Guid? UploadedByEmployee { get; set; }
    public Guid? UploadedByCustomer { get; set; }
    public string? EntityType { get; set; }
    public Guid? EntityId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }

    public virtual Employee? UploadedByEmployeeNavigation { get; set; }
    public virtual Customer? UploadedByCustomerNavigation { get; set; }
}
