using ChipmeoApis.Usecase.Interfaces;
using ChipmeoApis.Usecase.DTOs.Employee;
using ChipmeoApis.Core.Entities;
using ChipmeoApis.Core.Utils;

namespace ChipmeoApis.Usecase.Services;

public class EmployeeService(IEmployeeRepository repository) : IEmployeeService
{
    private readonly IEmployeeRepository _repository = repository;

    public async Task<IEnumerable<EmployeeDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var employees = await _repository.GetAllAsync(cancellationToken);
        return employees.Select(MapToDto);
    }

    public async Task<EmployeeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var employee = await _repository.GetByIdAsync(id, cancellationToken);
        return employee == null ? null : MapToDto(employee);
    }

    public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto, CancellationToken cancellationToken = default)
    {
        // Check if username exists
        var existing = await _repository.GetByUsernameAsync(dto.Username, cancellationToken);
        if (existing != null)
            throw new Exception("Username already exists");

        var employee = new Employee
        {
            FullName = dto.FullName,
            Username = dto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Email = dto.Email,
            Phone = dto.Phone,
            AvatarUrl = dto.AvatarUrl,
            RoleId = dto.RoleId,
            IsActive = dto.IsActive,
            CreatedAt = TimeUtils.GetVietnamTime()
        };

        var created = await _repository.CreateAsync(employee, cancellationToken);
        return MapToDto(created);
    }

    public async Task<bool> UpdateAsync(int id, UpdateEmployeeDto dto, CancellationToken cancellationToken = default)
    {
        var employee = await _repository.GetByIdAsync(id, cancellationToken);
        if (employee == null) return false;

        employee.FullName = dto.FullName;
        employee.Email = dto.Email;
        employee.Phone = dto.Phone;
        employee.AvatarUrl = dto.AvatarUrl;
        employee.RoleId = dto.RoleId;
        employee.IsActive = dto.IsActive;

        // Update password only if provided
        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
            employee.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        }

        return await _repository.UpdateAsync(employee, cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _repository.DeleteAsync(id, cancellationToken);
    }

    private static EmployeeDto MapToDto(Employee employee)
    {
        return new EmployeeDto
        {
            Id = employee.Id,
            FullName = employee.FullName ?? employee.Username,
            Username = employee.Username,
            Email = employee.Email,
            Phone = employee.Phone,
            AvatarUrl = employee.AvatarUrl,
            RoleId = employee.RoleId,
            RoleName = employee.Role?.Name ?? "",
            IsActive = employee.IsActive ?? true,
            LastLogin = employee.LastLogin,
            CreatedAt = employee.CreatedAt ?? TimeUtils.GetVietnamTime()
        };
    }
}




