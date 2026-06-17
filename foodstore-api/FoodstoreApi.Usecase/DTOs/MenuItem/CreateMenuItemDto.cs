namespace FoodstoreApi.Usecase.DTOs.MenuItem;

public record CreateMenuItemDto(int? CategoryId, string Name, decimal Price, string? ImageUrl = null, bool? IsActive = true, List<int>? AddonIds = null);




