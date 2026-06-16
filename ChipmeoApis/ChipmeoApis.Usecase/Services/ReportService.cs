using ChipmeoApis.Usecase.Interfaces;
using ChipmeoApis.Usecase.DTOs.Report;
using ChipmeoApis.Core.Utils;
using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;

namespace ChipmeoApis.Usecase.Services;

public class ReportService(IReportRepository repository) : IReportService
{
    private readonly IReportRepository _repository = repository;

    public async Task<DashboardOverviewDto> GetOverviewAsync(CancellationToken cancellationToken = default)
    {
        return await _repository.GetOverviewAsync(cancellationToken);
    }

    public async Task<DashboardStatsDto> GetStatsAsync(DateTime fromDate, DateTime toDate, string groupBy, CancellationToken cancellationToken = default)
    {
        return await _repository.GetStatsAsync(fromDate, toDate, groupBy, cancellationToken);
    }

    public async Task<SalesForecastDto> GetForecastAsync(int horizon = 7, CancellationToken cancellationToken = default)
    {
        // 1. Prepare Data
        var endDate = TimeUtils.GetVietnamTime().Date;
        var startDate = endDate.AddDays(-90);

        var salesData = await _repository.GetSalesDataAsync(startDate, endDate, cancellationToken);

        // Fill missing days with 0
        var fullData = new List<SalesData>();
        for (var date = startDate; date < endDate; date = date.AddDays(1))
        {
            var existing = salesData.FirstOrDefault(x => x.Date == date);
            fullData.Add(new SalesData { Date = date, Revenue = existing?.Revenue ?? 0 });
        }

        // Need at least a few data points to forecast
        if (fullData.Count < 7)
        {
             return new SalesForecastDto(new List<ForecastDataDto>());
        }

        // 2. ML.NET Pipeline
        var mlContext = new MLContext();
        var dataView = mlContext.Data.LoadFromEnumerable(fullData);

        // SSA Forecasting Pipeline
        var forecastingPipeline = mlContext.Forecasting.ForecastBySsa(
            outputColumnName: "ForecastedRevenue",
            inputColumnName: nameof(SalesData.Revenue),
            windowSize: 7,
            seriesLength: fullData.Count,
            trainSize: fullData.Count,
            horizon: horizon,
            confidenceLevel: 0.95f,
            confidenceLowerBoundColumn: "LowerBoundRevenue",
            confidenceUpperBoundColumn: "UpperBoundRevenue");

        // Train
        var model = forecastingPipeline.Fit(dataView);

        // Predict
        var forecastingEngine = model.CreateTimeSeriesEngine<SalesData, SalesPrediction>(mlContext);
        var prediction = forecastingEngine.Predict();

        // 3. Map Results
        var forecasts = new List<ForecastDataDto>();
        for (int i = 0; i < horizon; i++)
        {
            var date = endDate.AddDays(i);
            var revenue = prediction.ForecastedRevenue[i];
            // Ensure no negative revenue
            forecasts.Add(new ForecastDataDto(date.ToString("yyyy-MM-dd"), Math.Max(0, revenue)));
        }

        // 4. AI Recommendation (Service Types)
        string recommendation = "Dữ liệu chưa đủ để phân tích xu hướng chiến lược.";
        try 
        {
            var stats = await GetStatsAsync(TimeUtils.GetVietnamTime().AddDays(-30), TimeUtils.GetVietnamTime(), "day", cancellationToken);
            var typeStats = stats.SourceStats;
            
            if (typeStats != null && typeStats.Count != 0)
            {
                var total = typeStats.Sum(x => x.Sold);
                if (total > 0)
                {
                    var topType = typeStats.OrderByDescending(x => x.Sold).First();
                    var percent = (double)topType.Sold / total * 100;
                    
                    var isOffline = topType.Name.Contains("quán", StringComparison.OrdinalIgnoreCase) || 
                                   topType.Name.Contains("Source", StringComparison.OrdinalIgnoreCase) ||
                                   topType.Name.Contains("Offline", StringComparison.OrdinalIgnoreCase);

                    if (isOffline)
                    {
                        recommendation = $"💡 AI Insight: Khách hàng chủ yếu dùng bữa **tại quán** ({percent:F0}%). \n👉 Chiến lược: Tạo không gian trải nghiệm check-in, upsell trực tiếp combo đồ uống và món tráng miệng.";
                    }
                    else
                    {
                        recommendation = $"💡 AI Insight: Xu hướng đặt món **{topType.Name}** đang chiếm ưu thế ({percent:F0}%). \n👉 Chiến lược: Đẩy mạnh quảng cáo trên nền tảng Online, thiết kế bao bì bắt mắt và tối ưu thời gian giao hàng.";
                    }
                }
            }
        }
        catch 
        {
            // Ignore stats error during forecast
        }

        return new SalesForecastDto(forecasts, recommendation);
    }

