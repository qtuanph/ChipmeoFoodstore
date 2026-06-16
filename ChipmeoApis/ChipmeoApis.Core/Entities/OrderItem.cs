namespace ChipmeoApis.Core.Entities;

public partial class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int? MenuItemId { get; set; }

    public int? ComboId { get; set; }

    public string MenuItemName { get; set; } = null!;

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public decimal TotalPrice { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Combo? Combo { get; set; }

    public virtual MenuItem? MenuItem { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual ICollection<OrderItemAddon> OrderItemAddons { get; set; } = new List<OrderItemAddon>();
}




