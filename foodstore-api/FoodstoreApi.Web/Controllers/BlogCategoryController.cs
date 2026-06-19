using FoodstoreApi.Usecase.DTOs.Blog;
using FoodstoreApi.Usecase.Interfaces;
using FoodstoreApi.Web.Authorization;
using Microsoft.AspNetCore.Mvc;
using FoodstoreApi.Web.ApiResponse;

namespace FoodstoreApi.Web.Controllers;

[Route("api/blog/categories")]
[ApiController]
public class BlogCategoryController : ControllerBase
{
    private readonly IBlogCategoryService _categoryService;

    public BlogCategoryController(IBlogCategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var categories = await _categoryService.GetAllAsync(ct);
        return ApiResult.Success(categories);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var category = await _categoryService.GetByIdAsync(id, ct);
        if (category == null) return ApiResult.NotFound();
        return ApiResult.Success(category);
    }

    [HttpPost]
    [RequirePermission("blog.create")]
    public async Task<IActionResult> Create([FromBody] CreateBlogCategoryDto dto, CancellationToken ct)
    {
        var category = await _categoryService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }

    [HttpPut("{id:guid}")]
    [RequirePermission("blog.update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBlogCategoryDto dto, CancellationToken ct)
    {
        var category = await _categoryService.UpdateAsync(id, dto, ct);
        if (category == null) return ApiResult.NotFound();
        return ApiResult.Success(category);
    }

    [HttpDelete("{id:guid}")]
    [RequirePermission("blog.delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await _categoryService.DeleteAsync(id, ct);
        if (!result) return ApiResult.NotFound();
        return NoContent();
    }
}
