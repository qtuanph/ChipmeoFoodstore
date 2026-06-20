using FoodstoreApi.Core.Entities;
using FoodstoreApi.Infrastructure.Data;
using FoodstoreApi.Usecase.Interfaces;
using FoodstoreApi.Usecase.DTOs.Customer;
using Microsoft.EntityFrameworkCore;

namespace FoodstoreApi.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly StoreDbContext _context;

    public CustomerRepository(StoreDbContext context)
    {
        _context = context;
    }

    public async Task<Customer?> GetByIdAsync(Guid id)
    {
        return await _context.Customers.FindAsync(id);
    }

    public async Task<Customer?> GetByUserIdAsync(Guid userId)
    {
        return await _context.Customers.FirstOrDefaultAsync(c => c.UserId == userId);
    }

    public async Task<Customer?> GetByEmailAsync(string email)
    {
        return await _context.Customers
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.User.Email == email);
    }

    public async Task<Customer?> GetByPhoneAsync(string phone)
    {
        return await _context.Customers.FirstOrDefaultAsync(c => c.Phone == phone);
    }

    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        return await _context.Customers
            .Include(c => c.User)
            .ToListAsync();
    }

    public async Task<Customer> AddAsync(Customer customer)
    {
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
        return customer;
    }

    public async Task UpdateAsync(Customer customer)
    {
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Customer customer)
    {
        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();
    }

    public async Task<List<CustomerOrderHistoryDto>> GetOrderHistoryByCustomerIdAsync(Guid customerId)
    {
        return await _context.Orders
            .AsNoTracking()
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new CustomerOrderHistoryDto
            {
                Id = o.Id,
                OrderCode = o.OrderCode,
                CreatedAt = o.CreatedAt,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
            })
            .ToListAsync();
    }

    public async Task<List<Customer>> GetUpcomingBirthdaysAsync(DateTime from, DateTime to)
    {
        var customers = await _context.Customers
            .AsNoTracking()
            .Include(c => c.User)
            .Where(c => c.Birthday != null)
            .ToListAsync();

        return customers.Where(c =>
        {
            var b = c.Birthday!.Value;
            var thisYear = new DateTime(from.Year, b.Month, b.Day);
            return thisYear >= from.Date && thisYear <= to.Date;
        }).ToList();
    }
}
