namespace ChipmeoApis.Core.Entities;

public partial class MenuItemAddon
{
    public int Id { get; set; }

    public int MenuItemId { get; set; }

    public int AddonId { get; set; }

    public decimal? PriceOverride { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Addon Addon { get; set; } = null!;

    public virtual MenuItem MenuItem { get; set; } = null!;
}




