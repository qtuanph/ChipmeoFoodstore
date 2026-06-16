namespace ChipmeoApis.Core.Entities;

public partial class Combo
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal ComboPrice { get; set; }

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<ComboItem> ComboItems { get; set; } = new List<ComboItem>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}




