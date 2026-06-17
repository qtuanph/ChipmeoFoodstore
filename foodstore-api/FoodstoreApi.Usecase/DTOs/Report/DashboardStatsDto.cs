namespace FoodstoreApi.Usecase.DTOs.Report;

public record DashboardOverviewDto(
    decimal TodayRevenue,
    decimal TotalVat,
    int TodayOrders,
    decimal AvgOrderValue,
    string PeakHour,
    List<PopularItemDto> PopularItems,
    List<PopularItemDto> SourceStats
);

public record DashboardStatsDto(
    decimal TotalRevenue,
    decimal TotalVat,
    int TotalOrders,
    List<ChartDataDto> RevenueChart,
    List<ChartDataDto> OrdersChart,
    List<PopularItemDto> TopItems,
    List<PopularItemDto> PopularCombos,
    List<PopularItemDto> SourceStats
);

public record ChartDataDto(
    string Label,
    decimal Value
);

public record SalesForecastDto(
    List<ForecastDataDto> Forecasts,
    string? Recommendation = null
);

public record ForecastDataDto(
    string Date,
    float Revenue
);

public record PopularItemDto(
    string Name,
    int Sold
);




