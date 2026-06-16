namespace ChipmeoApis.Usecase.DTOs.Combo;

public class CreateComboDto
{
    public string Name { get; set; } = string.Empty;
    public decimal ComboPrice { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; } = true;
    
    public List<CreateComboItemDto> Items { get; set; } = new();
}

public class CreateComboItemDto
{
    public int MenuItemId { get; set; }
    public int Quantity { get; set; }
}




