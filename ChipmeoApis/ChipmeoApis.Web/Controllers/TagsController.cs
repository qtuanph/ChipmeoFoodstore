using ChipmeoApis.Usecase.DTOs.Tag;
using ChipmeoApis.Usecase.Services;
using ChipmeoApis.Web.Authorization;
using ChipmeoApis.Usecase.DTOs.Blog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChipmeoApis.Web.Controllers;

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
        return Ok(tags);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var tag = await _tagService.GetByIdAsync(id, cancellationToken);
        if (tag == null) return NotFound();
        return Ok(tag);
    }

    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetBySlug(string slug, CancellationToken cancellationToken)
    {
        var tag = await _tagService.GetBySlugAsync(slug, cancellationToken);
        if (tag == null) return NotFound();
        return Ok(tag);
    }

    [HttpPost]
    [RequirePermission("blog.create")]
    public async Task<IActionResult> Create([FromBody] CreateTagDto dto, CancellationToken cancellationToken)
    {
        var tag = await _tagService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = tag.Id }, tag);
    }

    [HttpPut("{id:int}")]
    [RequirePermission("blog.update")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTagDto dto, CancellationToken cancellationToken)
    {
        var tag = await _tagService.UpdateAsync(id, dto, cancellationToken);
        if (tag == null) return NotFound();
        return Ok(tag);
    }

    [HttpDelete("{id:int}")]
    [RequirePermission("blog.delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _tagService.DeleteAsync(id, cancellationToken);
        if (!deleted) return NotFound();
        return NoContent();
    }

    [HttpGet("post/{postId:int}")]
    public async Task<IActionResult> GetByPost(int postId, CancellationToken cancellationToken)
    {
        var tags = await _tagService.GetByPostIdAsync(postId, cancellationToken);
        return Ok(tags);
    }

    [HttpPut("post/{postId:int}")]
    [RequirePermission("blog.update")]
    public async Task<IActionResult> SetPostTags(int postId, [FromBody] int[] tagIds, CancellationToken cancellationToken)
    {
        await _tagService.SetPostTagsAsync(postId, tagIds, cancellationToken);
        return NoContent();
    }
}




