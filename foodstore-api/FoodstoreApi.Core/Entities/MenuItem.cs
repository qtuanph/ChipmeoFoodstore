namespace FoodstoreApi.Core.Entities;

public partial class MenuItem
{
    public int Id { get; set; }

    public int? CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public string? ImageUrl { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<ComboItem> ComboItems { get; set; } = new List<ComboItem>();

    public virtual ICollection<MenuItemAddon> MenuItemAddons { get; set; } = new List<MenuItemAddon>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}




