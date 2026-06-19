using FoodstoreApi.Usecase.DTOs.Auth;

namespace FoodstoreApi.Usecase.Interfaces;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<TokenResponse?> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<bool> ValidateTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<UserDto?> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserDto?> UpdateProfileAsync(Guid userId, UpdateProfileDto dto, CancellationToken cancellationToken = default);
}
