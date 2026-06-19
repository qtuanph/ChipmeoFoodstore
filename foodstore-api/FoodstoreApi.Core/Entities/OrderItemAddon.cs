namespace FoodstoreApi.Core.Entities;

public partial class OrderItemAddon : IAuditableEntity
{
    public Guid Id { get; set; }
    public Guid OrderItemId { get; set; }
    public Guid AddonId { get; set; }
    public string AddonName { get; set; } = null!;
    public int? Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }

    public virtual Addon Addon { get; set; } = null!;
    public virtual OrderItem OrderItem { get; set; } = null!;
}
