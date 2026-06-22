namespace FoodstoreApi.Usecase.DTOs.EInvoice;

public record EInvoiceProviderDto(
    Guid Id,
    string Name,
    string ProviderType,
    bool IsActive,
    object Config,
    string? Description,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record CreateEInvoiceProviderDto(
    string Name,
    string ProviderType,
    bool IsActive,
    object Config,
    string? Description
);

public record UpdateEInvoiceProviderDto(
    string Name,
    string ProviderType,
    bool IsActive,
    object Config,
    string? Description
);
