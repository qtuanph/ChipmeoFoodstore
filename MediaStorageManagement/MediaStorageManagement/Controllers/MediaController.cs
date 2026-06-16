using Microsoft.AspNetCore.Mvc;

namespace MediaStorageManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MediaController : ControllerBase
{
    private readonly StorageConfig _config;
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".ico", ".bmp" };
    private const long MaxFileSize = 10 * 1024 * 1024; // 10MB

    public MediaController(StorageConfig config)
    {
        _config = config;
    }

    /// <summary>
    /// Get list of all folders
    /// </summary>
    [HttpGet("folders")]
    public IActionResult GetFolders()
    {
        var folders = new List<string> { "blog", "avatars", "menu-items", "combos", "categories", "misc" };

        // Also include any custom folders that exist
        if (Directory.Exists(_config.Path))
        {
            var existingFolders = Directory.GetDirectories(_config.Path)
                .Select(d => Path.GetFileName(d))
                .Where(f => !folders.Contains(f));
            folders.AddRange(existingFolders);
        }

        return Ok(folders.Distinct().OrderBy(f => f));
    }

    /// <summary>
    /// Get list of files, optionally filtered by folder
    /// </summary>
    [HttpGet]
    public IActionResult GetFiles([FromQuery] string? folder = null)
    {
        var files = new List<MediaFileDto>();

        if (!Directory.Exists(_config.Path))
            return Ok(files);

        var searchPaths = string.IsNullOrEmpty(folder)
            ? Directory.GetDirectories(_config.Path)
            : new[] { Path.Combine(_config.Path, folder) };

        foreach (var folderPath in searchPaths.Where(Directory.Exists))
        {
            var folderName = Path.GetFileName(folderPath);
            var fileInfos = new DirectoryInfo(folderPath).GetFiles();

            foreach (var file in fileInfos)
            {
                files.Add(new MediaFileDto
                {
                    FileName = file.Name,
                    Folder = folderName,
                    FileUrl = $"/{folderName}/{file.Name}",
                    FileSize = file.Length,
                    FileType = GetMimeType(file.Extension),
                    CreatedAt = file.CreationTimeUtc
                });
            }
        }

        return Ok(files.OrderByDescending(f => f.CreatedAt));
    }

    /// <summary>
    /// Upload a file
    /// </summary>
    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file, [FromForm] string folder = "misc")
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { error = "No file provided" });

        if (file.Length > MaxFileSize)
            return BadRequest(new { error = "File too large. Max size is 10MB" });

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_allowedExtensions.Contains(extension))
            return BadRequest(new { error = $"File type not allowed. Allowed: {string.Join(", ", _allowedExtensions)}" });

        // Sanitize folder name
        folder = SanitizeFolderName(folder);

        // Create folder if not exists
        var folderPath = Path.Combine(_config.Path, folder);
        Directory.CreateDirectory(folderPath);

        // Generate unique filename
        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(folderPath, uniqueFileName);

        // Save file
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var fileInfo = new FileInfo(filePath);
        return Ok(new MediaFileDto
        {
            FileName = file.FileName,
            Folder = folder,
            FileUrl = $"/{folder}/{uniqueFileName}",
            FileSize = fileInfo.Length,
            FileType = GetMimeType(extension),
            CreatedAt = fileInfo.CreationTimeUtc
        });
    }

    /// <summary>
    /// Delete a file
    /// </summary>
    [HttpDelete("{folder}/{filename}")]
    public IActionResult Delete(string folder, string filename)
    {
        folder = SanitizeFolderName(folder);
        filename = Path.GetFileName(filename); // Security: prevent path traversal

        var filePath = Path.Combine(_config.Path, folder, filename);

        if (!System.IO.File.Exists(filePath))
            return NotFound(new { error = "File not found" });

        System.IO.File.Delete(filePath);
        return Ok(new { message = "File deleted successfully" });
    }

    private static string SanitizeFolderName(string folder)
    {
        // Remove any path separators and keep only alphanumeric, dash, underscore
        return new string(folder
            .Replace("/", "").Replace("\\", "").Replace("..", "")
            .Where(c => char.IsLetterOrDigit(c) || c == '-' || c == '_')
            .ToArray());
    }

    private static string GetMimeType(string extension)
    {
        return extension.ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".svg" => "image/svg+xml",
            ".ico" => "image/x-icon",
            ".bmp" => "image/bmp",
            _ => "application/octet-stream"
        };
    }
}

public class MediaFileDto
{
    public string FileName { get; set; } = "";
    public string Folder { get; set; } = "";
    public string FileUrl { get; set; } = "";
    public long FileSize { get; set; }
    public string FileType { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}
