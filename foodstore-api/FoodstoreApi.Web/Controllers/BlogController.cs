using FoodstoreApi.Usecase.Interfaces;
using FoodstoreApi.Usecase.DTOs.Blog;
using FoodstoreApi.Web.Authorization;
using FoodstoreApi.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FoodstoreApi.Web.ApiResponse;

namespace FoodstoreApi.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BlogController : ControllerBase
{
    private readonly IBlogService _blogService;

    public BlogController(IBlogService blogService)
    {
        _blogService = blogService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPosts(
        [FromQuery] string? status,
        [FromQuery] string? categorySlug,
        [FromQuery] string? tagSlug)
    {
        var posts = await _blogService.GetAllPostsAsync(status, categorySlug, tagSlug);
        return ApiResult.Success(posts);
    }

    [HttpGet("{id:guid}", Name = "GetPostById")]
    public async Task<IActionResult> GetPostById(Guid id)
    {
        var post = await _blogService.GetPostByIdAsync(id);
        if (post == null) return ApiResult.NotFound();
        return ApiResult.Success(post);
    }

    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetPostBySlug(string slug)
    {
        var post = await _blogService.GetPostBySlugAsync(slug);
        if (post == null) return ApiResult.NotFound();
        return ApiResult.Success(post);
    }

    [HttpPost]
    [RequirePermission("blog.create")]
    public async Task<IActionResult> CreatePost([FromBody] CreateBlogPostDto dto)
    {
        var employeeId = User.GetEmployeeId();
        var post = await _blogService.CreatePostAsync(dto, employeeId);
        return CreatedAtAction(nameof(GetPostById), new { id = post.Id }, ApiResult.Success(post));
    }

    [HttpPut("{id:guid}")]
    [RequirePermission("blog.update")]
    public async Task<IActionResult> UpdatePost(Guid id, [FromBody] UpdateBlogPostDto dto)
    {
        var post = await _blogService.UpdatePostAsync(id, dto);
        if (post == null) return ApiResult.NotFound();
        return ApiResult.Success(post);
    }

    [HttpDelete("{id:guid}")]
    [RequirePermission("blog.delete")]
    public async Task<IActionResult> DeletePost(Guid id)
    {
        var result = await _blogService.DeletePostAsync(id);
        if (!result) return ApiResult.NotFound();
        return NoContent();
    }

    [HttpPatch("{id:guid}/status")]
    [RequirePermission("blog.update")]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] string status)
    {
        var employeeId = User.GetEmployeeId();
        var post = await _blogService.ChangeStatusAsync(id, status, employeeId);
        if (post == null) return ApiResult.NotFound();
        return ApiResult.Success(post);
    }

    [HttpPatch("{id:guid}/schedule")]
    [RequirePermission("blog.update")]
    public async Task<IActionResult> SchedulePost(Guid id, [FromBody] DateTime? scheduledAt)
    {
        var result = await _blogService.SchedulePostAsync(id, scheduledAt);
        if (!result) return ApiResult.NotFound();
        return NoContent();
    }

    [HttpPost("{slug}/view")]
    public async Task<IActionResult> IncrementView(string slug)
    {
        var post = await _blogService.IncrementViewCountAsync(slug);
        if (post == null) return ApiResult.NotFound();
        return ApiResult.Success(new { viewCount = post.ViewCount });
    }

    [HttpGet("dashboard/stats")]
    [RequirePermission("dashboard.view")]
    public async Task<IActionResult> GetDashboardStats()
    {
        var stats = await _blogService.GetDashboardStatsAsync();
        return ApiResult.Success(stats);
    }

    [HttpGet("featured")]
    public async Task<IActionResult> GetFeatured([FromQuery] int limit = 5)
    {
        var posts = await _blogService.GetFeaturedPostsAsync(limit);
        return ApiResult.Success(posts);
    }

    [HttpGet("published")]
    public async Task<IActionResult> GetPublished(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10,
        [FromQuery] string? categorySlug = null,
        [FromQuery] string? tagSlug = null)
    {
        var posts = await _blogService.GetPublishedPostsAsync(page, limit, categorySlug, tagSlug);
        var total = await _blogService.GetTotalPublishedCountAsync(categorySlug, tagSlug);
        return ApiResult.Success(new { posts, total, page, limit });
    }

    [HttpGet("{id:guid}/revisions")]
    public async Task<IActionResult> GetRevisions(Guid id, [FromServices] IBlogRevisionService revisionService)
    {
        var revisions = await revisionService.GetByPostIdAsync(id);
        return ApiResult.Success(revisions);
    }

    [HttpPost("{id:guid}/revisions")]
    [RequirePermission("blog.update")]
    public async Task<IActionResult> CreateRevision(Guid id, [FromServices] IBlogRevisionService revisionService)
    {
        var employeeId = User.GetEmployeeId();
        var revision = await revisionService.CreateSnapshotAsync(id, employeeId);
        return ApiResult.Success(revision);
    }

    [HttpPost("revisions/{revisionId:guid}/restore")]
    [RequirePermission("blog.update")]
    public async Task<IActionResult> RestoreRevision(Guid revisionId, [FromServices] IBlogRevisionService revisionService)
    {
        var post = await revisionService.RestoreAsync(revisionId);
        if (post == null) return ApiResult.NotFound();
        return ApiResult.Success(post);
    }
}
