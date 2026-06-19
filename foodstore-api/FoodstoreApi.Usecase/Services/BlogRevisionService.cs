using FoodstoreApi.Usecase.DTOs.Blog;
using FoodstoreApi.Usecase.Interfaces;
using FoodstoreApi.Core.Entities;
using System.Text.Json;

namespace FoodstoreApi.Usecase.Services;

public class BlogRevisionService : IBlogRevisionService
{
    private readonly IBlogRevisionRepository _repo;
    private readonly IBlogRepository _blogRepo;
    private readonly IBlogBlockRepository _blockRepo;

    public BlogRevisionService(IBlogRevisionRepository repo, IBlogRepository blogRepo, IBlogBlockRepository blockRepo)
    {
        _repo = repo;
        _blogRepo = blogRepo;
        _blockRepo = blockRepo;
    }

    public async Task<List<BlogPostRevisionDto>> GetByPostIdAsync(Guid postId, CancellationToken ct = default)
    {
        var revisions = await _repo.GetByPostIdAsync(postId, ct);
        return revisions.Select(MapToDto).ToList();
    }

    public async Task<BlogPostRevisionDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var revision = await _repo.GetByIdAsync(id, ct);
        return revision == null ? null : MapToDto(revision);
    }

    public async Task<BlogPostRevisionDto> CreateSnapshotAsync(Guid postId, Guid? userId, CancellationToken ct = default)
    {
        var post = await _blogRepo.GetByIdAsync(postId);
        if (post == null) throw new KeyNotFoundException("Post not found");

        var blocks = await _blockRepo.GetByPostIdAsync(postId, ct);
        var blocksJson = JsonSerializer.Serialize(blocks.Select(b => new
        {
            b.BlockType,
            b.BlockData,
            b.SortOrder
        }));

        var revision = new BlogPostRevision
        {
            PostId = postId,
            Title = post.Title,
            Content = post.Content,
            Excerpt = post.Excerpt,
            ThumbnailUrl = post.ThumbnailUrl,
            Status = post.Status,
            MetaTitle = post.MetaTitle,
            MetaDescription = post.MetaDescription,
            FocusKeyword = post.FocusKeyword,
            Keywords = post.Keywords,
            CanonicalUrl = post.CanonicalUrl,
            OgImageUrl = post.OgImageUrl,
            WordCount = post.WordCount,
            ReadingTime = post.ReadingTime,
            BlocksJson = blocksJson,
            CreatedBy = userId,
        };

        var created = await _repo.CreateAsync(revision, ct);
        return MapToDto(created);
    }

    public async Task<BlogPostDto?> RestoreAsync(Guid revisionId, CancellationToken ct = default)
    {
        var revision = await _repo.GetByIdAsync(revisionId, ct);
        if (revision == null) return null;

        var post = await _blogRepo.GetByIdAsync(revision.PostId);
        if (post == null) return null;

        post.Title = revision.Title;
        post.Content = revision.Content;
        post.Excerpt = revision.Excerpt;
        post.ThumbnailUrl = revision.ThumbnailUrl;
        post.Status = revision.Status;
        post.MetaTitle = revision.MetaTitle;
        post.MetaDescription = revision.MetaDescription;
        post.FocusKeyword = revision.FocusKeyword;
        post.Keywords = revision.Keywords;
        post.CanonicalUrl = revision.CanonicalUrl;
        post.OgImageUrl = revision.OgImageUrl;
        post.WordCount = revision.WordCount;
        post.ReadingTime = revision.ReadingTime;

        await _blogRepo.UpdateAsync(post);

        if (!string.IsNullOrEmpty(revision.BlocksJson))
        {
            var blocks = JsonSerializer.Deserialize<List<CreateBlogBlockDto>>(revision.BlocksJson);
            if (blocks != null)
            {
                var blockEntities = blocks.Select((b, i) => new BlogPostBlock
                {
                    PostId = revision.PostId,
                    BlockType = b.BlockType,
                    BlockData = b.BlockData,
                    SortOrder = b.SortOrder
                }).ToList();
                await _blockRepo.SetPostBlocksAsync(revision.PostId, blockEntities, ct);
            }
        }

        var loaded = await _blogRepo.GetByIdAsync(post.Id);
        return loaded == null ? null : MapToFullDto(loaded);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        return await _repo.DeleteAsync(id, ct);
    }

    private static BlogPostRevisionDto MapToDto(BlogPostRevision r) => new()
    {
        Id = r.Id,
        PostId = r.PostId,
        Title = r.Title,
        Content = r.Content,
        Excerpt = r.Excerpt,
        ThumbnailUrl = r.ThumbnailUrl,
        Status = r.Status,
        BlocksJson = r.BlocksJson,
        CreatedBy = r.CreatedBy,
        CreatedAt = r.CreatedAt,
    };

    private static BlogPostDto MapToFullDto(BlogPost post) => BlogService.MapToDto(post);
}
