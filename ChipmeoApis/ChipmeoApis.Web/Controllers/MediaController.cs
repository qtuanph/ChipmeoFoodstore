using ChipmeoApis.Web.Authorization;
using ChipmeoApis.Web.Extensions;
using ChipmeoApis.Usecase.Interfaces;
using ChipmeoApis.Usecase.DTOs.Media;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChipmeoApis.Web.ApiResponse;

namespace ChipmeoApis.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MediaController : ControllerBase
{
    private readonly IMediaService _mediaService;

    public MediaController(IMediaService mediaService)
    {
        _mediaService = mediaService;
    }

    [HttpPost("upload")]
    [RequirePermission("media.upload")]
    public async Task<IActionResult> UploadFile(IFormFile file, [FromForm] string? folder = "blog")
    {
        if (file == null || file.Length == 0)
            return ApiResult.BadRequest("No file uploaded");

        var userId = User.GetUserId();
        try
        {
            using var stream = file.OpenReadStream();
            var media = await _mediaService.UploadFileAsync(stream, file.FileName, file.ContentType, file.Length, userId, folder ?? "blog");
            return ApiResult.Success(media);
        }
        catch (ArgumentException ex)
        {
            return ApiResult.BadRequest(ex.Message);
        }
    }

    [HttpGet]
    [RequirePermission("system.view")]
    public async Task<IActionResult> GetAllMedia()
    {
        var media = await _mediaService.GetAllMediaAsync();
        return ApiResult.Success(media);
    }

    [HttpDelete("{id}")]
    [RequirePermission("media.delete")]
    public async Task<IActionResult> DeleteMedia(int id)
    {
        var result = await _mediaService.DeleteMediaAsync(id);
        if (!result) return ApiResult.NotFound();
        return NoContent();
    }

    [HttpGet("check-usage")]
    [RequirePermission("media.delete")]
    public async Task<IActionResult> CheckUsage([FromQuery] string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return ApiResult.BadRequest("URL is required");

        var result = await _mediaService.CheckImageUsageAsync(url);
        return ApiResult.Success(result);
    }

    [HttpGet("unused")]
    [RequirePermission("media.delete")]
    public async Task<IActionResult> GetUnusedImages()
    {
        var allMedia = await _mediaService.GetAllMediaAsync();
        var usedUrls = await _mediaService.GetAllUsedImageUrlsAsync();

        var unusedMedia = allMedia.Where(m =>
            !usedUrls.Any(u => m.FileUrl.Contains(u, StringComparison.OrdinalIgnoreCase) ||
                               u.Contains(m.FileUrl, StringComparison.OrdinalIgnoreCase))
        ).ToList();

        return ApiResult.Success(new
        {
            totalMedia = allMedia.Count,
            usedCount = allMedia.Count - unusedMedia.Count,
            unusedCount = unusedMedia.Count,
            unusedMedia
        });
    }

    [HttpDelete("cleanup")]
    [RequirePermission("media.delete")]
    public async Task<IActionResult> CleanupUnusedImages()
    {
        var allMedia = await _mediaService.GetAllMediaAsync();
        var usedUrls = await _mediaService.GetAllUsedImageUrlsAsync();

        var unusedMedia = allMedia.Where(m =>
            !usedUrls.Any(u => m.FileUrl.Contains(u, StringComparison.OrdinalIgnoreCase) ||
                               u.Contains(m.FileUrl, StringComparison.OrdinalIgnoreCase))
        ).ToList();

        var deletedCount = 0;
        var errors = new List<string>();

        foreach (var media in unusedMedia)
        {
            try
            {
                var result = await _mediaService.DeleteMediaAsync(media.Id);
                if (result) deletedCount++;
            }
            catch (Exception ex)
            {
                errors.Add($"Failed to delete {media.FileName}: {ex.Message}");
            }
        }

        return ApiResult.Success(new
        {
            message = $"Đã xóa {deletedCount} ảnh không sử dụng",
            deletedCount,
            errors
        });
    }
}
