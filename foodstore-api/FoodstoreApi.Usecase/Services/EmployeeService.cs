using FoodstoreApi.Core.Entities;
using FoodstoreApi.Core.Entities.Identity;
using FoodstoreApi.Usecase.DTOs.Employee;
using FoodstoreApi.Usecase.Interfaces;
using FoodstoreApi.Usecase.Utils;
using Microsoft.AspNetCore.Identity;

namespace FoodstoreApi.Usecase.Services;

public class EmployeeService(
    IEmployeeRepository employeeRepository,
    UserManager<ApplicationUser> userManager) : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository = employeeRepository;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<IEnumerable<EmployeeDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var employees = await _employeeRepository.GetAllAsync(cancellationToken);
        return employees.Select(MapToDto);
    }

    public async Task<EmployeeDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdAsync(id, cancellationToken);
        return employee == null ? null : MapToDto(employee);
    }

    public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto, CancellationToken cancellationToken = default)
    {
        var normalizedUsername = UsernameHelper.Normalize(dto.Username);
        var existing = await _userManager.FindByNameAsync(normalizedUsername);
        if (existing != null)
            throw new Exception("Username already exists");

        var user = new ApplicationUser
        {
            UserName = normalizedUsername,
            Email = dto.Email ?? "",
            Name = dto.FullName,
            Banned = !dto.IsActive,
        };

        var createResult = await _userManager.CreateAsync(user, dto.Password);
        if (!createResult.Succeeded)
            throw new Exception(string.Join("; ", createResult.Errors.Select(e => e.Description)));

        var employee = new Employee
        {
            UserId = user.Id,
            RoleId = dto.RoleId,
            Phone = dto.Phone,
            AvatarUrl = dto.AvatarUrl,

        };

        var created = await _employeeRepository.CreateAsync(employee, cancellationToken);
        return MapToDto(created);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateEmployeeDto dto, CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdAsync(id, cancellationToken);
        if (employee == null) return false;

        employee.User.Name = dto.FullName;
        employee.User.Email = dto.Email ?? "";
        employee.Phone = dto.Phone;
        employee.AvatarUrl = dto.AvatarUrl;
        employee.RoleId = dto.RoleId;
        employee.User.Banned = !dto.IsActive;

        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(employee.User);
            var resetResult = await _userManager.ResetPasswordAsync(employee.User, token, dto.Password);
            if (!resetResult.Succeeded)
                throw new Exception("Password update failed");
        }

        await _userManager.UpdateAsync(employee.User);
        return await _employeeRepository.UpdateAsync(employee, cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _employeeRepository.DeleteAsync(id, cancellationToken);
    }

    private static EmployeeDto MapToDto(Employee employee)
    {
        return new EmployeeDto
        {
            Id = employee.Id,
            FullName = employee.User.Name,
            Username = employee.User.UserName ?? "",
            Email = employee.User.Email,
            Phone = employee.Phone,
            AvatarUrl = employee.AvatarUrl,
            RoleId = employee.RoleId,
            RoleName = employee.Role?.Name ?? "",
            IsActive = !employee.User.Banned,
            LastLogin = employee.LastLogin,
            CreatedAt = employee.CreatedAt,
            UpdatedAt = employee.UpdatedAt,
            CreatedBy = employee.CreatedBy,
            UpdatedBy = employee.UpdatedBy
        };
    }
}
