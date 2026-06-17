using FoodstoreApi.Usecase.Interfaces;
using FoodstoreApi.Usecase.DTOs;
using FoodstoreApi.Usecase.DTOs.Customer;
using FoodstoreApi.Core.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FoodstoreApi.Core.Utils;

namespace FoodstoreApi.Usecase.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repo;
    private readonly IConfiguration _configuration;
    private readonly IMediaService _mediaService;

    public CustomerService(ICustomerRepository repo, IConfiguration configuration, IMediaService mediaService)
    {
        _repo = repo;
        _configuration = configuration;
        _mediaService = mediaService;
    }

    public async Task<CustomerDto> RegisterAsync(CustomerRegisterDto dto)
    {
        // Check if email already exists
        var existingByEmail = await _repo.GetByEmailAsync(dto.Email);
        if (existingByEmail != null)
        {
            throw new Exception("Email đã được sử dụng");
        }

        // Check if phone already exists (if provided)
        if (!string.IsNullOrEmpty(dto.Phone))
        {
            var existingByPhone = await _repo.GetByPhoneAsync(dto.Phone);
            if (existingByPhone != null)
            {
                throw new Exception("Số điện thoại đã được sử dụng");
            }
        }

        var customer = new Customer
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Phone = dto.Phone,
            Points = 0,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repo.AddAsync(customer);
        return MapToDto(created);
    }

    public async Task<CustomerLoginResultDto> LoginAsync(CustomerLoginDto dto)
    {
        var customer = await _repo.GetByEmailAsync(dto.Email);
        
        if (customer == null || customer.IsActive != true)
        {
            throw new Exception("Email hoặc mật khẩu không đúng");
        }

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, customer.PasswordHash))
        {
            throw new Exception("Email hoặc mật khẩu không đúng");
        }

        var token = GenerateJwtToken(customer);
        
        return new CustomerLoginResultDto
        {
            Customer = MapToDto(customer),
            Token = token
        };
    }

    public async Task<CustomerDto?> GetCustomerByIdAsync(int id)
    {
        var customer = await _repo.GetByIdAsync(id);
        return customer == null ? null : MapToDto(customer);
    }

    public async Task<CustomerDto?> GetByPhoneAsync(string phone)
    {
        var customer = await _repo.GetByPhoneAsync(phone);
        return customer == null ? null : MapToDto(customer);
    }

    public async Task<List<CustomerDto>> GetAllCustomersAsync()
    {
        var customers = await _repo.GetAllAsync();
        return customers.Select(c => MapToDto(c)).ToList();
    }

    public async Task<CustomerDto?> UpdateProfileAsync(int id, CustomerUpdateDto dto)
    {
        var customer = await _repo.GetByIdAsync(id);
        if (customer == null) return null;

        var oldAvatarUrl = customer.AvatarUrl;

        if (!string.IsNullOrEmpty(dto.FullName))
            customer.FullName = dto.FullName;
        if (!string.IsNullOrEmpty(dto.Phone))
            customer.Phone = dto.Phone;
        if (!string.IsNullOrEmpty(dto.AvatarUrl))
            customer.AvatarUrl = dto.AvatarUrl;

        await _repo.UpdateAsync(customer);
        
        if (!string.IsNullOrEmpty(dto.AvatarUrl) && dto.AvatarUrl != oldAvatarUrl)
        {
            await _mediaService.LinkMediaToEntityAsync(dto.AvatarUrl, "customer", id);
        }

        return MapToDto(customer);
    }

    public async Task<bool> AddPointsAsync(int customerId, int points)
    {
        var customer = await _repo.GetByIdAsync(customerId);
        if (customer == null) return false;

        customer.Points = (customer.Points ?? 0) + points;
        await _repo.UpdateAsync(customer);
        return true;
    }

    public async Task<List<CustomerDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var customers = await _repo.GetAllAsync();
        return customers.Select(c => MapToDto(c)).ToList();
    }

    public async Task<CustomerDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await GetCustomerByIdAsync(id);
    }

    public async Task<CustomerDto> CreateAsync(CreateCustomerDto dto, CancellationToken cancellationToken = default)
    {
        var existingByEmail = await _repo.GetByEmailAsync(dto.Email);
        if (existingByEmail != null) throw new Exception("Email đã được sử dụng");

        if (!string.IsNullOrEmpty(dto.Phone))
        {
            var existingByPhone = await _repo.GetByPhoneAsync(dto.Phone);
            if (existingByPhone != null) throw new Exception("Số điện thoại đã được sử dụng");
        }

        var customer = new Customer
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Phone = dto.Phone,
            Points = 0,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repo.AddAsync(customer);
        return MapToDto(created);
    }

    public async Task<bool> UpdateAsync(int id, UpdateCustomerAdminDto dto, CancellationToken cancellationToken = default)
    {
        var customer = await _repo.GetByIdAsync(id);
        if (customer == null) return false;

        if (dto.FullName != null) customer.FullName = dto.FullName;
        if (dto.Phone != null) customer.Phone = dto.Phone;
        if (dto.IsActive.HasValue) customer.IsActive = dto.IsActive;
        if (dto.Points.HasValue) customer.Points = dto.Points;

        if (!string.IsNullOrEmpty(dto.AvatarUrl) && dto.AvatarUrl != customer.AvatarUrl)
        {
            customer.AvatarUrl = dto.AvatarUrl;
            await _mediaService.LinkMediaToEntityAsync(dto.AvatarUrl, "customer", id);
        }

        await _repo.UpdateAsync(customer);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var customer = await _repo.GetByIdAsync(id);
        if (customer == null) return false;

        await _mediaService.DeleteMediaByEntityAsync("customer", id);
        await _repo.DeleteAsync(customer);
        return true;
    }

    private static CustomerDto MapToDto(Customer customer)
    {
        return new CustomerDto
        {
            Id = customer.Id,
            FullName = customer.FullName,
            Phone = customer.Phone,
            Email = customer.Email ?? "",
            AvatarUrl = customer.AvatarUrl,
            Points = customer.Points ?? 0,
            IsActive = customer.IsActive ?? true,
            CreatedAt = customer.CreatedAt
        };
    }

    private string GenerateJwtToken(Customer customer)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"] ?? "FoodstoreApiSecretKey123!@#"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString()),
            new Claim(ClaimTypes.Email, customer.Email ?? ""),
            new Claim(ClaimTypes.Name, customer.FullName),
            new Claim(ClaimTypes.Role, "Customer")
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: TimeUtils.GetVietnamTime().AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}




