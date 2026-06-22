using System.Text.Json;
using FoodstoreApi.Usecase.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FoodstoreApi.Usecase.Services;

public class EInvoiceProviderFactory(IServiceProvider serviceProvider) : IEInvoiceProviderFactory
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public IEInvoiceProvider? GetProvider(string providerType, string configJson)
    {
        var config = string.IsNullOrEmpty(configJson)
            ? new Dictionary<string, object>()
            : JsonSerializer.Deserialize<Dictionary<string, object>>(configJson) ?? new();

        var providers = _serviceProvider.GetServices<IEInvoiceProvider>();
        return providers.FirstOrDefault(p =>
            p.ProviderType.Equals(providerType, StringComparison.OrdinalIgnoreCase));
    }
}
