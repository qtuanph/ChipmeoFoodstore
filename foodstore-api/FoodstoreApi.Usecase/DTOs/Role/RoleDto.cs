namespace FoodstoreApi.Usecase.DTOs.Role;

public class RoleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? DefaultRoute { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }

    public List<string> PermissionCodes { get; set; } = new();
}

public class CreateRoleDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? DefaultRoute { get; set; }
    public bool IsActive { get; set; } = true;
}

public class AssignPermissionsDto
{
    public List<string> PermissionCodes { get; set; } = new();
}
