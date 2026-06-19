namespace FoodstoreApi.Usecase.DTOs.Blog;

public class BlogPostBlockDto
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public string BlockType { get; set; } = null!;
    public string BlockData { get; set; } = "{}";
    public int SortOrder { get; set; }
}

public class CreateBlogBlockDto
{
    public string BlockType { get; set; } = null!;
    public string BlockData { get; set; } = "{}";
    public int SortOrder { get; set; }
}

public class UpdateBlogBlockDto
{
    public string? BlockType { get; set; }
    public string? BlockData { get; set; }
    public int? SortOrder { get; set; }
}
