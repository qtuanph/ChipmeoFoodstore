using FoodstoreApi.Usecase.DTOs;
using FoodstoreApi.Usecase.DTOs.Blog;
using FoodstoreApi.Usecase.DTOs.Tag;

namespace FoodstoreApi.Usecase.Interfaces;

public interface IBlogService
{
    Task<List<BlogPostDto>> GetAllPostsAsync(string? status = null);
    Task<BlogPostDto?> GetPostBySlugAsync(string slug);
    Task<BlogPostDto?> GetPostByIdAsync(int id);
    Task<BlogPostDto> CreatePostAsync(CreateBlogPostDto dto, int authorId);
    Task<BlogPostDto?> UpdatePostAsync(int id, UpdateBlogPostDto dto);
    Task<bool> DeletePostAsync(int id);
}




