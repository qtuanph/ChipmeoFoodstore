using FoodstoreApi.Usecase.DTOs.Blog;
using FoodstoreApi.Usecase.Interfaces;
using FoodstoreApi.Core.Entities;

namespace FoodstoreApi.Usecase.Services;

public class BlogBlockService : IBlogBlockService
{
    private readonly IBlogBlockRepository _repo;

    public BlogBlockService(IBlogBlockRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<BlogPostBlockDto>> GetByPostIdAsync(Guid postId, CancellationToken ct = default)
    {
        var blocks = await _repo.GetByPostIdAsync(postId, ct);
        return blocks.Select(MapToDto).ToList();
    }

    public async Task<BlogPostBlockDto> CreateAsync(Guid postId, CreateBlogBlockDto dto, CancellationToken ct = default)
    {
        var block = new BlogPostBlock
        {
            PostId = postId,
            BlockType = dto.BlockType,
            BlockData = dto.BlockData,
            SortOrder = dto.SortOrder,
        };
        var created = await _repo.CreateAsync(block, ct);
        return MapToDto(created);
    }

    public async Task<bool> UpdateAsync(Guid blockId, UpdateBlogBlockDto dto, CancellationToken ct = default)
    {
        var block = await _repo.GetByIdAsync(blockId, ct);
        if (block == null) return false;
        if (dto.BlockType != null) block.BlockType = dto.BlockType;
        if (dto.BlockData != null) block.BlockData = dto.BlockData;
        if (dto.SortOrder.HasValue) block.SortOrder = dto.SortOrder.Value;
        return await _repo.UpdateAsync(block, ct);
    }

    public async Task<bool> DeleteAsync(Guid blockId, CancellationToken ct = default)
    {
        return await _repo.DeleteAsync(blockId, ct);
    }

    public async Task<List<BlogPostBlockDto>> SetPostBlocksAsync(Guid postId, List<CreateBlogBlockDto> blocks, CancellationToken ct = default)
    {
        var entities = blocks.Select((b, i) => new BlogPostBlock
        {
            PostId = postId,
            BlockType = b.BlockType,
            BlockData = b.BlockData,
            SortOrder = b.SortOrder != 0 ? b.SortOrder : i,
        }).ToList();
        await _repo.SetPostBlocksAsync(postId, entities, ct);
        var saved = await _repo.GetByPostIdAsync(postId, ct);
        return saved.Select(MapToDto).ToList();
    }

    private static BlogPostBlockDto MapToDto(BlogPostBlock b) => new()
    {
        Id = b.Id,
        PostId = b.PostId,
        BlockType = b.BlockType,
        BlockData = b.BlockData,
        SortOrder = b.SortOrder,
    };
}
