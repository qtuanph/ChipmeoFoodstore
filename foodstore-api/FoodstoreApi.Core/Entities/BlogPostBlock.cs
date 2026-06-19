namespace FoodstoreApi.Core.Entities;

public class BlogPostBlock
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public string BlockType { get; set; } = null!;
    public string BlockData { get; set; } = "{}";
    public int SortOrder { get; set; }

    public virtual BlogPost Post { get; set; } = null!;
}
