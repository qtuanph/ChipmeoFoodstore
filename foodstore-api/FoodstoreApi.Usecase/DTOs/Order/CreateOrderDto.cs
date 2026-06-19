namespace FoodstoreApi.Usecase.DTOs.Order;

public class CreateOrderDto
{
    public Guid? SourceId { get; set; }
    public Guid? CustomerId { get; set; }
    public string? Note { get; set; }
    public string? DiscountCode { get; set; }
    
    public List<CreateOrderItemDto> Items { get; set; } = new();
}

public class CreateOrderItemDto
{
    public Guid? MenuItemId { get; set; }
    public Guid? ComboId { get; set; }
    public int Quantity { get; set; }
    public string? Note { get; set; }
    
    public List<CreateOrderItemAddonDto> Addons { get; set; } = new();
}

public class CreateOrderItemAddonDto
{
    public Guid AddonId { get; set; }
    public int Quantity { get; set; }
}
