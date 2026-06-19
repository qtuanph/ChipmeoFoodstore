namespace FoodstoreApi.Core.Entities;

public partial class OrderStatusHistory
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string? FromStatus { get; set; }
    public string ToStatus { get; set; } = null!;
    public Guid? ChangedBy { get; set; }
    public DateTime? ChangedAt { get; set; }
    public string? Note { get; set; }

    public virtual Employee? ChangedByNavigation { get; set; }
    public virtual Order Order { get; set; } = null!;
}
