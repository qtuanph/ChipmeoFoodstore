namespace FoodstoreApi.Core.Entities;

public class BlogCategory : IAuditableEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Description { get; set; }
    public Guid? ParentId { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }

    public virtual BlogCategory? Parent { get; set; }
    public virtual ICollection<BlogCategory> Children { get; set; } = new List<BlogCategory>();
    public virtual ICollection<BlogPostCategory> BlogPostCategories { get; set; } = new List<BlogPostCategory>();
}
