using ChipmeoApis.Usecase.DTOs.Order;

namespace ChipmeoApis.Usecase.Interfaces;

public interface IOrderService
{
    Task<IEnumerable<OrderDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<OrderDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<OrderDto> CreateAsync(CreateOrderDto dto, int employeeId, CancellationToken cancellationToken = default);
    Task<OrderDto> ProcessPaymentAsync(int orderId, ProcessPaymentDto dto, int? employeeId = null, CancellationToken cancellationToken = default);
    Task<bool> UpdateStatusAsync(int id, string status, int? employeeId = null, string? paymentMethod = null, decimal? paymentAmount = null, CancellationToken cancellationToken = default);
    Task<OrderDto> UpdateAsync(int id, CreateOrderDto dto, int employeeId, CancellationToken cancellationToken = default);
    Task<bool> UpdateKitchenStatusAsync(int orderId, string status, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrderDto>> GetByStatusAsync(string status, CancellationToken cancellationtoken = default);
    Task<IEnumerable<OrderDto>> GetPaidOrdersForKitchenAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<OrderDto>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<(IEnumerable<OrderDto> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default);
}




