using Microsoft.AspNetCore.Identity;

namespace FoodstoreApi.Core.Entities.Identity;

public class ApplicationRole : IdentityRole<Guid>, IAuditableEntity
{
    public string? Description { get; set; }
    public string? DefaultRoute { get; set; }
    public bool IsSystem { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
