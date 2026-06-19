using FoodstoreApi.Usecase.DTOs.Blog;
using FoodstoreApi.Usecase.Interfaces;
using FoodstoreApi.Core.Entities;
using System.Text.RegularExpressions;

namespace FoodstoreApi.Usecase.Services;

public class BlogCategoryService : IBlogCategoryService
{
    private readonly IBlogCategoryRepository _repo;

    public BlogCategoryService(IBlogCategoryRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<BlogCategoryDto>> GetAllAsync(CancellationToken ct = default)
    {
        var categories = await _repo.GetAllAsync(ct);
        var list = new List<BlogCategoryDto>();
        foreach (var c in categories)
        {
            var dto = MapToDto(c);
            dto.PostCount = await _repo.GetPostCountAsync(c.Id, ct);
            list.Add(dto);
        }
        return list;
    }

    public async Task<BlogCategoryDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var category = await _repo.GetByIdAsync(id, ct);
        if (category == null) return null;
        var dto = MapToDto(category);
        dto.PostCount = await _repo.GetPostCountAsync(category.Id, ct);
        return dto;
    }

    public async Task<BlogCategoryDto> CreateAsync(CreateBlogCategoryDto dto, CancellationToken ct = default)
    {
        var category = new BlogCategory
        {
            Name = dto.Name,
            Slug = GenerateSlug(dto.Name),
            Description = dto.Description,
            ParentId = dto.ParentId,
            SortOrder = dto.SortOrder,
        };
        var created = await _repo.CreateAsync(category, ct);
        return MapToDto(created);
    }

    public async Task<BlogCategoryDto?> UpdateAsync(Guid id, UpdateBlogCategoryDto dto, CancellationToken ct = default)
    {
        var category = await _repo.GetByIdAsync(id, ct);
        if (category == null) return null;
        if (dto.Name != null) { category.Name = dto.Name; category.Slug = GenerateSlug(dto.Name); }
        if (dto.Description != null) category.Description = dto.Description;
        if (dto.ParentId.HasValue) category.ParentId = dto.ParentId;
        if (dto.SortOrder.HasValue) category.SortOrder = dto.SortOrder.Value;
        await _repo.UpdateAsync(category, ct);
        return MapToDto(category);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        return await _repo.DeleteAsync(id, ct);
    }

    private static BlogCategoryDto MapToDto(BlogCategory c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Slug = c.Slug,
        Description = c.Description,
        ParentId = c.ParentId,
        ParentName = c.Parent?.Name,
        SortOrder = c.SortOrder,
        CreatedAt = c.CreatedAt,
    };

    private static string GenerateSlug(string name)
    {
        var slug = RemoveVietnameseDiacritics(name.ToLowerInvariant());
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = Regex.Replace(slug, @"[^a-z0-9\-]", "");
        slug = Regex.Replace(slug, @"-+", "-");
        return slug.Trim('-');
    }

    private static string RemoveVietnameseDiacritics(string text)
    {
        var map = new[] {
            "aáàạảãâấầậẩẫăắằặẳẵ", "AÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
            "eéèẹẻẽêếềệểễ", "EÉÈẸẺẼÊẾỀỆỂỄ",
            "iíìịỉĩ", "IÍÌỊỈĨ",
            "oóòọỏõôốồộổỗơớờợởỡ", "OÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
            "uúùụủũưứừựửữ", "UÚÙỤỦŨƯỨỪỰỬỮ",
            "yýỳỵỷỹ", "YÝỲỴỶỸ",
            "dđ", "DĐ"
        };
        foreach (var chars in map)
        {
            var replacement = chars[0];
            foreach (var c in chars[1..])
                text = text.Replace(c, replacement);
        }
        return text;
    }
}
