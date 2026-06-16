using ChipmeoApis.Usecase.DTOs;
using ChipmeoApis.Usecase.DTOs.Blog;
using ChipmeoApis.Usecase.DTOs.Tag;

namespace ChipmeoApis.Usecase.Interfaces;

public interface IBlogService
{
    Task<List<BlogPostDto>> GetAllPostsAsync(string? status = null);
    Task<BlogPostDto?> GetPostBySlugAsync(string slug);
    Task<BlogPostDto?> GetPostByIdAsync(int id);
    Task<BlogPostDto> CreatePostAsync(CreateBlogPostDto dto, int authorId);
    Task<BlogPostDto?> UpdatePostAsync(int id, UpdateBlogPostDto dto);
    Task<bool> DeletePostAsync(int id);
}




