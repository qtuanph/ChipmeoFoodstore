using ChipmeoApis.Usecase.DTOs;
using ChipmeoApis.Usecase.DTOs.Media;


namespace ChipmeoApis.Usecase.Interfaces;

public interface IMediaService
{
    Task<MediaDto> UploadFileAsync(Stream fileStream, string fileName, string contentType, long fileSize, int? uploadedBy, string folder = "misc");
    Task<List<MediaDto>> GetAllMediaAsync();
    Task<bool> DeleteMediaAsync(int id);
    Task LinkMediaToEntityAsync(string fileUrl, string entityType, int entityId);
    Task DeleteMediaByEntityAsync(string entityType, int entityId);
    Task LinkMediaFromContentAsync(string? content, string entityType, int entityId);
}




