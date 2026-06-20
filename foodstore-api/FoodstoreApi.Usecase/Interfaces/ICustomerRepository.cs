using FoodstoreApi.Core.Entities;
using FoodstoreApi.Usecase.DTOs.Customer;

namespace FoodstoreApi.Usecase.Interfaces;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(Guid id);
    Task<Customer?> GetByUserIdAsync(Guid userId);
    Task<Customer?> GetByEmailAsync(string email);
    Task<Customer?> GetByPhoneAsync(string phone);
    Task<IEnumerable<Customer>> GetAllAsync();
    Task<Customer> AddAsync(Customer customer);
    Task UpdateAsync(Customer customer);
    Task DeleteAsync(Customer customer);
    Task<List<CustomerOrderHistoryDto>> GetOrderHistoryByCustomerIdAsync(Guid customerId);
    Task<List<Customer>> GetUpcomingBirthdaysAsync(DateTime from, DateTime to);
}
