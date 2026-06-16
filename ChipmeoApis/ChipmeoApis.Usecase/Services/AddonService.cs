using ChipmeoApis.Usecase.Interfaces;
using ChipmeoApis.Usecase.DTOs.Addon;
using ChipmeoApis.Core.Entities;
using ChipmeoApis.Core.Utils;

namespace ChipmeoApis.Usecase.Services;

public class AddonService(IAddonRepository repository) : IAddonService
{
    private readonly IAddonRepository _repository = repository;

    public async Task<IEnumerable<AddonDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var addons = await _repository.GetAllAsync(cancellationToken);
        return addons.Select(MapToDto);
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
        return MapToDto(created);
    }

    public async Task<bool> UpdateAsync(int id, CreateAddonDto dto, CancellationToken cancellationToken = default)
    {
        var addon = await _repository.GetByIdAsync(id, cancellationToken);
        if (addon == null) return false;

        addon.Name = dto.Name;
        addon.Price = dto.Price;
        addon.IsActive = dto.IsActive;

        return await _repository.UpdateAsync(addon, cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _repository.DeleteAsync(id, cancellationToken);
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




