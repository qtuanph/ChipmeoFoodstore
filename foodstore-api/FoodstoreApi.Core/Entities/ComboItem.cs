namespace FoodstoreApi.Core.Entities;

public partial class ComboItem : IAuditableEntity
{
    public Guid Id { get; set; }
    public Guid ComboId { get; set; }
    public Guid MenuItemId { get; set; }
    public int? Quantity { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }

    public virtual Combo Combo { get; set; } = null!;
    public virtual MenuItem MenuItem { get; set; } = null!;
}
