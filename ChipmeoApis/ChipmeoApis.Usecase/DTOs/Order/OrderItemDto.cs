namespace ChipmeoApis.Usecase.DTOs.Order;

public class OrderItemDto
{
    public int Id { get; set; }
    public int? MenuItemId { get; set; }
    public string MenuItemName { get; set; } = string.Empty;
    public int? ComboId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public string? Note { get; set; }
    
    public List<OrderItemAddonDto> Addons { get; set; } = new();
}

public class OrderItemAddonDto
{
    public int Id { get; set; }
    public int AddonId { get; set; }
    public string AddonName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}




