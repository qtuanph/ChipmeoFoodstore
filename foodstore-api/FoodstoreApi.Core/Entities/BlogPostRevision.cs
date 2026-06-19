namespace FoodstoreApi.Core.Entities;

public class BlogPostRevision
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public string Title { get; set; } = null!;
    public string? Content { get; set; }
    public string? Excerpt { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? Status { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? FocusKeyword { get; set; }
    public string? Keywords { get; set; }
    public string? CanonicalUrl { get; set; }
    public string? OgImageUrl { get; set; }
    public int? WordCount { get; set; }
    public int? ReadingTime { get; set; }
    public string? BlocksJson { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual BlogPost Post { get; set; } = null!;
}
