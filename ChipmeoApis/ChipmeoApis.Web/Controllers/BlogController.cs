using ChipmeoApis.Usecase.Interfaces;
using ChipmeoApis.Usecase.DTOs;
using ChipmeoApis.Usecase.DTOs.Blog;
using ChipmeoApis.Web.Authorization;
using ChipmeoApis.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChipmeoApis.Web.ApiResponse;

namespace ChipmeoApis.Web.Controllers;

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
    public async Task<IActionResult> GetAllPosts([FromQuery] string? status)
    {
        var posts = await _blogService.GetAllPostsAsync(status);
        return ApiResult.Success(posts);
    }

    [HttpGet("{slug}")]
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
        var userId = User.GetUserId();
        var post = await _blogService.CreatePostAsync(dto, userId);
        return CreatedAtAction(nameof(GetPostBySlug), new { slug = post.Slug }, post);
    }

    [HttpPut("{id}")]
    [RequirePermission("blog.update")]
    public async Task<IActionResult> UpdatePost(int id, [FromBody] UpdateBlogPostDto dto)
    {
        var post = await _blogService.UpdatePostAsync(id, dto);
        if (post == null) return ApiResult.NotFound();
        return ApiResult.Success(post);
    }

    [HttpDelete("{id}")]
    [RequirePermission("blog.delete")]
    public async Task<IActionResult> DeletePost(int id)
    {
        var result = await _blogService.DeletePostAsync(id);
        if (!result) return ApiResult.NotFound();
        return NoContent();
    }
}




