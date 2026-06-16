namespace ChipmeoApis.Usecase.DTOs.MenuItem;

using ChipmeoApis.Usecase.DTOs.Addon;

public record MenuItemDto(int Id, int? CategoryId, string Name, string? Description, decimal Price, string? ImageUrl, bool? IsActive, DateTime? CreatedAt, string? CategoryName, List<AddonDto>? Addons = null);




