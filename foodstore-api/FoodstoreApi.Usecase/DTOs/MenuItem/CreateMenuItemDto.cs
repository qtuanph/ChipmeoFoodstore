namespace FoodstoreApi.Usecase.DTOs.MenuItem;

public record CreateMenuItemDto(Guid? CategoryId, string Name, decimal Price, string? ImageUrl = null, bool? IsActive = true, List<Guid>? AddonIds = null);
