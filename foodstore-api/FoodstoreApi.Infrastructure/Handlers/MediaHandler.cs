using System.Text.RegularExpressions;
using FoodstoreApi.Usecase.DTOs.Media;
using FoodstoreApi.Usecase.Interfaces;
using FoodstoreApi.Core.Entities;
using FoodstoreApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;

namespace FoodstoreApi.Infrastructure.Handlers;

public class MediaHandler : IMediaService
{
    private readonly IMediaRepository _repo;
    private readonly IAmazonS3 _s3Client;
    private readonly StoreDbContext _context;
    private readonly string _bucket;
    private readonly string _publicUrl;

    public MediaHandler(IMediaRepository repo, IAmazonS3 s3Client, StoreDbContext context, IConfiguration configuration)
    {
        _repo = repo;
        _s3Client = s3Client;
        _context = context;
        _bucket = configuration["S3:Bucket"] ?? "food-media";
        _publicUrl = configuration["S3:PublicUrl"] ?? "http://localhost:9000/food-media";
    }

    private async Task EnsureBucketExistsAsync()
    {
        var exists = await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, _bucket);
        if (!exists)
            await _s3Client.PutBucketAsync(new PutBucketRequest { BucketName = _bucket });

        await _s3Client.PutBucketPolicyAsync(new PutBucketPolicyRequest
        {
            BucketName = _bucket,
            Policy = $$"""
            {
              "Version": "2012-10-17",
              "Statement": [
                {
                  "Effect": "Allow",
                  "Principal": "*",
                  "Action": "s3:GetObject",
                  "Resource": "arn:aws:s3:::{{_bucket}}/*"
                }
              ]
            }
            """
        });
    }

    public async Task<MediaDto> UploadFileAsync(Stream fileStream, string fileName, string contentType, long fileSize, int? uploadedBy, string folder = "misc")
    {
        if (fileStream == null || fileStream.Length == 0)
            throw new ArgumentException("File is empty");

        await EnsureBucketExistsAsync();

        var ext = Path.GetExtension(fileName);
        var objectName = $"{folder}/{Guid.NewGuid()}{ext}";

        var putRequest = new PutObjectRequest
        {
            BucketName = _bucket,
            Key = objectName,
            InputStream = fileStream,
            ContentType = contentType
        };
        putRequest.Headers.ContentLength = fileStream.Length;

        await _s3Client.PutObjectAsync(putRequest);

        var fileUrl = $"{_publicUrl}/{objectName}";

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

        try
        {
            var uri = new Uri(media.FileUrl);
            var path = uri.AbsolutePath.TrimStart('/');
            var bucketPrefix = $"{_bucket}/";
            var idx = path.IndexOf(bucketPrefix, StringComparison.OrdinalIgnoreCase);
            var objectName = idx >= 0 ? path[(idx + bucketPrefix.Length)..] : path;

            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = _bucket,
                Key = objectName
            };
            await _s3Client.DeleteObjectAsync(deleteRequest);
        }
        catch { /* Ignore S3 deletion errors */ }

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

        var matches = System.Text.RegularExpressions.Regex.Matches(content, @"src\s*=\s*[""']([^""']+)[""']", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        foreach (System.Text.RegularExpressions.Match match in matches)
        {
            if (match.Groups.Count > 1)
            {
                var url = match.Groups[1].Value;
                if (url.Contains(_publicUrl) || !url.StartsWith("http"))
                {
                     await LinkMediaToEntityAsync(url, entityType, entityId);
                }
            }
        }
    }

    public async Task<ImageUsageResult> CheckImageUsageAsync(string url)
    {
        var usages = new List<ImageUsageDetail>();

        var blogMatch = await _context.BlogPosts.FirstOrDefaultAsync(p =>
            (p.ThumbnailUrl != null && p.ThumbnailUrl.Contains(url)) ||
            (p.OgImageUrl != null && p.OgImageUrl.Contains(url)) ||
            (p.Content != null && p.Content.Contains(url)));
        if (blogMatch != null)
            usages.Add(new ImageUsageDetail("Blog", blogMatch.Title));

        var menuMatch = await _context.MenuItems.FirstOrDefaultAsync(m =>
            m.ImageUrl != null && m.ImageUrl.Contains(url));
        if (menuMatch != null)
            usages.Add(new ImageUsageDetail("Món ăn", menuMatch.Name));

        var catMatch = await _context.Categories.FirstOrDefaultAsync(c =>
            c.ImageUrl != null && c.ImageUrl.Contains(url));
        if (catMatch != null)
            usages.Add(new ImageUsageDetail("Danh mục", catMatch.Name));

        var comboMatch = await _context.Combos.FirstOrDefaultAsync(c =>
            c.ImageUrl != null && c.ImageUrl.Contains(url));
        if (comboMatch != null)
            usages.Add(new ImageUsageDetail("Combo", comboMatch.Name));

        return new ImageUsageResult(url, usages.Count > 0, usages);
    }

    public async Task<HashSet<string>> GetAllUsedImageUrlsAsync()
    {
        var urls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var blogUrls = await _context.BlogPosts
            .Where(p => p.ThumbnailUrl != null || p.OgImageUrl != null)
            .Select(p => new { p.ThumbnailUrl, p.OgImageUrl, p.Content })
            .ToListAsync();

        foreach (var blog in blogUrls)
        {
            if (!string.IsNullOrEmpty(blog.ThumbnailUrl)) urls.Add(blog.ThumbnailUrl);
            if (!string.IsNullOrEmpty(blog.OgImageUrl)) urls.Add(blog.OgImageUrl);
            if (!string.IsNullOrEmpty(blog.Content))
            {
                var imgMatches = Regex.Matches(blog.Content, @"src\s*=\s*[""']([^""']+)[""']", RegexOptions.IgnoreCase);
                foreach (Match match in imgMatches)
                    if (match.Groups.Count > 1) urls.Add(match.Groups[1].Value);
            }
        }

        var menuUrls = await _context.MenuItems
            .Where(m => m.ImageUrl != null).Select(m => m.ImageUrl!).ToListAsync();
        foreach (var url in menuUrls) urls.Add(url);

        var catUrls = await _context.Categories
            .Where(c => c.ImageUrl != null).Select(c => c.ImageUrl!).ToListAsync();
        foreach (var url in catUrls) urls.Add(url);

        var comboUrls = await _context.Combos
            .Where(c => c.ImageUrl != null).Select(c => c.ImageUrl!).ToListAsync();
        foreach (var url in comboUrls) urls.Add(url);

        return urls;
    }
}
