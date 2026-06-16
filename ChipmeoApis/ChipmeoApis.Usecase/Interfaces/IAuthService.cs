using ChipmeoApis.Usecase.DTOs.Auth;

namespace ChipmeoApis.Usecase.Interfaces;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<TokenResponse?> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<bool> ValidateTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<UserDto?> GetProfileAsync(int userId, CancellationToken cancellationToken = default);
    Task<UserDto?> UpdateProfileAsync(int userId, UpdateProfileDto dto, CancellationToken cancellationToken = default);
}




