using ChipmeoApis.Usecase.Interfaces;
using ChipmeoApis.Usecase.DTOs.MenuItem;
using ChipmeoApis.Usecase.Utils;
using ChipmeoApis.Core.Constants;
using ChipmeoApis.Core.Entities;
using ChipmeoApis.Core.Utils;
using Microsoft.Extensions.Caching.Distributed;

namespace ChipmeoApis.Usecase.Services;

public class MenuItemService : IMenuItemService
{
    private readonly IMenuItemRepository _repo;
    private readonly IDistributedCache _cache;
    private readonly IMediaService _mediaService;

    public MenuItemService(IMenuItemRepository repo, IDistributedCache cache, IMediaService mediaService)
    {
        _repo = repo;
        _cache = cache;
        _mediaService = mediaService;
    }

    public async Task<IEnumerable<MenuItemDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _cache.GetOrSetAsync(CacheKeys.MenuItems.All, async () =>
        {
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
            )).ToList();
        }, TimeSpan.FromMinutes(10), cancellationToken);
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

        await _cache.RemoveAsync(CacheKeys.MenuItems.All, cancellationToken);
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

        await _cache.RemoveAsync(CacheKeys.MenuItems.All, cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await _repo.GetByIdAsync(id, cancellationToken);
        if (existing == null) return false;
        
        await _repo.DeleteAsync(existing, cancellationToken);
        await _mediaService.DeleteMediaByEntityAsync("menu_item", id);
        await _cache.RemoveAsync(CacheKeys.MenuItems.All, cancellationToken);
        return true;
    }
}
