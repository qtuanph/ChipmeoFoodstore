using FoodstoreApi.Core.Entities;

namespace FoodstoreApi.Usecase.Interfaces;

public interface IPaymentSettingRepository
{
    Task<PaymentSetting?> GetDefaultAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<PaymentSetting>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PaymentSetting?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PaymentSetting> AddAsync(PaymentSetting setting, CancellationToken cancellationToken = default);
    Task UpdateAsync(PaymentSetting setting, CancellationToken cancellationToken = default);
    Task DeleteAsync(PaymentSetting setting, CancellationToken cancellationToken = default);
    Task ClearOtherDefaultsAsync(int? excludeId, CancellationToken cancellationToken = default);
}