    public class SalesData
    {
        public DateTime Date { get; set; }
        public float Revenue { get; set; }
    }

    public class SalesPrediction
    {
        public float[] ForecastedRevenue { get; set; } = null!;
        public float[] LowerBoundRevenue { get; set; } = null!;
        public float[] UpperBoundRevenue { get; set; } = null!;
    }

    public async Task<DashboardLegacyDto> GetDashboardStatsAsync(CancellationToken cancellationToken = default)
    {
        return await _repository.GetLegacyDashboardStatsAsync(cancellationToken);
    }

    public async Task<List<ComboRecommendationDto>> GetComboRecommendationsAsync(CancellationToken cancellationToken = default)
    {
        var cutoffDate = TimeUtils.GetVietnamTime().AddDays(-30);
        var rawData = await _repository.GetComboRecommendationDataAsync(cutoffDate, cancellationToken);

        var pairCounts = new Dictionary<(int, int), int>();
        var itemInfo = new Dictionary<int, (string Name, decimal Price)>();

        var orders = rawData.GroupBy(x => x.OrderId);

        foreach (var order in orders)
        {
            var uniqueItems = order.DistinctBy(i => i.MenuItemId).ToList();

            for (int i = 0; i < uniqueItems.Count; i++)
            {
                if (!itemInfo.ContainsKey(uniqueItems[i].MenuItemId!.Value))
                {
                    itemInfo[uniqueItems[i].MenuItemId!.Value] = (uniqueItems[i].MenuItemName!, uniqueItems[i].UnitPrice);
                }

                for (int j = i + 1; j < uniqueItems.Count; j++)
                {
                    var id1 = uniqueItems[i].MenuItemId!.Value;
                    var id2 = uniqueItems[j].MenuItemId!.Value;

                    var key = id1 < id2 ? (id1, id2) : (id2, id1);

                    if (!pairCounts.ContainsKey(key))
                        pairCounts[key] = 0;
                    
                    pairCounts[key]++;
                }
            }
        }

        var topPairs = pairCounts.OrderByDescending(kvp => kvp.Value).Take(5);

        var recommendations = new List<ComboRecommendationDto>();
        foreach (var pair in topPairs)
        {
            var item1 = itemInfo[pair.Key.Item1];
            var item2 = itemInfo[pair.Key.Item2];
            var totalOriginal = item1.Price + item2.Price;
            
            var discounted = totalOriginal * 0.9m;
            var suggested = Math.Round(discounted / 1000m) * 1000m;

            recommendations.Add(new ComboRecommendationDto
            {
                Item1Name = item1.Name,
                Item2Name = item2.Name,
                Frequency = pair.Value,
                TotalOriginalPrice = totalOriginal,
                SuggestedPrice = suggested,
                Reason = $"Hai món này được gọi cùng nhau {pair.Value} lần trong 30 ngày qua."
            });
        }

        return recommendations;
    }
}




