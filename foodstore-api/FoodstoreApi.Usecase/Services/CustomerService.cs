using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FoodstoreApi.Core.Configuration;
using FoodstoreApi.Core.Entities;
using FoodstoreApi.Core.Entities.Identity;
using FoodstoreApi.Usecase.DTOs.Customer;
using FoodstoreApi.Usecase.Interfaces;
using FoodstoreApi.Usecase.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FoodstoreApi.Usecase.Services;

public class CustomerService(
    ICustomerRepository repo,
    UserManager<ApplicationUser> userManager,
    IOptions<JwtSettings> jwtOptions,
    IMediaService mediaService) : ICustomerService
{
    private readonly ICustomerRepository _repo = repo;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly JwtSettings _jwtSettings = jwtOptions.Value;
    private readonly IMediaService _mediaService = mediaService;

    public async Task<CustomerDto> RegisterAsync(CustomerRegisterDto dto)
    {
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
            throw new Exception("Email đã được sử dụng");

        var normalizedUsername = UsernameHelper.Normalize(dto.Username);
        var existingUsername = await _userManager.FindByNameAsync(normalizedUsername);
        if (existingUsername != null)
            throw new Exception("Username đã được sử dụng");

        var user = new ApplicationUser
        {
            UserName = normalizedUsername,
            Email = dto.Email,
            EmailConfirmed = false,
            Name = dto.Name,

        };

        var createResult = await _userManager.CreateAsync(user, dto.Password);
        if (!createResult.Succeeded)
            throw new Exception(string.Join("; ", createResult.Errors.Select(e => e.Description)));

        var customer = new Customer
        {
            UserId = user.Id,
            CustomerCode = $"KH-{DateTime.Now:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}",
            Phone = dto.Phone,
            Birthday = dto.Birthday,
            LoyaltyPoints = 0,
            MembershipLevel = "bronze",
        };

        customer = await _repo.AddAsync(customer);
        return MapToDto(customer, user);
    }

    public async Task<CustomerLoginResultDto> LoginAsync(CustomerLoginDto dto)
    {
        var user = await _userManager.FindByNameAsync(dto.Username);
        if (user == null || user.Banned)
            throw new Exception("Username hoặc mật khẩu không đúng");

        var passwordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!passwordValid)
            throw new Exception("Username hoặc mật khẩu không đúng");

        var customer = await _repo.GetByUserIdAsync(user.Id);
        if (customer == null)
            throw new Exception("Email hoặc mật khẩu không đúng");

        var token = GenerateJwtToken(user, customer);

        return new CustomerLoginResultDto
        {
            Customer = MapToDto(customer, user),
            Token = token
        };
    }

    public async Task<CustomerDto?> GetCustomerByIdAsync(Guid id)
    {
        var customer = await _repo.GetByIdAsync(id);
        if (customer == null) return null;
        var user = await _userManager.FindByIdAsync(customer.UserId.ToString());
        return MapToDto(customer, user);
    }

    public async Task<CustomerDto?> GetByPhoneAsync(string phone)
    {
        var customer = await _repo.GetByPhoneAsync(phone);
        if (customer == null) return null;
        var user = await _userManager.FindByIdAsync(customer.UserId.ToString());
        return MapToDto(customer, user);
    }

    public async Task<List<CustomerDto>> GetAllCustomersAsync()
    {
        var customers = await _repo.GetAllAsync();
        var result = new List<CustomerDto>();
        foreach (var c in customers)
        {
            var user = await _userManager.FindByIdAsync(c.UserId.ToString());
            result.Add(MapToDto(c, user));
        }
        return result;
    }

    public async Task<CustomerDto?> UpdateProfileAsync(Guid id, CustomerUpdateDto dto)
    {
        var customer = await _repo.GetByIdAsync(id);
        if (customer == null) return null;

        var user = await _userManager.FindByIdAsync(customer.UserId.ToString());
        if (user == null) return null;

        var oldAvatarUrl = customer.AvatarUrl;

        if (!string.IsNullOrEmpty(dto.Name))
        {
            user.Name = dto.Name;

        }

        if (!string.IsNullOrEmpty(dto.Phone))
            customer.Phone = dto.Phone;

        if (dto.Birthday.HasValue)
            customer.Birthday = dto.Birthday;

        if (!string.IsNullOrEmpty(dto.AvatarUrl))
            customer.AvatarUrl = dto.AvatarUrl;

        if (dto.Points.HasValue)
        {
            customer.LoyaltyPoints = dto.Points.Value;
            customer.MembershipLevel = CalculateMembershipLevel(dto.Points.Value);
        }

        if (dto.IsActive.HasValue)
            user.Banned = !dto.IsActive.Value;

        await _userManager.UpdateAsync(user);
        await _repo.UpdateAsync(customer);

        if (!string.IsNullOrEmpty(dto.AvatarUrl) && dto.AvatarUrl != oldAvatarUrl)
            await _mediaService.LinkMediaToEntityAsync(dto.AvatarUrl, "customer", customer.Id);

        return MapToDto(customer, user);
    }

    public async Task<bool> AddPointsAsync(Guid customerId, int points, string? reason = null)
    {
        if (points <= 0) return false;
        var customer = await _repo.GetByIdAsync(customerId);
        if (customer == null) return false;

        customer.LoyaltyPoints += points;
        customer.MembershipLevel = CalculateMembershipLevel(customer.LoyaltyPoints);

        await _repo.UpdateAsync(customer);
        return true;
    }

    public async Task<List<CustomerDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await GetAllCustomersAsync();
    }

    public async Task<CustomerDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await GetCustomerByIdAsync(id);
    }

    public async Task<CustomerDto> CreateAsync(CreateCustomerDto dto, CancellationToken cancellationToken = default)
    {
        var phone = dto.Phone ?? Guid.NewGuid().ToString("N")[..10];
        var username = !string.IsNullOrWhiteSpace(dto.Username)
            ? UsernameHelper.Normalize(dto.Username)
            : $"kh{phone.Replace(" ", "").Replace("+84", "0")}";
        var email = dto.Email ?? $"{username}@temp.foodstore";
        var password = dto.Password ?? "123456";

        var existingEmail = await _userManager.FindByEmailAsync(email);
        if (existingEmail != null) throw new Exception("Email đã được sử dụng");

        var existingUsername = await _userManager.FindByNameAsync(username);
        if (existingUsername != null) throw new Exception("Username đã được sử dụng");

        var user = new ApplicationUser
        {
            UserName = username,
            Email = email,
            EmailConfirmed = true,
            Name = dto.Name,
            Banned = !dto.IsActive,
        };

        var createResult = await _userManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
            throw new Exception(string.Join("; ", createResult.Errors.Select(e => e.Description)));

        var customer = new Customer
        {
            UserId = user.Id,
            CustomerCode = $"KH-{DateTime.Now:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}",
            Phone = dto.Phone,
            Birthday = dto.Birthday,
            LoyaltyPoints = 0,
            MembershipLevel = "bronze",
        };

        customer = await _repo.AddAsync(customer);
        return MapToDto(customer, user);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateCustomerAdminDto dto, CancellationToken cancellationToken = default)
    {
        var customer = await _repo.GetByIdAsync(id);
        if (customer == null) return false;

        var user = await _userManager.FindByIdAsync(customer.UserId.ToString());
        if (user == null) return false;

        if (dto.Name != null) user.Name = dto.Name;
        if (dto.Phone != null) customer.Phone = dto.Phone;
        if (dto.Birthday.HasValue) customer.Birthday = dto.Birthday;
        if (dto.IsActive.HasValue) user.Banned = !dto.IsActive.Value;
        if (dto.Points.HasValue)
        {
            customer.LoyaltyPoints = dto.Points.Value;
            customer.MembershipLevel = CalculateMembershipLevel(dto.Points.Value);
        }

        if (!string.IsNullOrEmpty(dto.AvatarUrl) && dto.AvatarUrl != customer.AvatarUrl)
        {
            customer.AvatarUrl = dto.AvatarUrl;
            await _mediaService.LinkMediaToEntityAsync(dto.AvatarUrl, "customer", customer.Id);
        }


        await _userManager.UpdateAsync(user);
        await _repo.UpdateAsync(customer);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customer = await _repo.GetByIdAsync(id);
        if (customer == null) return false;

        await _mediaService.DeleteMediaByEntityAsync("customer", customer.Id);

        var user = await _userManager.FindByIdAsync(customer.UserId.ToString());
        if (user != null)
        {
            user.Banned = true;
            await _userManager.UpdateAsync(user);
        }

        await _repo.DeleteAsync(customer);
        return true;
    }

    public async Task<List<CustomerOrderHistoryDto>> GetOrderHistoryAsync(Guid customerId)
    {
        return await _repo.GetOrderHistoryByCustomerIdAsync(customerId);
    }

    public async Task<UpcomingBirthdaysDto> GetUpcomingBirthdaysAsync()
    {
        var today = DateTime.UtcNow.Date;
        var weekEnd = today.AddDays(7);
        var monthEnd = today.AddMonths(1);

        var all = await _repo.GetUpcomingBirthdaysAsync(today, monthEnd);

        var thisWeek = all.Where(c =>
        {
            var b = new DateTime(today.Year, c.Birthday!.Value.Month, c.Birthday!.Value.Day);
            return b >= today && b < weekEnd.AddDays(1);
        }).Select(MapToBirthdayDto).ToList();

        var thisMonth = all.Select(MapToBirthdayDto).ToList();

        return new UpcomingBirthdaysDto
        {
            ThisWeek = thisWeek,
            ThisMonth = thisMonth,
            TotalThisWeek = thisWeek.Count,
            TotalThisMonth = thisMonth.Count,
        };
    }

    private static CustomerDto MapToDto(Customer customer, ApplicationUser? user)
    {
        return new CustomerDto
        {
            Id = customer.Id,
            UserId = customer.UserId,
            CustomerCode = customer.CustomerCode,
            Name = user?.Name ?? "",
            Username = user?.UserName ?? "",
            Email = user?.Email ?? "",
            Phone = customer.Phone,
            AvatarUrl = customer.AvatarUrl,
            LoyaltyPoints = customer.LoyaltyPoints,
            MembershipLevel = customer.MembershipLevel,
            Birthday = customer.Birthday,
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt,
            CreatedBy = customer.CreatedBy,
            UpdatedBy = customer.UpdatedBy
        };
    }

    private string GenerateJwtToken(ApplicationUser user, Customer customer)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email ?? ""),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, "customer"),
            new Claim("CustomerId", customer.Id.ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_jwtSettings.ExpiryInHours),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string CalculateMembershipLevel(int points)
    {
        if (points >= 5000) return "diamond";
        if (points >= 1000) return "platinum";
        if (points >= 300) return "gold";
        if (points >= 100) return "silver";
        return "bronze";
    }

    private static CustomerBirthdayDto MapToBirthdayDto(Customer c)
    {
        return new CustomerBirthdayDto
        {
            Id = c.Id,
            CustomerCode = c.CustomerCode,
            Name = c.User?.Name ?? "",
            Phone = c.Phone,
            Birthday = c.Birthday,
            MembershipLevel = c.MembershipLevel,
        };
    }
}
