using FoodstoreApi.Usecase.DTOs;
using FoodstoreApi.Usecase.DTOs.Media;

namespace FoodstoreApi.Usecase.Interfaces;

public interface IMediaService
{
    Task<MediaDto> UploadFileAsync(Stream fileStream, string fileName, string contentType, long fileSize, Guid? uploadedBy, string folder = "misc");
    Task<MediaDto> UploadFileForCustomerAsync(Stream fileStream, string fileName, string contentType, long fileSize, Guid? uploadedByCustomer, string folder = "misc");
    Task<List<MediaDto>> GetAllMediaAsync();
    Task<bool> DeleteMediaAsync(Guid id);
    Task LinkMediaToEntityAsync(string fileUrl, string entityType, Guid entityId);
    Task DeleteMediaByEntityAsync(string entityType, Guid entityId);
    Task LinkMediaFromContentAsync(string? content, string entityType, Guid entityId);
    Task<ImageUsageResult> CheckImageUsageAsync(string url);
    Task<HashSet<string>> GetAllUsedImageUrlsAsync();
}

public record ImageUsageResult(string Url, bool IsInUse, List<ImageUsageDetail> Usages);
public record ImageUsageDetail(string EntityType, string EntityName);
