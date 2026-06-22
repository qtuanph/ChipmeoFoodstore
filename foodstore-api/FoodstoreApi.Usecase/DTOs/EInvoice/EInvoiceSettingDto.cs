namespace FoodstoreApi.Usecase.DTOs.EInvoice;

public record EInvoiceSettingDto(
    Guid Id,
    Guid? DefaultProviderId,
    string? DefaultProviderName,
    bool AutoIssue,
    string? DefaultTemplateCode,
    string? DefaultSerialNumber,
    object? DigitalSignatureConfig,
    DateTime UpdatedAt
);

public record UpdateEInvoiceSettingDto(
    Guid? DefaultProviderId,
    bool AutoIssue,
    string? DefaultTemplateCode,
    string? DefaultSerialNumber,
    object? DigitalSignatureConfig
);
