namespace FoodstoreApi.Usecase.DTOs.MenuItem;

using FoodstoreApi.Usecase.DTOs.Addon;

public record MenuItemDto(Guid Id, Guid? CategoryId, string Name, string? Description, decimal Price, string? ImageUrl, bool? IsActive, DateTime CreatedAt, DateTime UpdatedAt, Guid? CreatedBy, Guid? UpdatedBy, string? CategoryName, List<AddonDto>? Addons = null);
