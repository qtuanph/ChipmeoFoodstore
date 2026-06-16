using ChipmeoApis.Usecase.Interfaces;
using ChipmeoApis.Usecase.DTOs.MenuItem;
using ChipmeoApis.Core.Entities;
using ChipmeoApis.Core.Utils;

using Microsoft.Extensions.Caching.Memory;

namespace ChipmeoApis.Usecase.Services;

public class MenuItemService(IMenuItemRepository repo, IMemoryCache cache, IMediaService mediaService) : IMenuItemService
{
    private readonly IMenuItemRepository _repo = repo;
    private readonly IMemoryCache _cache = cache;
    private readonly IMediaService _mediaService = mediaService;
    private const string CacheKey = "menu_items_all";

    public async Task<IEnumerable<MenuItemDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _cache.GetOrCreateAsync(CacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
            var items = await _repo.GetAllAsync(cancellationToken);
            return items.Select(i => new MenuItemDto(
                i.Id, 
                i.CategoryId, 
                i.Name,
                i.Description,
                i.Price,
                i.ImageUrl,
                i.IsActive, 
                i.CreatedAt, 
                i.Category?.Name,
                i.MenuItemAddons?.Select(ma => new Usecase.DTOs.Addon.AddonDto { Id = ma.Addon.Id, Name = ma.Addon.Name, Price = ma.Addon.Price, IsActive = ma.Addon.IsActive ?? false }).ToList()
            ));
        }) ?? Enumerable.Empty<MenuItemDto>();
    }

    public async Task<MenuItemDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var i = await _repo.GetByIdAsync(id, cancellationToken);
        if (i == null) return null;
        return new MenuItemDto(
            i.Id, 
            i.CategoryId, 
            i.Name,
            i.Description,
            i.Price,
            i.ImageUrl,
            i.IsActive, 
            i.CreatedAt, 
            i.Category?.Name,
            i.MenuItemAddons?.Select(ma => new Usecase.DTOs.Addon.AddonDto { Id = ma.Addon.Id, Name = ma.Addon.Name, Price = ma.Addon.Price, IsActive = ma.Addon.IsActive ?? false }).ToList()
        );
    }

    public async Task<MenuItemDto> CreateAsync(CreateMenuItemDto dto, CancellationToken cancellationToken = default)
    {
        var entity = new MenuItem 
        { 
            CategoryId = dto.CategoryId, 
            Name = dto.Name, 
            Price = dto.Price,
            ImageUrl = dto.ImageUrl,
            IsActive = dto.IsActive,
            MenuItemAddons = dto.AddonIds?.Select(aid => new MenuItemAddon { AddonId = aid, IsActive = true, CreatedAt = TimeUtils.GetVietnamTime() }).ToList() ?? new List<MenuItemAddon>()
        };
        var created = await _repo.AddAsync(entity, cancellationToken);
        
        if (!string.IsNullOrEmpty(dto.ImageUrl))
        {
            await _mediaService.LinkMediaToEntityAsync(dto.ImageUrl, "menu_item", created.Id);
        }

        _cache.Remove(CacheKey); // Invalidate cache
        return new MenuItemDto(
            created.Id, 
            created.CategoryId, 
            created.Name,
            created.Description,
            created.Price,
            created.ImageUrl,
            created.IsActive, 
            created.CreatedAt, 
            created.Category?.Name,
            new List<Usecase.DTOs.Addon.AddonDto>() 
        );
    }

    public async Task<bool> UpdateAsync(int id, CreateMenuItemDto dto, CancellationToken cancellationToken = default)
    {
        var existing = await _repo.GetByIdAsync(id, cancellationToken);
        if (existing == null) return false;
        
        var oldImageUrl = existing.ImageUrl;

        existing.CategoryId = dto.CategoryId;
        existing.Name = dto.Name;
        existing.Price = dto.Price;
        existing.ImageUrl = dto.ImageUrl;
        existing.IsActive = dto.IsActive;

        // Update addons
        if (dto.AddonIds != null)
        {
            existing.MenuItemAddons ??= new List<MenuItemAddon>();
            existing.MenuItemAddons.Clear();
            foreach (var aid in dto.AddonIds)
            {
                existing.MenuItemAddons.Add(new MenuItemAddon { AddonId = aid, IsActive = true, CreatedAt = TimeUtils.GetVietnamTime() });
            }
        }

        await _repo.UpdateAsync(existing, cancellationToken);
        
        if (!string.IsNullOrEmpty(dto.ImageUrl) && dto.ImageUrl != oldImageUrl)
        {
            await _mediaService.LinkMediaToEntityAsync(dto.ImageUrl, "menu_item", id);
        }

        _cache.Remove(CacheKey); // Invalidate cache
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await _repo.GetByIdAsync(id, cancellationToken);
        if (existing == null) return false;
        
        await _repo.DeleteAsync(existing, cancellationToken);
        
        // Cleanup media
        await _mediaService.DeleteMediaByEntityAsync("menu_item", id);

        _cache.Remove(CacheKey); // Invalidate cache
        return true;
    }
}





