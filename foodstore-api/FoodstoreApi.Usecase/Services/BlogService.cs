using FoodstoreApi.Usecase.Interfaces;
using FoodstoreApi.Usecase.DTOs;
using FoodstoreApi.Usecase.DTOs.Blog;
using FoodstoreApi.Core.Entities;
using FoodstoreApi.Usecase.DTOs.Tag;
using System.Text.RegularExpressions;

namespace FoodstoreApi.Usecase.Services;

public class BlogService : IBlogService
{
    private readonly IBlogRepository _repo;
    private readonly IMediaService _mediaService;

    public BlogService(IBlogRepository repo, IMediaService mediaService)
    {
        _repo = repo;
        _mediaService = mediaService;
    }

    public async Task<List<BlogPostDto>> GetAllPostsAsync(string? status = null)
    {
        var posts = await _repo.GetAllAsync(status);
        return posts.Select(p => MapToDto(p)).ToList();
    }

    public async Task<BlogPostDto?> GetPostBySlugAsync(string slug)
    {
        var post = await _repo.GetBySlugAsync(slug);
        return post == null ? null : MapToDto(post);
    }

    public async Task<BlogPostDto?> GetPostByIdAsync(int id)
    {
        var post = await _repo.GetByIdAsync(id);
        return post == null ? null : MapToDto(post);
    }

