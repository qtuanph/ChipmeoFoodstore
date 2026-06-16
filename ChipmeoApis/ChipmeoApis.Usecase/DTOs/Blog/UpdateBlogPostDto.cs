namespace ChipmeoApis.Usecase.DTOs.Blog;

public class UpdateBlogPostDto
{
    public string? Title { get; set; }
    public string? Excerpt { get; set; }
    public string? Content { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? Status { get; set; }
    // SEO Fields
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? FocusKeyword { get; set; }
    public string? Keywords { get; set; }
    public string? CanonicalUrl { get; set; }
    public string? OgImageUrl { get; set; }
    public int? SeoScore { get; set; }

    public List<int>? TagIds { get; set; }
}




