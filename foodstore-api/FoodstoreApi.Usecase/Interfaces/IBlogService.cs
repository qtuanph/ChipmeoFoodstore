using FoodstoreApi.Usecase.DTOs.Blog;

namespace FoodstoreApi.Usecase.Interfaces;

public interface IBlogService
{
    Task<List<BlogPostDto>> GetAllPostsAsync(string? status = null, string? categorySlug = null, string? tagSlug = null);
    Task<BlogPostDto?> GetPostBySlugAsync(string slug);
    Task<BlogPostDto?> GetPostByIdAsync(Guid id);
    Task<BlogPostDto> CreatePostAsync(CreateBlogPostDto dto, Guid authorId);
    Task<BlogPostDto?> UpdatePostAsync(Guid id, UpdateBlogPostDto dto);
    Task<bool> DeletePostAsync(Guid id);
    Task<BlogPostDto?> ChangeStatusAsync(Guid id, string status, Guid userId);
    Task<bool> SchedulePostAsync(Guid id, DateTime? scheduledAt);
    Task<BlogPostDto?> IncrementViewCountAsync(string slug);
    Task<CmsDashboardStatsDto> GetDashboardStatsAsync();
    Task<List<BlogPostDto>> GetFeaturedPostsAsync(int limit = 5);
    Task<List<BlogPostDto>> GetPublishedPostsAsync(int page = 1, int limit = 10, string? categorySlug = null, string? tagSlug = null);
    Task<int> GetTotalPublishedCountAsync(string? categorySlug = null, string? tagSlug = null);
}
