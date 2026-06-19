using FoodstoreApi.Core.Entities;

namespace FoodstoreApi.Usecase.Interfaces;

public interface IPaymentSettingService
{
    Task<PaymentSetting?> GetAsync(CancellationToken cancellationToken = default);
    Task<List<PaymentSetting>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PaymentSetting?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PaymentSetting> SaveAsync(PaymentSetting setting, CancellationToken cancellationToken = default);
    Task SetDefaultAsync(Guid id, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
