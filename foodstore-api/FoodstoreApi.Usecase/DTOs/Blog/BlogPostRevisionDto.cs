namespace FoodstoreApi.Usecase.DTOs.Blog;

public class BlogPostRevisionDto
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public string Title { get; set; } = null!;
    public string? Content { get; set; }
    public string? Excerpt { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? Status { get; set; }
    public string? BlocksJson { get; set; }
    public Guid? CreatedBy { get; set; }
    public string? CreatedByName { get; set; }
    public DateTime CreatedAt { get; set; }
}
