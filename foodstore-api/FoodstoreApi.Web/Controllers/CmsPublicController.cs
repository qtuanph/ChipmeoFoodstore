using FoodstoreApi.Usecase.Interfaces;
using FoodstoreApi.Usecase.Services;
using Microsoft.AspNetCore.Mvc;
using FoodstoreApi.Web.ApiResponse;

namespace FoodstoreApi.Web.Controllers;

[Route("api/cms")]
[ApiController]
public class CmsPublicController : ControllerBase
{
    private readonly IBlogService _blogService;
    private readonly IBlogCategoryService _categoryService;
    private readonly ITagService _tagService;

    public CmsPublicController(
        IBlogService blogService,
        IBlogCategoryService categoryService,
        ITagService tagService)
    {
        _blogService = blogService;
        _categoryService = categoryService;
        _tagService = tagService;
    }

    [HttpGet("articles")]
    public async Task<IActionResult> GetArticles(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10,
        [FromQuery] string? category = null,
        [FromQuery] string? tag = null)
    {
        var posts = await _blogService.GetPublishedPostsAsync(page, limit, category, tag);
        var total = await _blogService.GetTotalPublishedCountAsync(category, tag);
        return ApiResult.Success(new
        {
            data = posts,
            pagination = new { page, limit, total, totalPages = (int)Math.Ceiling((double)total / limit) }
        });
    }

    [HttpGet("articles/{slug}")]
    public async Task<IActionResult> GetArticle(string slug)
    {
        var post = await _blogService.GetPostBySlugAsync(slug);
        if (post == null || post.Status != "published")
            return ApiResult.NotFound();
        return ApiResult.Success(post);
    }

    [HttpGet("featured")]
    public async Task<IActionResult> GetFeatured([FromQuery] int limit = 5)
    {
        var posts = await _blogService.GetFeaturedPostsAsync(limit);
        return ApiResult.Success(posts);
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories(CancellationToken ct)
    {
        var categories = await _categoryService.GetAllAsync(ct);
        return ApiResult.Success(categories);
    }

    [HttpGet("tags")]
    public async Task<IActionResult> GetTags(CancellationToken ct)
    {
        var tags = await _tagService.GetAllAsync(ct);
        return ApiResult.Success(tags);
    }

    [HttpPost("articles/{slug}/view")]
    public async Task<IActionResult> TrackView(string slug)
    {
        var post = await _blogService.IncrementViewCountAsync(slug);
        if (post == null) return ApiResult.NotFound();
        return ApiResult.Success(new { viewCount = post.ViewCount });
    }
}
