namespace FoodstoreApi.Core.Entities;

public class BlogPostTag : IAuditableEntity
{
    public Guid PostId { get; set; }
    public Guid TagId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }

    public virtual BlogPost Post { get; set; } = null!;
    public virtual Tag Tag { get; set; } = null!;
}
