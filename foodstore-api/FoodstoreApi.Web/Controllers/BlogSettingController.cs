using FoodstoreApi.Usecase.DTOs.Blog;
using FoodstoreApi.Usecase.Interfaces;
using FoodstoreApi.Web.Authorization;
using Microsoft.AspNetCore.Mvc;
using FoodstoreApi.Web.ApiResponse;

namespace FoodstoreApi.Web.Controllers;

[Route("api/blog/settings")]
[ApiController]
public class BlogSettingController : ControllerBase
{
    private readonly IBlogSettingService _settingService;

    public BlogSettingController(IBlogSettingService settingService)
    {
        _settingService = settingService;
    }

    [HttpGet]
    [RequirePermission("system.view")]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var settings = await _settingService.GetAllAsync(ct);
        return ApiResult.Success(settings);
    }

    [HttpGet("{key}")]
    public async Task<IActionResult> GetByKey(string key, CancellationToken ct)
    {
        var setting = await _settingService.GetByKeyAsync(key, ct);
        if (setting == null) return ApiResult.NotFound();
        return ApiResult.Success(setting);
    }

    [HttpPut("{key}")]
    [RequirePermission("system.update")]
    public async Task<IActionResult> Upsert(string key, [FromBody] UpdateBlogSettingDto dto, CancellationToken ct)
    {
        var setting = await _settingService.UpsertAsync(key, dto, ct);
        return ApiResult.Success(setting);
    }

    [HttpDelete("{id:guid}")]
    [RequirePermission("system.update")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await _settingService.DeleteAsync(id, ct);
        if (!result) return ApiResult.NotFound();
        return NoContent();
    }
}
