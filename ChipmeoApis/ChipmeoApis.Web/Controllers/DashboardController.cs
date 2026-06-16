using ChipmeoApis.Usecase.Interfaces;
using ChipmeoApis.Web.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChipmeoApis.Core.Utils;

namespace ChipmeoApis.Web.Controllers;

[ApiController]
[Route("admin/dashboard")]
public class DashboardController(IReportService reportService) : ControllerBase
{
    private readonly IReportService _reportService = reportService;

    [HttpGet("overview")]
    [RequirePermission("dashboard.view")]
    public async Task<IActionResult> GetOverview(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _reportService.GetOverviewAsync(cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DashboardController] Error in GetOverview: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("stats")]
    [RequirePermission("dashboard.view")]
    public async Task<IActionResult> GetStats(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _reportService.GetDashboardStatsAsync(cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DashboardController] Error in GetStats: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("analytics")]
    [RequirePermission("analytics.view")]
    public async Task<IActionResult> GetAnalytics(
        [FromQuery] DateTime? fromDate, 
        [FromQuery] DateTime? toDate, 
        [FromQuery] string groupBy = "day", 
        CancellationToken cancellationToken = default)
    {
        var now = TimeUtils.GetVietnamTime();
        var end = toDate.HasValue ? toDate.Value.Date.AddDays(1).AddTicks(-1) : now.Date.AddDays(1).AddTicks(-1);
        var start = fromDate ?? now.Date.AddDays(-7);

        try
        {
            var result = await _reportService.GetStatsAsync(start, end, groupBy, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DashboardController] Error in GetAnalytics: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("forecast")]
    [RequirePermission("analytics.view")]
    public async Task<IActionResult> GetForecast([FromQuery] int horizon = 7, CancellationToken cancellationToken = default)
    {
        try 
        {
            var result = await _reportService.GetForecastAsync(horizon, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            // Fallback or error handling if ML fails (e.g. not enough data)
            return BadRequest(new { error = "Could not generate forecast. Need more historical data.", details = ex.Message });
        }
    }
    [HttpGet("recommendations")]
    [RequirePermission("analytics.view")]
    public async Task<IActionResult> GetRecommendations(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _reportService.GetComboRecommendationsAsync(cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DashboardController] Error in GetRecommendations: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            return StatusCode(500, new { error = ex.Message });
        }
    }
}




