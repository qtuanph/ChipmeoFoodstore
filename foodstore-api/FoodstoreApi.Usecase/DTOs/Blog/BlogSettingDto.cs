namespace FoodstoreApi.Usecase.DTOs.Blog;

public class BlogSettingDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = null!;
    public string Value { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class UpdateBlogSettingDto
{
    public string Value { get; set; } = null!;
    public string? Description { get; set; }
}
