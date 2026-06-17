using FoodstoreApi.Usecase.Interfaces;
using FoodstoreApi.Usecase.DTOs.Addon;
using FoodstoreApi.Usecase.Utils;
using FoodstoreApi.Core.Constants;
using FoodstoreApi.Core.Entities;
using FoodstoreApi.Core.Utils;
using Microsoft.Extensions.Caching.Distributed;

namespace FoodstoreApi.Usecase.Services;

public class AddonService : IAddonService
{
    private readonly IAddonRepository _repository;
    private readonly IDistributedCache _cache;

    public AddonService(IAddonRepository repository, IDistributedCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<IEnumerable<AddonDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _cache.GetOrSetAsync(CacheKeys.Addons.All, async () =>
        {
            var addons = await _repository.GetAllAsync(cancellationToken);
            return addons.Select(MapToDto).ToList();
        }, TimeSpan.FromMinutes(30), cancellationToken);
    }

    public async Task<AddonDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var addon = await _repository.GetByIdAsync(id, cancellationToken);
        return addon == null ? null : MapToDto(addon);
    }

    public async Task<AddonDto> CreateAsync(CreateAddonDto dto, CancellationToken cancellationToken = default)
    {
        var addon = new Addon
        {
            Name = dto.Name,
            Price = dto.Price,
            IsActive = dto.IsActive,
            CreatedAt = TimeUtils.GetVietnamTime()
        };

        var created = await _repository.CreateAsync(addon, cancellationToken);
        await _cache.RemoveAsync(CacheKeys.Addons.All, cancellationToken);
        await _cache.RemoveAsync(CacheKeys.MenuItems.All, cancellationToken);
        return MapToDto(created);
    }

    public async Task<bool> UpdateAsync(int id, CreateAddonDto dto, CancellationToken cancellationToken = default)
    {
        var addon = await _repository.GetByIdAsync(id, cancellationToken);
        if (addon == null) return false;

        addon.Name = dto.Name;
        addon.Price = dto.Price;
        addon.IsActive = dto.IsActive;

        var result = await _repository.UpdateAsync(addon, cancellationToken);
        if (result)
        {
            await _cache.RemoveAsync(CacheKeys.Addons.All, cancellationToken);
            await _cache.RemoveAsync(CacheKeys.Addons.ById(id), cancellationToken);
            await _cache.RemoveAsync(CacheKeys.MenuItems.All, cancellationToken);
        }
        return result;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var result = await _repository.DeleteAsync(id, cancellationToken);
        if (result)
        {
            await _cache.RemoveAsync(CacheKeys.Addons.All, cancellationToken);
            await _cache.RemoveAsync(CacheKeys.Addons.ById(id), cancellationToken);
            await _cache.RemoveAsync(CacheKeys.MenuItems.All, cancellationToken);
        }
        return result;
    }

    private static AddonDto MapToDto(Addon addon)
    {
        return new AddonDto
        {
            Id = addon.Id,
            Name = addon.Name,
            Price = addon.Price,
            IsActive = addon.IsActive ?? true,
            CreatedAt = addon.CreatedAt ?? TimeUtils.GetVietnamTime()
        };
    }
}
