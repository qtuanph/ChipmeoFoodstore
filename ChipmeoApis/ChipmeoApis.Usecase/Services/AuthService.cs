using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ChipmeoApis.Core.Configuration;
using ChipmeoApis.Core.Utils;
using ChipmeoApis.Usecase.DTOs.Auth;
using ChipmeoApis.Usecase.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ChipmeoApis.Usecase.Services;

public class AuthService(IEmployeeRepository employeeRepository, IOptions<JwtSettings> jwtOptions, IMediaService mediaService) : IAuthService
{
    private readonly IEmployeeRepository _employeeRepository = employeeRepository;
    private readonly JwtSettings _jwtSettings = jwtOptions.Value;
    private readonly IMediaService _mediaService = mediaService;

    public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        // Find user by username
        var employee = await _employeeRepository.GetByUsernameAsync(request.Username, cancellationToken);

        if (employee == null || employee.IsActive != true)
            return null;

        // Verify password (BCrypt)
        if (!BCrypt.Net.BCrypt.Verify(request.Password, employee.PasswordHash))
            return null;

        // Update last login
        employee.LastLogin = TimeUtils.GetVietnamTime();
        await _employeeRepository.UpdateAsync(employee, cancellationToken);

        // Get permissions
        var permissions = employee.Role?.RolePermissions
            .Where(rp => rp.IsActive)
            .Select(rp => rp.Permission.Code)
            .ToList() ?? new List<string>();
            
        // Generate JWT token
        var token = GenerateJwtToken(employee, permissions);
        var expiresIn = _jwtSettings.ExpiryInHours * 3600;

        return new LoginResponse
        {
            Token = token,
            RefreshToken = null, // Implement refresh token if needed
            ExpiresIn = expiresIn,
            User = new UserInfo
            {
                Id = employee.Id,
                FullName = employee.FullName ?? employee.Username,
                Username = employee.Username,
                Email = employee.Email,
                AvatarUrl = employee.AvatarUrl,
                RoleId = employee.RoleId,
                RoleName = employee.Role?.Name ?? "Unknown",
                DefaultRoute = employee.Role?.DefaultRoute,
                Permissions = permissions
            }
        };
    }

    public Task<TokenResponse?> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Refresh token not yet implemented");
    }

    public Task<bool> ValidateTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public async Task<UserDto?> GetProfileAsync(int userId, CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdAsync(userId, cancellationToken);

        if (employee == null) return null;

        return new UserDto
        {
            Id = employee.Id,
            Username = employee.Username,
            FullName = employee.FullName ?? employee.Username,
            Email = employee.Email,
            Phone = employee.Phone,
            AvatarUrl = employee.AvatarUrl,
            RoleName = employee.Role?.Name ?? "Unknown"
        };
    }

    public async Task<UserDto?> UpdateProfileAsync(int userId, UpdateProfileDto dto, CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdAsync(userId, cancellationToken);

        if (employee == null) return null;
        
        // Update basic info
        employee.FullName = dto.FullName;
        employee.Phone = dto.Phone;
        employee.Email = dto.Email;
        employee.AvatarUrl = dto.AvatarUrl; // Update AvatarUrl

        // Update password if provided
        if (!string.IsNullOrEmpty(dto.NewPassword))
        {
            if (string.IsNullOrEmpty(dto.CurrentPassword))
            {
                throw new ArgumentException("Current password is required to change password");
            }

            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, employee.PasswordHash))
            {
                throw new ArgumentException("Current password is incorrect");
            }

            employee.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        }

        await _employeeRepository.UpdateAsync(employee, cancellationToken);

        if (!string.IsNullOrEmpty(dto.AvatarUrl) && dto.AvatarUrl != employee.AvatarUrl)
             await _mediaService.LinkMediaToEntityAsync(dto.AvatarUrl, "employee", userId);
             
        return new UserDto
        {
            Id = employee.Id,
            Username = employee.Username,
            FullName = employee.FullName ?? employee.Username,
            Email = employee.Email,
            Phone = employee.Phone,
            AvatarUrl = employee.AvatarUrl,
            RoleName = employee.Role?.Name ?? "Unknown"
        };
    }

    private string GenerateJwtToken(Core.Entities.Employee employee, List<string> permissions)
    {
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, employee.Id.ToString()),
            new(ClaimTypes.Name, employee.Username),
            new(ClaimTypes.GivenName, employee.FullName ?? employee.Username),
            new(ClaimTypes.Role, employee.Role?.Name ?? "User"),
            new("RoleId", employee.RoleId.ToString()),
        };

        // Add permissions as claims
        foreach (var permission in permissions)
        {
            claims.Add(new Claim("permission", permission));
        }

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: TimeUtils.GetVietnamTime().AddHours(_jwtSettings.ExpiryInHours),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}





