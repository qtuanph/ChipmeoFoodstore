namespace FoodstoreApi.Usecase.DTOs.Auth;

public class UpdateProfileDto
{
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? AvatarUrl { get; set; }
    public string? CurrentPassword { get; set; }
    public string? NewPassword { get; set; }
}
