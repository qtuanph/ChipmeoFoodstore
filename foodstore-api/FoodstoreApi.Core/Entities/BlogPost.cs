namespace FoodstoreApi.Core.Entities;

public partial class BlogPost : IAuditableEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Excerpt { get; set; }
    public string? Content { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? Status { get; set; }
    public Guid? AuthorId { get; set; }
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
    public DateTime? ReviewedAt { get; set; }
    public bool IsFeatured { get; set; }
    public string? Template { get; set; }
    public int ViewCount { get; set; }
    public bool AllowComments { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }

    public virtual Employee? Author { get; set; }
    public virtual Employee? ReviewedByNavigation { get; set; }
    public virtual ICollection<BlogPostTag> BlogPostTags { get; set; } = new List<BlogPostTag>();
    public virtual ICollection<BlogPostCategory> BlogPostCategories { get; set; } = new List<BlogPostCategory>();
    public virtual ICollection<BlogPostRevision> BlogPostRevisions { get; set; } = new List<BlogPostRevision>();
    public virtual ICollection<BlogPostBlock> BlogPostBlocks { get; set; } = new List<BlogPostBlock>();
}
