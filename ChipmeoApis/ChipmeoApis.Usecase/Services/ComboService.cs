using ChipmeoApis.Usecase.Interfaces;
using ChipmeoApis.Usecase.DTOs.Combo;
using ChipmeoApis.Usecase.Utils;
using ChipmeoApis.Core.Constants;
using ChipmeoApis.Core.Entities;
using ChipmeoApis.Core.Utils;
using Microsoft.Extensions.Caching.Distributed;

namespace ChipmeoApis.Usecase.Services;

public class ComboService : IComboService
{
    private readonly IComboRepository _repository;
    private readonly IMediaService _mediaService;
    private readonly IDistributedCache _cache;

    public ComboService(IComboRepository repository, IMediaService mediaService, IDistributedCache cache)
    {
        _repository = repository;
        _mediaService = mediaService;
        _cache = cache;
    }

    public async Task<IEnumerable<ComboDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _cache.GetOrSetAsync(CacheKeys.Combos.All, async () =>
        {
            var combos = await _repository.GetAllAsync(cancellationToken);
            return combos.Select(MapToDto).ToList();
        }, TimeSpan.FromMinutes(30), cancellationToken);
    }

    public async Task<ComboDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var combo = await _repository.GetByIdAsync(id, cancellationToken);
        return combo == null ? null : MapToDto(combo);
    }

    public async Task<ComboDto> CreateAsync(CreateComboDto dto, CancellationToken cancellationToken = default)
    {
        var combo = new Combo
        {
            Name = dto.Name,
            ComboPrice = dto.ComboPrice,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl,
            IsActive = dto.IsActive,
            CreatedAt = TimeUtils.GetVietnamTime(),
            ComboItems = dto.Items.Select(i => new ComboItem
            {
                MenuItemId = i.MenuItemId,
                Quantity = i.Quantity,
                CreatedAt = TimeUtils.GetVietnamTime()
            }).ToList()
        };

        var created = await _repository.CreateAsync(combo, cancellationToken);
        
        if (!string.IsNullOrEmpty(dto.ImageUrl))
        {
            await _mediaService.LinkMediaToEntityAsync(dto.ImageUrl, "combo", created.Id);
        }

        await _cache.RemoveAsync(CacheKeys.Combos.All, cancellationToken);
        return MapToDto(created);
    }

    public async Task<bool> UpdateAsync(int id, CreateComboDto dto, CancellationToken cancellationToken = default)
    {
        var combo = await _repository.GetByIdAsync(id, cancellationToken);
        if (combo == null) return false;

        var oldImageUrl = combo.ImageUrl;

        combo.Name = dto.Name;
        combo.ComboPrice = dto.ComboPrice;
        combo.Description = dto.Description;
        combo.ImageUrl = dto.ImageUrl;
        combo.IsActive = dto.IsActive;

        var newItems = dto.Items.Select(i => new ComboItem
        {
            ComboId = id,
            MenuItemId = i.MenuItemId,
            Quantity = i.Quantity,
            CreatedAt = TimeUtils.GetVietnamTime()
        }).ToList();

        var result = await _repository.UpdateWithItemsAsync(combo, newItems, cancellationToken);
        
        if (result)
        {
            if (!string.IsNullOrEmpty(dto.ImageUrl) && dto.ImageUrl != oldImageUrl)
            {
                await _mediaService.LinkMediaToEntityAsync(dto.ImageUrl, "combo", id);
            }
            await _cache.RemoveAsync(CacheKeys.Combos.All, cancellationToken);
            await _cache.RemoveAsync(CacheKeys.Combos.ById(id), cancellationToken);
        }
        
        return result;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var result = await _repository.DeleteAsync(id, cancellationToken);
        if (result)
        {
            await _cache.RemoveAsync(CacheKeys.Combos.All, cancellationToken);
            await _cache.RemoveAsync(CacheKeys.Combos.ById(id), cancellationToken);
        }
        return result;
    }

    private static ComboDto MapToDto(Combo combo)
    {
        return new ComboDto
        {
            Id = combo.Id,
            Name = combo.Name,
            ComboPrice = combo.ComboPrice,
            Description = combo.Description,
            ImageUrl = combo.ImageUrl,
            IsActive = combo.IsActive ?? true,
            CreatedAt = combo.CreatedAt ?? TimeUtils.GetVietnamTime(),
            Items = combo.ComboItems?.Select(ci => new ComboItemDto
            {
                Id = ci.Id,
                MenuItemId = ci.MenuItemId,
                MenuItemName = ci.MenuItem?.Name ?? "",
                Quantity = ci.Quantity ?? 0
            }).ToList() ?? new List<ComboItemDto>()
        };
    }
}
