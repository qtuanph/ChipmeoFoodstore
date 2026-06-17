using FoodstoreApi.Core.Entities;

namespace FoodstoreApi.Usecase.Interfaces;

public interface IPaymentSettingService
{
    Task<PaymentSetting?> GetAsync(CancellationToken cancellationToken = default);
    Task<List<PaymentSetting>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PaymentSetting?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PaymentSetting> SaveAsync(PaymentSetting setting, CancellationToken cancellationToken = default);
    Task SetDefaultAsync(int id, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}




