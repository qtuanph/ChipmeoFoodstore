namespace FoodstoreApi.Usecase.DTOs.EInvoice;

public record EInvoiceDashboardDto(
    int TotalInvoices,
    int DraftInvoices,
    int IssuedInvoices,
    int FailedInvoices,
    int CancelledInvoices,
    int ActiveProviders
);
