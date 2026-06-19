using FoodstoreApi.Usecase.DTOs.Blog;
using FoodstoreApi.Usecase.Interfaces;
using FoodstoreApi.Web.Authorization;
using Microsoft.AspNetCore.Mvc;
using FoodstoreApi.Web.ApiResponse;

namespace FoodstoreApi.Web.Controllers;

[Route("api/blog/{postId:guid}/blocks")]
[ApiController]
public class BlogBlockController : ControllerBase
{
    private readonly IBlogBlockService _blockService;

    public BlogBlockController(IBlogBlockService blockService)
    {
        _blockService = blockService;
    }

    [HttpGet]
    public async Task<IActionResult> GetByPost(Guid postId, CancellationToken ct)
    {
        var blocks = await _blockService.GetByPostIdAsync(postId, ct);
        return ApiResult.Success(blocks);
    }

    [HttpPost]
    [RequirePermission("blog.update")]
    public async Task<IActionResult> Create(Guid postId, [FromBody] CreateBlogBlockDto dto, CancellationToken ct)
    {
        var block = await _blockService.CreateAsync(postId, dto, ct);
        return CreatedAtAction(nameof(GetByPost), new { postId }, block);
    }

    [HttpPut("{blockId:guid}")]
    [RequirePermission("blog.update")]
    public async Task<IActionResult> Update(Guid postId, Guid blockId, [FromBody] UpdateBlogBlockDto dto, CancellationToken ct)
    {
        var result = await _blockService.UpdateAsync(blockId, dto, ct);
        if (!result) return ApiResult.NotFound();
        return NoContent();
    }

    [HttpDelete("{blockId:guid}")]
    [RequirePermission("blog.update")]
    public async Task<IActionResult> Delete(Guid postId, Guid blockId, CancellationToken ct)
    {
        var result = await _blockService.DeleteAsync(blockId, ct);
        if (!result) return ApiResult.NotFound();
        return NoContent();
    }

    [HttpPut]
    [RequirePermission("blog.update")]
    public async Task<IActionResult> SetBlocks(Guid postId, [FromBody] List<CreateBlogBlockDto> blocks, CancellationToken ct)
    {
        var result = await _blockService.SetPostBlocksAsync(postId, blocks, ct);
        return ApiResult.Success(result);
    }
}
