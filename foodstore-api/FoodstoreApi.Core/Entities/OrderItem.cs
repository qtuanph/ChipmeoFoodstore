namespace FoodstoreApi.Core.Entities;

public partial class OrderItem : IAuditableEntity
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid? MenuItemId { get; set; }
    public Guid? ComboId { get; set; }
    public string MenuItemName { get; set; } = null!;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }

    public virtual Combo? Combo { get; set; }
    public virtual MenuItem? MenuItem { get; set; }
    public virtual Order Order { get; set; } = null!;
    public virtual ICollection<OrderItemAddon> OrderItemAddons { get; set; } = new List<OrderItemAddon>();
}
