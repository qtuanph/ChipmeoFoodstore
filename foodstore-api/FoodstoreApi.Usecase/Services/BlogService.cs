using FoodstoreApi.Usecase.Interfaces;
using FoodstoreApi.Usecase.DTOs.Blog;
using FoodstoreApi.Core.Entities;
using FoodstoreApi.Usecase.DTOs.Tag;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace FoodstoreApi.Usecase.Services;

public class BlogService : IBlogService
{
    private readonly IBlogRepository _repo;
    private readonly IMediaService _mediaService;
    private readonly IBlogCategoryRepository _categoryRepo;
    private readonly IBlogRevisionRepository _revisionRepo;
    private readonly IBlogBlockRepository _blockRepo;

    public BlogService(IBlogRepository repo, IMediaService mediaService,
        IBlogCategoryRepository categoryRepo, IBlogRevisionRepository revisionRepo,
        IBlogBlockRepository blockRepo)
    {
        _repo = repo;
        _mediaService = mediaService;
        _categoryRepo = categoryRepo;
        _revisionRepo = revisionRepo;
        _blockRepo = blockRepo;
    }

    public async Task<List<BlogPostDto>> GetAllPostsAsync(string? status = null, string? categorySlug = null, string? tagSlug = null)
    {
        var posts = await _repo.GetAllAsync(status, categorySlug, tagSlug);
        return posts.Select(p => MapToDto(p)).ToList();
    }

    public async Task<BlogPostDto?> GetPostBySlugAsync(string slug)
    {
        var post = await _repo.GetBySlugAsync(slug);
        return post == null ? null : MapToDto(post);
    }

    public async Task<BlogPostDto?> GetPostByIdAsync(Guid id)
    {
        var post = await _repo.GetByIdAsync(id);
        return post == null ? null : MapToDto(post);
    }

