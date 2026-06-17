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
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? AvatarUrl { get; set; }
    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string? DefaultRoute { get; set; }
    public List<string> Permissions { get; set; } = new();
}




