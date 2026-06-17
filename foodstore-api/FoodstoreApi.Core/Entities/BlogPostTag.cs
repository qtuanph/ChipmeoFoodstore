namespace FoodstoreApi.Core.Entities;

public class BlogPostTag
{
    public int PostId { get; set; }
    public int TagId { get; set; }

    // Navigation
    public virtual BlogPost Post { get; set; } = null!;
    public virtual Tag Tag { get; set; } = null!;
}




