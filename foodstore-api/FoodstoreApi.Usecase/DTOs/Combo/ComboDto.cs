namespace FoodstoreApi.Usecase.DTOs.Combo;

public class ComboDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal ComboPrice { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    
    public List<ComboItemDto> Items { get; set; } = new();
}

public class ComboItemDto
{
    public Guid Id { get; set; }
    public Guid MenuItemId { get; set; }
    public string MenuItemName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
