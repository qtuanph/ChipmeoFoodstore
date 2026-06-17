namespace FoodstoreApi.Usecase.DTOs.Role;

public class RoleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? DefaultRoute { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public List<PermissionDto> Permissions { get; set; } = new();
}

public class PermissionDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Module { get; set; } = string.Empty;
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
    public List<int> PermissionIds { get; set; } = new();
}




