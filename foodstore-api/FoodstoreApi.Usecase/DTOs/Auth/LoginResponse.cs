namespace FoodstoreApi.Usecase.DTOs.Auth;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string? RefreshToken { get; set; }
    public UserInfo User { get; set; } = null!;
    public int ExpiresIn { get; set; }
}

public class UserInfo
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string EmployeeCode { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string? DefaultRoute { get; set; }
    public List<string> Permissions { get; set; } = new();
}
