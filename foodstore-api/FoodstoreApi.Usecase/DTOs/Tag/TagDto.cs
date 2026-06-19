namespace FoodstoreApi.Usecase.DTOs.Tag;

public record TagDto(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    string Color,
    int PostCount,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record CreateTagDto(
    string Name,
    string? Description,
    string? Color
);

public record UpdateTagDto(
    string? Name,
    string? Description,
    string? Color
);
