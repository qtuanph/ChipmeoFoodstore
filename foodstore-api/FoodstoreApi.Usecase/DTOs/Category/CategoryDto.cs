namespace FoodstoreApi.Usecase.DTOs.Category;

public record CategoryDto(Guid Id, string Name, string? Description, string? ImageUrl, bool? IsActive, DateTime CreatedAt, DateTime UpdatedAt, Guid? CreatedBy, Guid? UpdatedBy);
