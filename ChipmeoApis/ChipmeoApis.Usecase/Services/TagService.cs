using ChipmeoApis.Usecase.DTOs.Tag;
using ChipmeoApis.Usecase.Interfaces;
using ChipmeoApis.Core.Entities;
using System.Text.RegularExpressions;

namespace ChipmeoApis.Usecase.Services;

public interface ITagService
{
    Task<IEnumerable<TagDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TagDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TagDto?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<TagDto> CreateAsync(CreateTagDto dto, CancellationToken cancellationToken = default);
    Task<TagDto?> UpdateAsync(int id, UpdateTagDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TagDto>> GetByPostIdAsync(int postId, CancellationToken cancellationToken = default);
    Task SetPostTagsAsync(int postId, IEnumerable<int> tagIds, CancellationToken cancellationToken = default);
}

public class TagService : ITagService
{
    private readonly ITagRepository _repository;

    public TagService(ITagRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TagDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var tags = await _repository.GetAllAsync(cancellationToken);
        return tags.Select(MapToDto);
    }

    public async Task<TagDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var tag = await _repository.GetByIdAsync(id, cancellationToken);
        return tag != null ? MapToDto(tag) : null;
    }

    public async Task<TagDto?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var tag = await _repository.GetBySlugAsync(slug, cancellationToken);
        return tag != null ? MapToDto(tag) : null;
    }

    public async Task<TagDto> CreateAsync(CreateTagDto dto, CancellationToken cancellationToken = default)
    {
        var tag = new Tag
        {
            Name = dto.Name,
            Slug = GenerateSlug(dto.Name),
            Description = dto.Description,
            Color = dto.Color ?? "#f59e0b",
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repository.CreateAsync(tag, cancellationToken);
        return MapToDto(created);
    }

    public async Task<TagDto?> UpdateAsync(int id, UpdateTagDto dto, CancellationToken cancellationToken = default)
    {
        var tag = await _repository.GetByIdAsync(id, cancellationToken);
        if (tag == null) return null;

        if (dto.Name != null)
        {
            tag.Name = dto.Name;
            tag.Slug = GenerateSlug(dto.Name);
        }
        if (dto.Description != null) tag.Description = dto.Description;
        if (dto.Color != null) tag.Color = dto.Color;

        await _repository.UpdateAsync(tag, cancellationToken);
        return MapToDto(tag);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _repository.DeleteAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<TagDto>> GetByPostIdAsync(int postId, CancellationToken cancellationToken = default)
    {
        var tags = await _repository.GetByPostIdAsync(postId, cancellationToken);
        return tags.Select(MapToDto);
    }

    public async Task SetPostTagsAsync(int postId, IEnumerable<int> tagIds, CancellationToken cancellationToken = default)
    {
        await _repository.SetPostTagsAsync(postId, tagIds, cancellationToken);
    }

    private static TagDto MapToDto(Tag tag)
    {
        return new TagDto(
            tag.Id,
            tag.Name,
            tag.Slug,
            tag.Description,
            tag.Color,
            tag.BlogPostTags?.Count ?? 0
        );
    }

    private static string GenerateSlug(string name)
    {
        // Convert to lowercase
        var slug = name.ToLowerInvariant();
        // Replace Vietnamese characters
        slug = RemoveVietnameseDiacritics(slug);
        // Replace spaces with hyphens
        slug = Regex.Replace(slug, @"\s+", "-");
        // Remove invalid characters
        slug = Regex.Replace(slug, @"[^a-z0-9\-]", "");
        // Remove multiple hyphens
        slug = Regex.Replace(slug, @"-+", "-");
        // Trim hyphens from ends
        slug = slug.Trim('-');
        return slug;
    }

    private static string RemoveVietnameseDiacritics(string text)
    {
        string[] vietnameseChars = new string[]
        {
            "aáàạảãâấầậẩẫăắằặẳẵ",
            "AÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
            "eéèẹẻẽêếềệểễ",
            "EÉÈẸẺẼÊẾỀỆỂỄ",
            "iíìịỉĩ",
            "IÍÌỊỈĨ",
            "oóòọỏõôốồộổỗơớờợởỡ",
            "OÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
            "uúùụủũưứừựửữ",
            "UÚÙỤỦŨƯỨỪỰỬỮ",
            "yýỳỵỷỹ",
            "YÝỲỴỶỸ",
            "dđ",
            "DĐ"
        };

        foreach (string chars in vietnameseChars)
        {
            char replacement = chars[0];
            foreach (char c in chars.Substring(1))
            {
                text = text.Replace(c, replacement);
            }
        }
        return text;
    }
}




