using ChipmeoApis.Core.Entities;

namespace ChipmeoApis.Usecase.Interfaces;

public interface IBlogRepository
{
    Task<IEnumerable<BlogPost>> GetAllAsync(string? status = null);
    Task<BlogPost?> GetByIdAsync(int id);
    Task<BlogPost?> GetBySlugAsync(string slug);
    Task<BlogPost> AddAsync(BlogPost post);
    Task UpdateAsync(BlogPost post);
    Task DeleteAsync(BlogPost post);
}




