namespace FoodstoreApi.Usecase.DTOs.Combo;

public class ComboDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal ComboPrice { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public List<ComboItemDto> Items { get; set; } = new();
}

public class ComboItemDto
{
    public int Id { get; set; }
    public int MenuItemId { get; set; }
    public string MenuItemName { get; set; } = string.Empty;
    public int Quantity { get; set; }
}