    public async Task<BlogPostDto> CreatePostAsync(CreateBlogPostDto dto, Guid authorId)
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
            MetaTitle = dto.MetaTitle,
            MetaDescription = dto.MetaDescription,
            FocusKeyword = dto.FocusKeyword,
            Keywords = dto.Keywords,
            CanonicalUrl = dto.CanonicalUrl,
            OgImageUrl = dto.OgImageUrl ?? dto.ThumbnailUrl,
            WordCount = wordCount,
            ReadingTime = readingTime,
            SeoScore = dto.SeoScore ?? 0,
            ScheduledAt = dto.ScheduledAt,
            IsFeatured = dto.IsFeatured,
            Template = dto.Template,
            AllowComments = dto.AllowComments,
        };

        if (dto.TagIds != null)
            foreach (var tagId in dto.TagIds)
                post.BlogPostTags.Add(new BlogPostTag { TagId = tagId });

        if (dto.CategoryIds != null)
            foreach (var catId in dto.CategoryIds)
                post.BlogPostCategories.Add(new BlogPostCategory { CategoryId = catId });

        if (post.Status == "published" && post.ScheduledAt == null)
            post.PublishedAt = DateTime.UtcNow;

        if (dto.Blocks != null)
        {
            post.BlogPostBlocks = dto.Blocks.Select((b, i) => new BlogPostBlock
            {
                BlockType = b.BlockType,
                BlockData = b.BlockData,
                SortOrder = b.SortOrder != 0 ? b.SortOrder : i,
            }).ToList();
        }

        var created = await _repo.AddAsync(post);

        if (!string.IsNullOrEmpty(dto.ThumbnailUrl))
            await _mediaService.LinkMediaToEntityAsync(dto.ThumbnailUrl, "blog_post", created.Id);
        if (!string.IsNullOrEmpty(dto.OgImageUrl) && dto.OgImageUrl != dto.ThumbnailUrl)
            await _mediaService.LinkMediaToEntityAsync(dto.OgImageUrl, "blog_post", created.Id);
        await _mediaService.LinkMediaFromContentAsync(dto.Content, "blog_post", created.Id);

        var loaded = await _repo.GetByIdAsync(created.Id);
        return MapToDto(loaded ?? created);
    }

    public async Task<BlogPostDto?> UpdatePostAsync(Guid id, UpdateBlogPostDto dto)
    {
        var post = await _repo.GetByIdAsync(id);
        if (post == null) return null;

        var oldThumbnailUrl = post.ThumbnailUrl;
        var oldOgImageUrl = post.OgImageUrl;

        if (dto.Title != null) { post.Title = dto.Title; }
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
                post.PublishedAt = DateTime.UtcNow;
        }
        if (dto.MetaTitle != null) post.MetaTitle = dto.MetaTitle;
        if (dto.MetaDescription != null) post.MetaDescription = dto.MetaDescription;
        if (dto.FocusKeyword != null) post.FocusKeyword = dto.FocusKeyword;
        if (dto.Keywords != null) post.Keywords = dto.Keywords;
        if (dto.CanonicalUrl != null) post.CanonicalUrl = dto.CanonicalUrl;
        if (dto.OgImageUrl != null) post.OgImageUrl = dto.OgImageUrl;
        if (dto.SeoScore.HasValue) post.SeoScore = dto.SeoScore;
        if (dto.ScheduledAt != null) post.ScheduledAt = dto.ScheduledAt;
        if (dto.IsFeatured.HasValue) post.IsFeatured = dto.IsFeatured.Value;
        if (dto.Template != null) post.Template = dto.Template;
        if (dto.AllowComments.HasValue) post.AllowComments = dto.AllowComments.Value;

        if (dto.TagIds != null)
        {
            post.BlogPostTags.Clear();
            foreach (var tagId in dto.TagIds)
                post.BlogPostTags.Add(new BlogPostTag { TagId = tagId });
        }

        if (dto.CategoryIds != null)
        {
            post.BlogPostCategories.Clear();
            foreach (var catId in dto.CategoryIds)
                post.BlogPostCategories.Add(new BlogPostCategory { CategoryId = catId });
        }

        if (dto.Blocks != null)
        {
            post.BlogPostBlocks = dto.Blocks.Select((b, i) => new BlogPostBlock
            {
                PostId = id,
                BlockType = b.BlockType,
                BlockData = b.BlockData,
                SortOrder = b.SortOrder != 0 ? b.SortOrder : i,
            }).ToList();
        }

        await _repo.UpdateAsync(post);

        if (!string.IsNullOrEmpty(dto.ThumbnailUrl) && dto.ThumbnailUrl != oldThumbnailUrl)
            await _mediaService.LinkMediaToEntityAsync(dto.ThumbnailUrl, "blog_post", id);
        if (!string.IsNullOrEmpty(dto.OgImageUrl) && dto.OgImageUrl != oldOgImageUrl)
            await _mediaService.LinkMediaToEntityAsync(dto.OgImageUrl, "blog_post", id);
        if (!string.IsNullOrEmpty(dto.Content))
            await _mediaService.LinkMediaFromContentAsync(dto.Content, "blog_post", id);

        var loaded = await _repo.GetByIdAsync(post.Id);
        return MapToDto(loaded ?? post);
    }

    public async Task<bool> DeletePostAsync(Guid id)
    {
        var post = await _repo.GetByIdAsync(id);
        if (post == null) return false;
        await _repo.DeleteAsync(post);
        await _mediaService.DeleteMediaByEntityAsync("blog_post", id);
        return true;
    }

    public async Task<BlogPostDto?> ChangeStatusAsync(Guid id, string status, Guid userId)
    {
        var post = await _repo.GetByIdAsync(id);
        if (post == null) return null;

        var oldStatus = post.Status;
        post.Status = status;

        if (status == "published" && post.PublishedAt == null)
            post.PublishedAt = DateTime.UtcNow;
        if (status == "reviewed")
        {
            post.ReviewedBy = userId;
            post.ReviewedAt = DateTime.UtcNow;
        }
        if (status == "draft" && oldStatus == "published")
        {
            post.PublishedAt = null;
            post.ReviewedBy = null;
            post.ReviewedAt = null;
        }

        await _repo.UpdateAsync(post);

        await _revisionRepo.CreateAsync(new BlogPostRevision
        {
            PostId = id,
            Title = post.Title,
            Content = post.Content,
            Excerpt = post.Excerpt,
            Status = status,
            MetaTitle = post.MetaTitle,
            MetaDescription = post.MetaDescription,
            WordCount = post.WordCount,
            ReadingTime = post.ReadingTime,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow,
        });

        var loaded = await _repo.GetByIdAsync(id);
        return MapToDto(loaded ?? post);
    }

    public async Task<bool> SchedulePostAsync(Guid id, DateTime? scheduledAt)
    {
        var post = await _repo.GetByIdAsync(id);
        if (post == null) return false;
        post.ScheduledAt = scheduledAt;
        if (scheduledAt != null) post.Status = "scheduled";
        await _repo.UpdateAsync(post);
        return true;
    }

    public async Task<BlogPostDto?> IncrementViewCountAsync(string slug)
    {
        var post = await _repo.GetBySlugAsync(slug);
        if (post == null) return null;
        post.ViewCount++;
        await _repo.UpdateAsync(post);
        return MapToDto(post);
    }

    public async Task<CmsDashboardStatsDto> GetDashboardStatsAsync()
    {
        var allPosts = (await _repo.GetAllAsync()).ToList();
        return new CmsDashboardStatsDto
        {
            TotalPosts = allPosts.Count,
            PublishedPosts = allPosts.Count(p => p.Status == "published"),
            DraftPosts = allPosts.Count(p => p.Status == "draft"),
            ScheduledPosts = allPosts.Count(p => p.Status == "scheduled"),
            FeaturedPostsCount = allPosts.Count(p => p.IsFeatured),
            TotalViews = allPosts.Sum(p => p.ViewCount),
            RecentPostsCount = allPosts.Count(p => p.CreatedAt >= DateTime.UtcNow.AddDays(-7)),
        };
    }

    public async Task<List<BlogPostDto>> GetFeaturedPostsAsync(int limit = 5)
    {
        var posts = await _repo.GetFeaturedAsync(limit);
        return posts.Select(p => MapToDto(p)).ToList();
    }

    public async Task<List<BlogPostDto>> GetPublishedPostsAsync(int page = 1, int limit = 10, string? categorySlug = null, string? tagSlug = null)
    {
        var posts = await _repo.GetPublishedAsync(page, limit, categorySlug, tagSlug);
        return posts.Select(p => MapToDto(p)).ToList();
    }

    public async Task<int> GetTotalPublishedCountAsync(string? categorySlug = null, string? tagSlug = null)
    {
        return await _repo.GetPublishedCountAsync(categorySlug, tagSlug);
    }

    public static BlogPostDto MapToDto(BlogPost post)
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
            AuthorName = post.Author?.User?.Name,
            PublishedAt = post.PublishedAt,
            MetaTitle = post.MetaTitle,
            MetaDescription = post.MetaDescription,
            FocusKeyword = post.FocusKeyword,
            Keywords = post.Keywords,
            CanonicalUrl = post.CanonicalUrl,
            OgImageUrl = post.OgImageUrl,
            ReadingTime = post.ReadingTime,
            WordCount = post.WordCount,
            SeoScore = post.SeoScore,
            ScheduledAt = post.ScheduledAt,
            ReviewedBy = post.ReviewedBy,
            ReviewedByName = post.ReviewedByNavigation?.User?.Name,
            ReviewedAt = post.ReviewedAt,
            IsFeatured = post.IsFeatured,
            Template = post.Template,
            ViewCount = post.ViewCount,
            AllowComments = post.AllowComments,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt,
            CreatedBy = post.CreatedBy,
            UpdatedBy = post.UpdatedBy,
            Tags = post.BlogPostTags.Select(pt => new TagDto(
                pt.Tag.Id, pt.Tag.Name, pt.Tag.Slug,
                pt.Tag.Description, pt.Tag.Color, 0,
                pt.Tag.CreatedAt, pt.Tag.UpdatedAt
            )).ToList(),
            Categories = post.BlogPostCategories.Select(pc => new BlogCategoryDto
            {
                Id = pc.Category.Id,
                Name = pc.Category.Name,
                Slug = pc.Category.Slug,
                Description = pc.Category.Description,
                ParentId = pc.Category.ParentId,
                SortOrder = pc.Category.SortOrder,
                CreatedAt = pc.Category.CreatedAt,
            }).ToList(),
            Blocks = post.BlogPostBlocks.OrderBy(b => b.SortOrder).Select(b => new BlogPostBlockDto
            {
                Id = b.Id,
                PostId = b.PostId,
                BlockType = b.BlockType,
                BlockData = b.BlockData,
                SortOrder = b.SortOrder,
            }).ToList(),
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
        var textOnly = Regex.Replace(content, "<[^>]+>", " ");
        return textOnly.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
    }

    private static int CalculateReadingTime(int wordCount)
    {
        return Math.Max(1, (int)Math.Ceiling(wordCount / 200.0));
    }
}
