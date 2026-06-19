using FoodstoreApi.Usecase.DTOs.Tag;
using FoodstoreApi.Usecase.Services;
using FoodstoreApi.Web.Authorization;
using FoodstoreApi.Usecase.DTOs.Blog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FoodstoreApi.Web.ApiResponse;

namespace FoodstoreApi.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagsController : ControllerBase
{
    private readonly ITagService _tagService;

    public TagsController(ITagService tagService)
    {
        _tagService = tagService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var tags = await _tagService.GetAllAsync(cancellationToken);
        return ApiResult.Success(tags);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var tag = await _tagService.GetByIdAsync(id, cancellationToken);
        if (tag == null) return ApiResult.NotFound();
        return ApiResult.Success(tag);
    }

    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetBySlug(string slug, CancellationToken cancellationToken)
    {
        var tag = await _tagService.GetBySlugAsync(slug, cancellationToken);
        if (tag == null) return ApiResult.NotFound();
        return ApiResult.Success(tag);
    }

    [HttpPost]
    [RequirePermission("blog.create")]
    public async Task<IActionResult> Create([FromBody] CreateTagDto dto, CancellationToken cancellationToken)
    {
        var tag = await _tagService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = tag.Id }, tag);
    }

    [HttpPut("{id:guid}")]
    [RequirePermission("blog.update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTagDto dto, CancellationToken cancellationToken)
    {
        var tag = await _tagService.UpdateAsync(id, dto, cancellationToken);
        if (tag == null) return ApiResult.NotFound();
        return ApiResult.Success(tag);
    }

    [HttpDelete("{id:guid}")]
    [RequirePermission("blog.delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _tagService.DeleteAsync(id, cancellationToken);
        if (!deleted) return ApiResult.NotFound();
        return NoContent();
    }

    [HttpGet("post/{postId:guid}")]
    public async Task<IActionResult> GetByPost(Guid postId, CancellationToken cancellationToken)
    {
        var tags = await _tagService.GetByPostIdAsync(postId, cancellationToken);
        return ApiResult.Success(tags);
    }

    [HttpPut("post/{postId:guid}")]
    [RequirePermission("blog.update")]
    public async Task<IActionResult> SetPostTags(Guid postId, [FromBody] Guid[] tagIds, CancellationToken cancellationToken)
    {
        await _tagService.SetPostTagsAsync(postId, tagIds, cancellationToken);
        return NoContent();
    }
}
