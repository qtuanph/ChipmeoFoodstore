using FoodstoreApi.Core.Entities.Identity;

namespace FoodstoreApi.Core.Entities;

public partial class Customer : IAuditableEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string CustomerCode { get; set; } = null!;
    public string? Phone { get; set; }
    public string? AvatarUrl { get; set; }
    public int LoyaltyPoints { get; set; }
    public string? MembershipLevel { get; set; }
    public DateTime? Birthday { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }

    public virtual ApplicationUser User { get; set; } = null!;
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
