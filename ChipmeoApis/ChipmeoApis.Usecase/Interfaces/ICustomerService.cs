using ChipmeoApis.Usecase.DTOs;
using ChipmeoApis.Usecase.DTOs.Customer;

namespace ChipmeoApis.Usecase.Interfaces;

public interface ICustomerService
{
    Task<CustomerDto> RegisterAsync(CustomerRegisterDto dto);
    Task<CustomerLoginResultDto> LoginAsync(CustomerLoginDto dto);
    Task<CustomerDto?> GetCustomerByIdAsync(int id);
    Task<List<CustomerDto>> GetAllCustomersAsync();
    Task<CustomerDto?> GetByPhoneAsync(string phone);
    
    // Admin methods
    Task<List<CustomerDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CustomerDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CustomerDto> CreateAsync(CreateCustomerDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, UpdateCustomerAdminDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<CustomerDto?> UpdateProfileAsync(int id, CustomerUpdateDto dto);
    Task<bool> AddPointsAsync(int customerId, int points);
}




