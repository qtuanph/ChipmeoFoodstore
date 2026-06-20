using FoodstoreApi.Usecase.DTOs.Customer;

namespace FoodstoreApi.Usecase.Interfaces;

public interface ICustomerService
{
    Task<CustomerDto> RegisterAsync(CustomerRegisterDto dto);
    Task<CustomerLoginResultDto> LoginAsync(CustomerLoginDto dto);
    Task<CustomerDto?> GetCustomerByIdAsync(Guid id);
    Task<List<CustomerDto>> GetAllCustomersAsync();
    Task<CustomerDto?> GetByPhoneAsync(string phone);

    Task<List<CustomerDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CustomerDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CustomerDto> CreateAsync(CreateCustomerDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Guid id, UpdateCustomerAdminDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<CustomerDto?> UpdateProfileAsync(Guid id, CustomerUpdateDto dto);
    Task<bool> AddPointsAsync(Guid customerId, int points, string? reason = null);
    Task<List<CustomerOrderHistoryDto>> GetOrderHistoryAsync(Guid customerId);
    Task<UpcomingBirthdaysDto> GetUpcomingBirthdaysAsync();
}
