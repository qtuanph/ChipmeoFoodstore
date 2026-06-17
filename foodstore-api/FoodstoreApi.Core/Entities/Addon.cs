namespace FoodstoreApi.Core.Entities;

public partial class Addon
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<MenuItemAddon> MenuItemAddons { get; set; } = new List<MenuItemAddon>();

    public virtual ICollection<OrderItemAddon> OrderItemAddons { get; set; } = new List<OrderItemAddon>();
}




