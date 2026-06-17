namespace FoodstoreApi.Usecase.DTOs.Addon;

public class CreateAddonDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;
}




