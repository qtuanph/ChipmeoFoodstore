namespace ChipmeoApis.Core.Entities;

public partial class Media
{
    public int Id { get; set; }

    public string FileName { get; set; } = null!;

    public string Folder { get; set; } = "misc";

    public string FileUrl { get; set; } = null!;

    public string FileType { get; set; } = null!;

    public long? FileSize { get; set; }

    public string? AltText { get; set; }

    public int? UploadedByEmployee { get; set; }

    public int? UploadedByCustomer { get; set; }

    public string? EntityType { get; set; }

    public int? EntityId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Employee? UploadedByEmployeeNavigation { get; set; }

    public virtual Customer? UploadedByCustomerNavigation { get; set; }
}




