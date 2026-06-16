using ChipmeoApis.Core.Entities;

namespace ChipmeoApis.Usecase.Interfaces;

public interface IMediaRepository
{
    Task<IEnumerable<Media>> GetAllAsync();
    Task<Media?> GetByIdAsync(int id);
    Task<Media?> GetByUrlAsync(string url);
    Task<IEnumerable<Media>> GetByEntityAsync(string entityType, int entityId);
    Task<Media> AddAsync(Media media);
    Task UpdateAsync(Media media);
    Task DeleteAsync(Media media);
}




