namespace FoodstoreApi.Usecase.DTOs.Tag;

public record TagDto(
    int Id,
    string Name,
    string Slug,
    string? Description,
    string Color,
    int PostCount
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