    public async Task<BlogPostDto> CreatePostAsync(CreateBlogPostDto dto, int authorId)
    {
        var wordCount = CalculateWordCount(dto.Content);
        var readingTime = CalculateReadingTime(wordCount);

        var post = new BlogPost
        {
            Title = dto.Title,
            Slug = GenerateSlug(dto.Title),
            Excerpt = dto.Excerpt,
            Content = dto.Content,
            ThumbnailUrl = dto.ThumbnailUrl,
            Status = dto.Status ?? "draft",
            AuthorId = authorId,
            // SEO Fields
            MetaTitle = dto.MetaTitle,
            MetaDescription = dto.MetaDescription,
            FocusKeyword = dto.FocusKeyword,
            Keywords = dto.Keywords,
            CanonicalUrl = dto.CanonicalUrl,
            OgImageUrl = dto.OgImageUrl ?? dto.ThumbnailUrl,
            WordCount = wordCount,
            ReadingTime = readingTime,
            SeoScore = dto.SeoScore ?? 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        if (dto.TagIds != null && dto.TagIds.Any())
        {
            foreach (var tagId in dto.TagIds)
            {
                post.BlogPostTags.Add(new BlogPostTag { TagId = tagId });
            }
        }

        if (post.Status == "published")
        {
            post.PublishedAt = DateTime.UtcNow;
        }

        var created = await _repo.AddAsync(post);
        
        // Link media
        if (!string.IsNullOrEmpty(dto.ThumbnailUrl))
            await _mediaService.LinkMediaToEntityAsync(dto.ThumbnailUrl, "blog_post", created.Id);
            
        if (!string.IsNullOrEmpty(dto.OgImageUrl) && dto.OgImageUrl != dto.ThumbnailUrl)
            await _mediaService.LinkMediaToEntityAsync(dto.OgImageUrl, "blog_post", created.Id);
            
        await _mediaService.LinkMediaFromContentAsync(dto.Content, "blog_post", created.Id);

        var loaded = await _repo.GetByIdAsync(created.Id);
        return MapToDto(loaded ?? created);
    }

    public async Task<BlogPostDto?> UpdatePostAsync(int id, UpdateBlogPostDto dto)
    {
        var post = await _repo.GetByIdAsync(id);
        if (post == null) return null;

        var oldThumbnailUrl = post.ThumbnailUrl;
        var oldOgImageUrl = post.OgImageUrl;

        if (dto.Title != null)
        {
            post.Title = dto.Title;
        }
        if (dto.Excerpt != null) post.Excerpt = dto.Excerpt;
        if (dto.Content != null)
        {
            post.Content = dto.Content;
            post.WordCount = CalculateWordCount(dto.Content);
            post.ReadingTime = CalculateReadingTime(post.WordCount ?? 0);
        }
        if (dto.ThumbnailUrl != null) post.ThumbnailUrl = dto.ThumbnailUrl;
        if (dto.Status != null)
        {
            post.Status = dto.Status;
            if (post.Status == "published" && post.PublishedAt == null)
            {
                post.PublishedAt = DateTime.UtcNow;
            }
        }
        // SEO Fields
        if (dto.MetaTitle != null) post.MetaTitle = dto.MetaTitle;
        if (dto.MetaDescription != null) post.MetaDescription = dto.MetaDescription;
        if (dto.FocusKeyword != null) post.FocusKeyword = dto.FocusKeyword;
        if (dto.Keywords != null) post.Keywords = dto.Keywords;
        if (dto.CanonicalUrl != null) post.CanonicalUrl = dto.CanonicalUrl;
        if (dto.OgImageUrl != null) post.OgImageUrl = dto.OgImageUrl;
        if (dto.SeoScore.HasValue) post.SeoScore = dto.SeoScore;

        if (dto.TagIds != null)
        {
            post.BlogPostTags.Clear();
            foreach (var tagId in dto.TagIds)
            {
                post.BlogPostTags.Add(new BlogPostTag { TagId = tagId });
            }
        }

        post.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(post);

        // Update media links
        if (!string.IsNullOrEmpty(dto.ThumbnailUrl) && dto.ThumbnailUrl != oldThumbnailUrl)
            await _mediaService.LinkMediaToEntityAsync(dto.ThumbnailUrl, "blog_post", id);

        if (!string.IsNullOrEmpty(dto.OgImageUrl) && dto.OgImageUrl != oldOgImageUrl)
            await _mediaService.LinkMediaToEntityAsync(dto.OgImageUrl, "blog_post", id);
            
        if (!string.IsNullOrEmpty(dto.Content))
            await _mediaService.LinkMediaFromContentAsync(dto.Content, "blog_post", id);

        var loaded = await _repo.GetByIdAsync(post.Id);
        return MapToDto(loaded ?? post);
    }

    public async Task<bool> DeletePostAsync(int id)
    {
        var post = await _repo.GetByIdAsync(id);
        if (post == null) return false;

        await _repo.DeleteAsync(post);
        await _mediaService.DeleteMediaByEntityAsync("blog_post", id);
        return true;
    }

    private static BlogPostDto MapToDto(BlogPost post)
    {
        return new BlogPostDto
        {
            Id = post.Id,
            Title = post.Title,
            Slug = post.Slug,
            Excerpt = post.Excerpt,
            Content = post.Content,
            ThumbnailUrl = post.ThumbnailUrl,
            Status = post.Status,
            AuthorId = post.AuthorId,
            AuthorName = post.Author?.FullName,
            PublishedAt = post.PublishedAt,
            // SEO Fields
            MetaTitle = post.MetaTitle,
            MetaDescription = post.MetaDescription,
            FocusKeyword = post.FocusKeyword,
            Keywords = post.Keywords,
            CanonicalUrl = post.CanonicalUrl,
            OgImageUrl = post.OgImageUrl,
            ReadingTime = post.ReadingTime,
            WordCount = post.WordCount,
            SeoScore = post.SeoScore,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt,
            Tags = post.BlogPostTags.Select(pt => new TagDto(
                pt.Tag.Id,
                pt.Tag.Name,
                pt.Tag.Slug,
                pt.Tag.Description,
                pt.Tag.Color,
                0 // PostCount placeholder
            )).ToList()
        };
    }

    private static string GenerateSlug(string title)
    {
        return title.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("đ", "d")
            .Replace("á", "a").Replace("à", "a").Replace("ả", "a").Replace("ã", "a").Replace("ạ", "a")
            + "-" + Guid.NewGuid().ToString().Substring(0, 6);
    }

    private static int CalculateWordCount(string? content)
    {
        if (string.IsNullOrWhiteSpace(content)) return 0;
        // Strip HTML tags
        var textOnly = Regex.Replace(content, "<[^>]+>", " ");
        // Split by whitespace and count
        var words = textOnly.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        return words.Length;
    }

    private static int CalculateReadingTime(int wordCount)
    {
        // Average reading speed: 200 words per minute
        var minutes = (int)Math.Ceiling(wordCount / 200.0);
        return Math.Max(1, minutes); // Minimum 1 minute
    }
}





