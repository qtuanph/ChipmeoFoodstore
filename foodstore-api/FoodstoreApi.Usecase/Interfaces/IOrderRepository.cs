using FoodstoreApi.Core.Entities;


public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Order> CreateAsync(Order order, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Order order, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Order>> GetByStatusAsync(string status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Order>> GetBySourceIdAsync(int sourceId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Order>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Order> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default);
    
    // New methods for Service refactoring
    Task<Order?> GetLastOrderByCodePrefixAsync(string prefix, CancellationToken cancellationToken = default);
    Task AddHistoryAsync(OrderStatusHistory history, CancellationToken cancellationToken = default);
    Task<int> CountActiveOrdersBySourceIdAsync(int sourceId, int? excludeOrderId = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<Order>> GetPaidOrdersForKitchenAsync(CancellationToken cancellationToken = default);
}





