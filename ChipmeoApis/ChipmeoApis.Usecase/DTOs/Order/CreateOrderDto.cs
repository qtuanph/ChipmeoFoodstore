namespace ChipmeoApis.Usecase.DTOs.Order;

public class CreateOrderDto
{
    public int? SourceId { get; set; }
    public int? CustomerId { get; set; }
    public string? Note { get; set; }
    public string? DiscountCode { get; set; }
    
    public List<CreateOrderItemDto> Items { get; set; } = new();
}

public class CreateOrderItemDto
{
    public int? MenuItemId { get; set; }
    public int? ComboId { get; set; }
    public int Quantity { get; set; }
    public string? Note { get; set; }
    
    public List<CreateOrderItemAddonDto> Addons { get; set; } = new();
}

public class CreateOrderItemAddonDto
{
    public int AddonId { get; set; }
    public int Quantity { get; set; }
}




