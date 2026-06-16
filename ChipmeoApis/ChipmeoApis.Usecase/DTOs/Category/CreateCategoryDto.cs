namespace ChipmeoApis.Usecase.DTOs.Category;

public record CreateCategoryDto(string Name, string? Description = null, string? ImageUrl = null, bool? IsActive = true);




