using FoodstoreApi.Usecase.DTOs.Tag;

namespace FoodstoreApi.Usecase.DTOs.Blog;

public class BlogPostDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Excerpt { get; set; }
    public string? Content { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? Status { get; set; }
    public Guid? AuthorId { get; set; }
    public string? AuthorName { get; set; }
    public DateTime? PublishedAt { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? FocusKeyword { get; set; }
    public string? Keywords { get; set; }
    public string? CanonicalUrl { get; set; }
    public string? OgImageUrl { get; set; }
    public int? ReadingTime { get; set; }
    public int? WordCount { get; set; }
    public int? SeoScore { get; set; }
    public DateTime? ScheduledAt { get; set; }
    public Guid? ReviewedBy { get; set; }
    public string? ReviewedByName { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public bool IsFeatured { get; set; }
    public string? Template { get; set; }
    public int ViewCount { get; set; }
    public bool AllowComments { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }

    public List<TagDto>? Tags { get; set; }
    public List<BlogCategoryDto>? Categories { get; set; }
    public List<BlogPostBlockDto>? Blocks { get; set; }
}
