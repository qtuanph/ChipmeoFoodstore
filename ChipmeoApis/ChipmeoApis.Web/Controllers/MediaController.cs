using ChipmeoApis.Web.Authorization;
using ChipmeoApis.Usecase.Interfaces;
using ChipmeoApis.Usecase.DTOs.Media;
using ChipmeoApis.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace ChipmeoApis.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MediaController : ControllerBase
{
    private readonly IMediaService _mediaService;
    private readonly StoreDbContext _context;

    public MediaController(IMediaService mediaService, StoreDbContext context)
    {
        _mediaService = mediaService;
        _context = context;
    }

    [HttpPost("upload")]
    [RequirePermission("media.upload")]
    public async Task<IActionResult> UploadFile(IFormFile file, [FromForm] string? folder = "blog")
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        try
        {
            using var stream = file.OpenReadStream();
            var media = await _mediaService.UploadFileAsync(stream, file.FileName, file.ContentType, file.Length, userId, folder ?? "blog");
            return Ok(media);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    [RequirePermission("system.view")] // Or generic media view?
    public async Task<IActionResult> GetAllMedia()
    {
        var media = await _mediaService.GetAllMediaAsync();
        return Ok(media);
    }

    [HttpDelete("{id}")]
    [RequirePermission("media.delete")]
    public async Task<IActionResult> DeleteMedia(int id)
    {
        var result = await _mediaService.DeleteMediaAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Check if an image URL is being used in any entity
    /// </summary>
    [HttpGet("check-usage")]
    [RequirePermission("media.delete")]
    public async Task<IActionResult> CheckUsage([FromQuery] string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return BadRequest("URL is required");

        var usages = await GetImageUsages(url);
        return Ok(new { 
            url, 
            isInUse = usages.Count > 0, 
            usages 
        });
    }

    /// <summary>
    /// Get list of all unused images
    /// </summary>
    [HttpGet("unused")]
    [RequirePermission("media.delete")]
    public async Task<IActionResult> GetUnusedImages()
    {
        var allMedia = await _mediaService.GetAllMediaAsync();
        var usedUrls = await GetAllUsedImageUrls();

        var unusedMedia = allMedia.Where(m => 
            !usedUrls.Any(u => m.FileUrl.Contains(u, StringComparison.OrdinalIgnoreCase) || 
                               u.Contains(m.FileUrl, StringComparison.OrdinalIgnoreCase))
        ).ToList();

        return Ok(new { 
            totalMedia = allMedia.Count,
            usedCount = allMedia.Count - unusedMedia.Count,
            unusedCount = unusedMedia.Count,
            unusedMedia 
        });
    }

    /// <summary>
    /// Delete all unused images (cleanup)
    /// </summary>
    [HttpDelete("cleanup")]
    [RequirePermission("media.delete")]
    public async Task<IActionResult> CleanupUnusedImages()
    {
        var allMedia = await _mediaService.GetAllMediaAsync();
        var usedUrls = await GetAllUsedImageUrls();

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

        return Ok(new { 
            message = $"Đã xóa {deletedCount} ảnh không sử dụng",
            deletedCount,
            errors 
        });
    }

    private async Task<List<string>> GetImageUsages(string url)
    {
        var usages = new List<string>();

        // Check BlogPosts
        var blogMatch = await _context.BlogPosts.FirstOrDefaultAsync(p => 
            (p.ThumbnailUrl != null && p.ThumbnailUrl.Contains(url)) ||
            (p.OgImageUrl != null && p.OgImageUrl.Contains(url)) ||
            (p.Content != null && p.Content.Contains(url)));
        if (blogMatch != null) 
            usages.Add($"Blog: {blogMatch.Title}");

        // Check MenuItems
        var menuMatch = await _context.MenuItems.FirstOrDefaultAsync(m => 
            m.ImageUrl != null && m.ImageUrl.Contains(url));
        if (menuMatch != null) 
            usages.Add($"Món ăn: {menuMatch.Name}");

        // Check Categories
        var catMatch = await _context.Categories.FirstOrDefaultAsync(c => 
            c.ImageUrl != null && c.ImageUrl.Contains(url));
        if (catMatch != null) 
            usages.Add($"Danh mục: {catMatch.Name}");

        // Check Combos
        var comboMatch = await _context.Combos.FirstOrDefaultAsync(c => 
            c.ImageUrl != null && c.ImageUrl.Contains(url));
        if (comboMatch != null) 
            usages.Add($"Combo: {comboMatch.Name}");

        return usages;
    }

    private async Task<HashSet<string>> GetAllUsedImageUrls()
    {
        var urls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // Blog thumbnails and OG images
        var blogUrls = await _context.BlogPosts
            .Where(p => p.ThumbnailUrl != null || p.OgImageUrl != null)
            .Select(p => new { p.ThumbnailUrl, p.OgImageUrl, p.Content })
            .ToListAsync();

        foreach (var blog in blogUrls)
        {
            if (!string.IsNullOrEmpty(blog.ThumbnailUrl))
                urls.Add(blog.ThumbnailUrl);
            if (!string.IsNullOrEmpty(blog.OgImageUrl))
                urls.Add(blog.OgImageUrl);
            
            // Extract images from blog content HTML
            if (!string.IsNullOrEmpty(blog.Content))
            {
                var imgMatches = Regex.Matches(blog.Content, @"src\s*=\s*[""']([^""']+)[""']", RegexOptions.IgnoreCase);
                foreach (Match match in imgMatches)
                {
                    if (match.Groups.Count > 1)
                        urls.Add(match.Groups[1].Value);
                }
            }
        }

        // Menu item images
        var menuUrls = await _context.MenuItems
            .Where(m => m.ImageUrl != null)
            .Select(m => m.ImageUrl!)
            .ToListAsync();
        foreach (var url in menuUrls) urls.Add(url);

        // Category images
        var catUrls = await _context.Categories
            .Where(c => c.ImageUrl != null)
            .Select(c => c.ImageUrl!)
            .ToListAsync();
        foreach (var url in catUrls) urls.Add(url);

        // Combo images
        var comboUrls = await _context.Combos
            .Where(c => c.ImageUrl != null)
            .Select(c => c.ImageUrl!)
            .ToListAsync();
        foreach (var url in comboUrls) urls.Add(url);

        return urls;
    }
}




