namespace FoodstoreApi.Usecase.DTOs.Report;

public record DashboardLegacyDto(
    object Today,
    object Month,
    object Total,
    List<PopularItemLegacyDto> PopularItems,
    List<PopularItemLegacyDto> PopularCombos,
    List<DailyRevenueDto> Last7Days,
    List<MonthlyRevenueDto> Last6Months,
    decimal AverageOrderValue,
    string PeakHour,
    Dictionary<string, decimal> PaymentMethodBreakdown,
    int TotalCustomers,
    List<PopularItemLegacyDto> ServiceTypeStats
);

public record PopularItemLegacyDto(
    string Name,
    int Quantity,
    decimal Revenue
);

public record DailyRevenueDto(
    string Date,
    decimal Revenue,
    int Orders
);

public record MonthlyRevenueDto(
    string Month,
    decimal Revenue,
    int Orders
);




