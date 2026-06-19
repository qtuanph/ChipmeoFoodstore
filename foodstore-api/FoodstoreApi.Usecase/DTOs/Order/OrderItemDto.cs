namespace FoodstoreApi.Usecase.DTOs.Order;

public class OrderItemDto
{
    public Guid Id { get; set; }
    public Guid? MenuItemId { get; set; }
    public string MenuItemName { get; set; } = string.Empty;
    public Guid? ComboId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public List<OrderItemAddonDto> Addons { get; set; } = new();
}

public class OrderItemAddonDto
{
    public Guid Id { get; set; }
    public Guid AddonId { get; set; }
    public string AddonName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
