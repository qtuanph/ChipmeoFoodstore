namespace FoodstoreApi.Core.Entities;

public class BlogPostCategory
{
    public Guid PostId { get; set; }
    public Guid CategoryId { get; set; }

    public virtual BlogPost Post { get; set; } = null!;
    public virtual BlogCategory Category { get; set; } = null!;
}
