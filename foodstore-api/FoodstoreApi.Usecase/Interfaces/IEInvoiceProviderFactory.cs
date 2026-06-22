namespace FoodstoreApi.Usecase.Interfaces;

public interface IEInvoiceProviderFactory
{
    IEInvoiceProvider? GetProvider(string providerType, string configJson);
}
