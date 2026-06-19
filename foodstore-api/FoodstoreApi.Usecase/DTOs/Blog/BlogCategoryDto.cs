namespace FoodstoreApi.Usecase.DTOs.Blog;

public class BlogCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Description { get; set; }
    public Guid? ParentId { get; set; }
    public string? ParentName { get; set; }
    public int SortOrder { get; set; }
    public int PostCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateBlogCategoryDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public Guid? ParentId { get; set; }
    public int SortOrder { get; set; }
}

public class UpdateBlogCategoryDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Guid? ParentId { get; set; }
    public int? SortOrder { get; set; }
}
