namespace FoodstoreApi.Core.Entities;

public partial class MenuItemAddon : IAuditableEntity
{
    public Guid Id { get; set; }
    public Guid MenuItemId { get; set; }
    public Guid AddonId { get; set; }
    public decimal? PriceOverride { get; set; }
    public bool? IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }

    public virtual Addon Addon { get; set; } = null!;
    public virtual MenuItem MenuItem { get; set; } = null!;
}
