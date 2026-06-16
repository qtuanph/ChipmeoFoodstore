using ChipmeoApis.Usecase.Interfaces;
using ChipmeoApis.Usecase.DTOs.Auth;
using Microsoft.AspNetCore.Mvc;

namespace ChipmeoApis.Web.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService, IConfiguration configuration) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly IConfiguration _configuration = configuration;

    /// <summary>
    /// Đăng nhập cho nhân viên/admin
    /// </summary>
    /// <summary>
    /// Đăng nhập cho nhân viên/admin
    /// </summary>
    [HttpPost("login")]
    [Microsoft.AspNetCore.RateLimiting.EnableRateLimiting("AuthPolicy")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { error = "Username and password are required" });
        }

        var response = await _authService.LoginAsync(request, cancellationToken);
        
        if (response == null)
        {
            return Unauthorized(new { error = "Invalid credentials" });
        }

        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var response = await _authService.RefreshTokenAsync(request.RefreshToken, cancellationToken);
        
        if (response == null)
        {
            return Unauthorized(new { error = "Invalid refresh token" });
        }

        return Ok(response);
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        return Ok(new { message = "Logged out successfully" });
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized(new { error = "User not authenticated" });
        }

        var profile = await _authService.GetProfileAsync(userId, cancellationToken);
        if (profile == null)
        {
            return NotFound(new { error = "User not found" });
        }

        return Ok(profile);
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized(new { error = "User not authenticated" });
        }

        try
        {
            var updatedProfile = await _authService.UpdateProfileAsync(userId, dto, cancellationToken);
            if (updatedProfile == null)
            {
                return NotFound(new { error = "User not found" });
            }
            return Ok(updatedProfile);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}




