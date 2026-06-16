using ChipmeoApis.Usecase.Interfaces;
using ChipmeoApis.Usecase.DTOs.Category;
using ChipmeoApis.Core.Entities;

namespace ChipmeoApis.Usecase.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repo;
    private readonly IMediaService _mediaService;

    public CategoryService(ICategoryRepository repo, IMediaService mediaService)
    {
        _repo = repo;
        _mediaService = mediaService;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _repo.GetAllAsync(cancellationToken);
        return categories.Select(c => new CategoryDto(c.Id, c.Name, c.Description, c.ImageUrl, c.IsActive, c.CreatedAt));
    }

    public async Task<CategoryDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var c = await _repo.GetByIdAsync(id, cancellationToken);
        if (c == null) return null;
        return new CategoryDto(c.Id, c.Name, c.Description, c.ImageUrl, c.IsActive, c.CreatedAt);
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto, CancellationToken cancellationToken = default)
    {
        var entity = new Category { Name = dto.Name, Description = dto.Description, ImageUrl = dto.ImageUrl, IsActive = dto.IsActive };
        var created = await _repo.AddAsync(entity, cancellationToken);
        
        if (!string.IsNullOrEmpty(dto.ImageUrl))
        {
            await _mediaService.LinkMediaToEntityAsync(dto.ImageUrl, "category", created.Id);
        }

        return new CategoryDto(created.Id, created.Name, created.Description, created.ImageUrl, created.IsActive, created.CreatedAt);
    }

    public async Task<bool> UpdateAsync(int id, CreateCategoryDto dto, CancellationToken cancellationToken = default)
    {
        var existing = await _repo.GetByIdAsync(id, cancellationToken);
        if (existing == null) return false;
        
        var oldImageUrl = existing.ImageUrl;

        existing.Name = dto.Name;
        existing.Description = dto.Description;
        existing.ImageUrl = dto.ImageUrl;
        existing.IsActive = dto.IsActive;
        
        await _repo.UpdateAsync(existing, cancellationToken);

        if (!string.IsNullOrEmpty(dto.ImageUrl) && dto.ImageUrl != oldImageUrl)
        {
            await _mediaService.LinkMediaToEntityAsync(dto.ImageUrl, "category", id);
        }

        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await _repo.GetByIdAsync(id, cancellationToken);
        if (existing == null) return false;
        
        await _repo.DeleteAsync(existing, cancellationToken);
        await _mediaService.DeleteMediaByEntityAsync("category", id);
        
        return true;
    }
}




