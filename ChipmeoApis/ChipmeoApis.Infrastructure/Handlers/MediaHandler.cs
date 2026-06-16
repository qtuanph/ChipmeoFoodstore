using ChipmeoApis.Usecase.DTOs.Media;
using ChipmeoApis.Usecase.Interfaces;
using ChipmeoApis.Usecase.DTOs;
using ChipmeoApis.Core.Entities;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ChipmeoApis.Infrastructure.Handlers;

public class MediaHandler : IMediaService
{
    private readonly IMediaRepository _repo;
    private readonly HttpClient _httpClient;
    private readonly string _mediaApiUrl;
    private readonly string _mediaApiKey;

    public MediaHandler(IMediaRepository repo, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _repo = repo;
        _httpClient = httpClientFactory.CreateClient();
        
        // Media Storage API settings
        _mediaApiUrl = configuration["Media:ApiUrl"] ?? "https://media.chipmeo.io.vn";
        _mediaApiKey = configuration["Media:ApiKey"] ?? "chipmeo-media-api-key-2024";
    }

    public async Task<MediaDto> UploadFileAsync(Stream fileStream, string fileName, string contentType, long fileSize, int? uploadedBy, string folder = "misc")
    {
        if (fileStream == null || fileStream.Length == 0)
            throw new ArgumentException("File is empty");

        // Upload to Media Storage API
        using var content = new MultipartFormDataContent();
        using var streamContent = new StreamContent(fileStream);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        content.Add(streamContent, "file", fileName);
        content.Add(new StringContent(folder), "folder");

        var request = new HttpRequestMessage(HttpMethod.Post, $"{_mediaApiUrl}/api/media/upload");
        request.Headers.Add("X-Api-Key", _mediaApiKey);
        request.Content = content;

        var response = await _httpClient.SendAsync(request);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Media upload failed: {errorContent}");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var uploadResult = JsonSerializer.Deserialize<MediaApiResponse>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (uploadResult == null)
            throw new Exception("Failed to parse media upload response");

        // Build full URL
        var fileUrl = uploadResult.FileUrl.StartsWith("http") 
            ? uploadResult.FileUrl 
            : $"{_mediaApiUrl}{uploadResult.FileUrl}";

        var media = new Media
        {
            FileName = fileName,
            Folder = folder,
            FileUrl = fileUrl,
            FileType = contentType,
            FileSize = fileSize,
            UploadedByEmployee = uploadedBy,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repo.AddAsync(media);

        return new MediaDto
        {
            Id = created.Id,
            FileName = created.FileName,
            Folder = created.Folder,
            FileUrl = created.FileUrl,
            FileType = created.FileType,
            FileSize = created.FileSize,
            CreatedAt = created.CreatedAt
        };
    }

    public async Task<bool> DeleteMediaAsync(int id)
    {
        var media = await _repo.GetByIdAsync(id);
        if (media == null) return false;

        // Try to delete from Media Storage API
        try
        {
            var uri = new Uri(media.FileUrl);
            var urlPath = uri.AbsolutePath.TrimStart('/'); // blog/xxx.jpg
            
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{_mediaApiUrl}/api/media/{urlPath}");
            request.Headers.Add("X-Api-Key", _mediaApiKey);
            await _httpClient.SendAsync(request);
        }
        catch { /* Ignore file deletion errors */ }

        await _repo.DeleteAsync(media);
        return true;
    }

    public async Task<List<MediaDto>> GetAllMediaAsync()
    {
        var mediaList = await _repo.GetAllAsync();
        return mediaList.Select(m => new MediaDto
        {
            Id = m.Id,
            FileName = m.FileName,
            Folder = m.Folder,
            FileUrl = m.FileUrl,
            FileType = m.FileType,
            FileSize = m.FileSize,
            EntityType = m.EntityType,
            EntityId = m.EntityId,
            CreatedAt = m.CreatedAt
        }).ToList();
    }

    public async Task LinkMediaToEntityAsync(string fileUrl, string entityType, int entityId)
    {
        if (string.IsNullOrEmpty(fileUrl)) return;

        var media = await _repo.GetByUrlAsync(fileUrl);
        if (media != null)
        {
            media.EntityType = entityType;
            media.EntityId = entityId;
            // The repository doesn't have an UpdateAsync, but EF Core tracks changes if retrieved from context.
            // However, MediaRepository.AddAsync and DeleteAsync call SaveChangesAsync.
            // We need a way to save changes. Since I cannot change existing repo interface easily without ensuring all implementations update (luckily only one here),
            // I'll assume I can just use the context if I had access or add UpdateAsync to IRepostory.
            // Actually, let's add UpdateAsync to IMediaRepository properly in next step if needed, 
            // OR reuse AddAsync if it handles update (unlikely), 
            // OR rely on direct context manipulation if repo exposed it (it doesn't).
            // Best approach: Add UpdateAsync to IMediaRepository. I missed that in the plan but I can do it now.
            // Wait, I can't modify interface again in this tool call sequence easily without proper order.
            // I'll implement logic assuming I will add UpdateAsync to repo in next steps.
            
            // Re-reading MediaRepository: it uses _context.Media.Add(media).
            // I should add UpdateAsync to MediaRepository first.
            // For now I will put the logic here and will fix the repository in next turn.
            await _repo.UpdateAsync(media);
        }
    }

    public async Task DeleteMediaByEntityAsync(string entityType, int entityId)
    {
        var mediaList = await _repo.GetByEntityAsync(entityType, entityId);
        foreach (var media in mediaList)
        {
            await DeleteMediaAsync(media.Id);
        }
    }

    public async Task LinkMediaFromContentAsync(string? content, string entityType, int entityId)
    {
        if (string.IsNullOrWhiteSpace(content)) return;

        // Regex to find src="..."
        var matches = System.Text.RegularExpressions.Regex.Matches(content, @"src\s*=\s*[""']([^""']+)[""']", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        foreach (System.Text.RegularExpressions.Match match in matches)
        {
            if (match.Groups.Count > 1)
            {
                var url = match.Groups[1].Value;
                // Only link if it's our media
                if (url.Contains(_mediaApiUrl) || !url.StartsWith("http")) 
                {
                     await LinkMediaToEntityAsync(url, entityType, entityId);
                }
            }
        }
    }
}

public class MediaApiResponse
{
    public string FileName { get; set; } = "";
    public string Folder { get; set; } = "";
    public string FileUrl { get; set; } = "";
    public long FileSize { get; set; }
    public string FileType { get; set; } = "";
}








