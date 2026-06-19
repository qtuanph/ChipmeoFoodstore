using FoodstoreApi.Usecase.Interfaces;
using FoodstoreApi.Usecase.DTOs.Auth;
using FoodstoreApi.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FoodstoreApi.Web.ApiResponse;

namespace FoodstoreApi.Web.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("login")]
    [AllowAnonymous]
    [Microsoft.AspNetCore.RateLimiting.EnableRateLimiting("AuthPolicy")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return ApiResult.BadRequest("Username and password are required");

        var response = await _authService.LoginAsync(request, cancellationToken);

        if (response == null)
            return Unauthorized();

        SetAuthCookie(response.Token);

        return ApiResult.Success(response);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var response = await _authService.RefreshTokenAsync(request.RefreshToken, cancellationToken);

        if (response == null)
            return Unauthorized();

        return ApiResult.Success(response);
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        ClearAuthCookie();
        return ApiResult.Success(new { message = "Logged out successfully" });
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        var profile = await _authService.GetProfileAsync(userId, cancellationToken);
        if (profile == null) return ApiResult.NotFound("User not found");

        return ApiResult.Success(profile);
    }

    [HttpPut("profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        try
        {
            var updatedProfile = await _authService.UpdateProfileAsync(userId, dto, cancellationToken);
            return updatedProfile == null
                ? ApiResult.NotFound("User not found")
                : ApiResult.Success(updatedProfile);
        }
        catch (ArgumentException ex)
        {
            return ApiResult.BadRequest(ex.Message);
        }
    }

    private void SetAuthCookie(string token)
    {
        Response.Cookies.Append("auth_token", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });
    }

    private void ClearAuthCookie()
    {
        Response.Cookies.Append("auth_token", "", new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UnixEpoch
        });
    }
}

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}
