using FoodstoreApi.Usecase.Interfaces;
using FoodstoreApi.Web.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FoodstoreApi.Web.ApiResponse;

namespace FoodstoreApi.Web.Controllers;

[ApiController]
[Route("api/admin/dashboard")]
public class DashboardController(IReportService reportService) : ControllerBase
{
    [HttpGet("overview")]
    [RequirePermission("dashboard.view")]
    public async Task<IActionResult> GetOverview(CancellationToken cancellationToken)
    {
        return ApiResult.Success(await reportService.GetOverviewAsync(cancellationToken));
    }

    [HttpGet("stats")]
    [RequirePermission("dashboard.view")]
    public async Task<IActionResult> GetStats(CancellationToken cancellationToken)
    {
        return ApiResult.Success(await reportService.GetDashboardStatsAsync(cancellationToken));
    }

    [HttpGet("analytics")]
    [RequirePermission("analytics.view")]
    public async Task<IActionResult> GetAnalytics(
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] string groupBy = "day",
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var end = toDate?.Date.AddDays(1).AddTicks(-1) ?? now.Date.AddDays(1).AddTicks(-1);
        var start = fromDate ?? now.Date.AddDays(-7);

        return ApiResult.Success(await reportService.GetStatsAsync(start, end, groupBy, cancellationToken));
    }

    [HttpGet("forecast")]
    [RequirePermission("analytics.view")]
    public async Task<IActionResult> GetForecast([FromQuery] int horizon = 7, CancellationToken cancellationToken = default)
    {
        try
        {
            return ApiResult.Success(await reportService.GetForecastAsync(horizon, cancellationToken));
        }
        catch
        {
            return ApiResult.BadRequest("Could not generate forecast. Need more historical data.");
        }
    }

    [HttpGet("recommendations")]
    [RequirePermission("analytics.view")]
    public async Task<IActionResult> GetRecommendations(CancellationToken cancellationToken)
    {
        return ApiResult.Success(await reportService.GetComboRecommendationsAsync(cancellationToken));
    }
}
