using ChipmeoApis.Usecase.Interfaces;
using ChipmeoApis.Usecase.DTOs.Discount;
using ChipmeoApis.Usecase.Utils;
using ChipmeoApis.Core.Constants;
using ChipmeoApis.Core.Entities;
using ChipmeoApis.Core.Utils;
using Microsoft.Extensions.Caching.Distributed;

namespace ChipmeoApis.Usecase.Services;

public class DiscountService : IDiscountService
{
    private readonly IDiscountRepository _repository;
    private readonly IDistributedCache _cache;

    public DiscountService(IDiscountRepository repository, IDistributedCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<IEnumerable<DiscountDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _cache.GetOrSetAsync(CacheKeys.Discounts.All, async () =>
        {
            var discounts = await _repository.GetAllAsync(cancellationToken);
            return discounts.Select(MapToDto).ToList();
        }, TimeSpan.FromMinutes(5), cancellationToken);
    }

    public async Task<DiscountDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var discount = await _repository.GetByIdAsync(id, cancellationToken);
        return discount == null ? null : MapToDto(discount);
    }

    public async Task<DiscountDto> CreateAsync(CreateDiscountDto dto, CancellationToken cancellationToken = default)
    {
        var discount = new Discount
        {
            Code = dto.Code.ToUpper(),
            Name = dto.Name,
            Type = dto.Type,
            Value = dto.Value,
            MaxDiscountAmount = dto.MaxDiscountAmount,
            MinOrderAmount = dto.MinOrderAmount,
            UsageLimit = dto.UsageLimit,
            UsedCount = 0,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            IsActive = dto.IsActive,
            CreatedAt = TimeUtils.GetVietnamTime()
        };

        var created = await _repository.CreateAsync(discount, cancellationToken);
        await _cache.RemoveAsync(CacheKeys.Discounts.All, cancellationToken);
        return MapToDto(created);
    }

    public async Task<bool> UpdateAsync(int id, CreateDiscountDto dto, CancellationToken cancellationToken = default)
    {
        var discount = await _repository.GetByIdAsync(id, cancellationToken);
        if (discount == null) return false;

        discount.Code = dto.Code.ToUpper();
        discount.Name = dto.Name;
        discount.Type = dto.Type;
        discount.Value = dto.Value;
        discount.MaxDiscountAmount = dto.MaxDiscountAmount;
        discount.MinOrderAmount = dto.MinOrderAmount;
        discount.UsageLimit = dto.UsageLimit;
        discount.StartDate = dto.StartDate;
        discount.EndDate = dto.EndDate;
        discount.IsActive = dto.IsActive;

        var result = await _repository.UpdateAsync(discount, cancellationToken);
        if (result)
        {
            await _cache.RemoveAsync(CacheKeys.Discounts.All, cancellationToken);
            await _cache.RemoveAsync(CacheKeys.Discounts.ById(id), cancellationToken);
        }
        return result;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var result = await _repository.DeleteAsync(id, cancellationToken);
        if (result)
        {
            await _cache.RemoveAsync(CacheKeys.Discounts.All, cancellationToken);
            await _cache.RemoveAsync(CacheKeys.Discounts.ById(id), cancellationToken);
        }
        return result;
    }

    private static DiscountDto MapToDto(Discount discount)
    {
        return new DiscountDto
        {
            Id = discount.Id,
            Code = discount.Code,
            Name = discount.Name,
            Type = discount.Type,
            Value = discount.Value,
            MaxDiscountAmount = discount.MaxDiscountAmount,
            MinOrderAmount = discount.MinOrderAmount,
            UsageLimit = discount.UsageLimit,
            UsedCount = discount.UsedCount ?? 0,
            StartDate = discount.StartDate,
            EndDate = discount.EndDate,
            IsActive = discount.IsActive ?? true,
            CreatedAt = discount.CreatedAt ?? TimeUtils.GetVietnamTime()
        };
    }
}
