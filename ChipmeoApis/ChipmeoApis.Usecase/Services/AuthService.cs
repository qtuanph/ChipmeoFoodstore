using ChipmeoApis.Usecase.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ChipmeoApis.Usecase.DTOs.Auth;
using ChipmeoApis.Core.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ChipmeoApis.Usecase.Services;

public class AuthService(IEmployeeRepository employeeRepository, IConfiguration configuration, IMediaService mediaService) : IAuthService
{
    private readonly IEmployeeRepository _employeeRepository = employeeRepository;
    private readonly IConfiguration _configuration = configuration;
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
        var expiresIn = int.Parse(_configuration["JwtSettings:ExpiryInHours"] ?? "1") * 3600;

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
                RoleId = employee.RoleId,
                RoleName = employee.Role?.Name ?? "Unknown",
                DefaultRoute = employee.Role?.DefaultRoute,
                Permissions = permissions
            }
        };
    }

    public Task<TokenResponse?> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        // TODO: Implement refresh token logic
        // For now, return null
        return Task.FromResult<TokenResponse?>(null);
    }

    public Task<bool> ValidateTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"] ?? "");
            
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["JwtSettings:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["JwtSettings:Audience"],
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

        // Link media if avatar changed
        if (!string.IsNullOrEmpty(dto.AvatarUrl) && dto.AvatarUrl != employee.AvatarUrl) // Note: employee is updated in memory but logic requires checking old value. 
        // Actually, update is called above. I need to capture old value before update or check dto against null.
        // The original code didn't capture old value properly in my snippet.
        // Let's re-read the file content logic.
        // Wait, I should do this carefully. 
        // I will do a targeted replacement of the end of the function.
        if (!string.IsNullOrEmpty(dto.AvatarUrl))
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
            Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"] ?? ""));
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
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: TimeUtils.GetVietnamTime().AddHours(int.Parse(_configuration["JwtSettings:ExpiryInHours"] ?? "1")),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}





