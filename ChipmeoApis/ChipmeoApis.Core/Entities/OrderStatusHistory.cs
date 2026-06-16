namespace ChipmeoApis.Core.Entities;

public partial class OrderStatusHistory
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public string? FromStatus { get; set; }

    public string ToStatus { get; set; } = null!;

    public int? ChangedBy { get; set; }

    public DateTime? ChangedAt { get; set; }

    public string? Note { get; set; }

    public virtual Employee? ChangedByNavigation { get; set; }

    public virtual Order Order { get; set; } = null!;
}




