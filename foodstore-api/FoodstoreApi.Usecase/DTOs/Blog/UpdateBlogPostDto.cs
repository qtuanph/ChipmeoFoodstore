namespace FoodstoreApi.Usecase.DTOs.Blog;

public class UpdateBlogPostDto
{
    public string? Title { get; set; }
    public string? Excerpt { get; set; }
    public string? Content { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? Status { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? FocusKeyword { get; set; }
    public string? Keywords { get; set; }
    public string? CanonicalUrl { get; set; }
    public string? OgImageUrl { get; set; }
    public int? SeoScore { get; set; }
    public DateTime? ScheduledAt { get; set; }
    public bool? IsFeatured { get; set; }
    public string? Template { get; set; }
    public bool? AllowComments { get; set; }

    public List<Guid>? TagIds { get; set; }
    public List<Guid>? CategoryIds { get; set; }
    public List<CreateBlogBlockDto>? Blocks { get; set; }
}
