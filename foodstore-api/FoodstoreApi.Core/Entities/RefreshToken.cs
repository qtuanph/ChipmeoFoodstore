using FoodstoreApi.Core.Entities.Identity;

namespace FoodstoreApi.Core.Entities;

public class RefreshToken : IAuditableEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    public DateTime? RevokedAt { get; set; }

    public virtual ApplicationUser User { get; set; } = null!;
}
