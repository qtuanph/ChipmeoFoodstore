using ChipmeoApis.Usecase.Interfaces;
using ChipmeoApis.Usecase.DTOs.Report;
using ChipmeoApis.Core.Utils;
using ChipmeoApis.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ChipmeoApis.Infrastructure.Repositories;

public class ReportRepository(StoreDbContext context) : IReportRepository
{
    private readonly StoreDbContext _context = context;

    public async Task<DashboardOverviewDto> GetOverviewAsync(CancellationToken cancellationToken = default)
    {
        var today = TimeUtils.GetVietnamTime().Date;
        var tomorrow = today.AddDays(1);

        var todayOrdersQuery = _context.Orders.AsNoTracking()
            .Where(o => o.CreatedAt >= today && o.CreatedAt < tomorrow && (o.Status == "paid" || o.Status == "preparing" || o.Status == "served"));

        var todayRevenue = await todayOrdersQuery.SumAsync(o => o.TotalAmount ?? 0, cancellationToken);
        var todayVat = await todayOrdersQuery.SumAsync(o => o.VatAmount ?? 0, cancellationToken);
        var todayOrdersCount = await todayOrdersQuery.CountAsync(cancellationToken);

        var avgOrderValue = todayOrdersCount > 0 ? todayRevenue / todayOrdersCount : 0;

        var hourlyOrders = await todayOrdersQuery
            .Where(o => o.CreatedAt.HasValue)
            .GroupBy(o => o.CreatedAt!.Value.Hour)
            .Select(g => new { Hour = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .FirstOrDefaultAsync(cancellationToken);

        var peakHour = hourlyOrders != null ? $"{hourlyOrders.Hour}:00 - {hourlyOrders.Hour + 1}:00" : "N/A";

        var popularItemsList = await _context.OrderItems.AsNoTracking()
            .Where(i => i.Order.CreatedAt >= today && i.Order.CreatedAt < tomorrow && (i.Order.Status == "paid" || i.Order.Status == "preparing" || i.Order.Status == "served"))
            .GroupBy(i => i.MenuItemName)
            .Select(g => new { Name = g.Key, Sold = g.Sum(i => i.Quantity) })
            .OrderByDescending(x => x.Sold)
            .Take(5)
            .ToListAsync(cancellationToken);

        var popularItems = popularItemsList.Select(x => new PopularItemDto(x.Name, x.Sold)).ToList();

        var serviceStats = await todayOrdersQuery
            .GroupBy(o => o.Source != null ? o.Source.Name : "Khác")
            .Select(g => new { Name = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync(cancellationToken);

        var sourceStats = serviceStats.Select(x => new PopularItemDto(x.Name, x.Count)).ToList();

        return new DashboardOverviewDto(
            todayRevenue,
            todayVat,
            todayOrdersCount,
            avgOrderValue,
            peakHour,
            popularItems,
            sourceStats
        );
    }

    public async Task<DashboardStatsDto> GetStatsAsync(DateTime fromDate, DateTime toDate, string groupBy, CancellationToken cancellationToken = default)
    {
        var ordersQuery = _context.Orders.AsNoTracking()
            .Where(o => o.CreatedAt >= fromDate && o.CreatedAt <= toDate && (o.Status == "paid" || o.Status == "preparing" || o.Status == "served"));

        var totalRevenue = await ordersQuery.SumAsync(o => o.TotalAmount ?? 0, cancellationToken);
        var totalVat = await ordersQuery.SumAsync(o => o.VatAmount ?? 0, cancellationToken);
        var totalOrders = await ordersQuery.CountAsync(cancellationToken);

        List<ChartDataDto> revenueChart;
        List<ChartDataDto> ordersChart;

        var ordersList = await ordersQuery
            .Select(o => new { o.CreatedAt, o.TotalAmount })
            .ToListAsync(cancellationToken);

        if (groupBy == "month")
        {
            var grouped = ordersList
                .GroupBy(o => new { o.CreatedAt!.Value.Year, o.CreatedAt!.Value.Month })
                .Select(g => new
                {
                    Label = $"{g.Key.Year}-{g.Key.Month:D2}",
                    Revenue = g.Sum(x => x.TotalAmount ?? 0),
                    Orders = g.Count()
                })
                .OrderBy(x => x.Label)
                .ToList();

            revenueChart = grouped.Select(x => new ChartDataDto(x.Label, x.Revenue)).ToList();
            ordersChart = grouped.Select(x => new ChartDataDto(x.Label, x.Orders)).ToList();
        }
        else
        {
            var grouped = ordersList
                .GroupBy(o => o.CreatedAt!.Value.Date)
                .Select(g => new
                {
                    Label = g.Key.ToString("yyyy-MM-dd"),
                    Revenue = g.Sum(x => x.TotalAmount ?? 0),
                    Orders = g.Count()
                })
                .OrderBy(x => x.Label)
                .ToList();

            revenueChart = grouped.Select(x => new ChartDataDto(x.Label, x.Revenue)).ToList();
            ordersChart = grouped.Select(x => new ChartDataDto(x.Label, x.Orders)).ToList();
        }

        var topItemsList = await _context.OrderItems.AsNoTracking()
            .Where(i => i.Order.CreatedAt >= fromDate && i.Order.CreatedAt <= toDate && (i.Order.Status == "paid" || i.Order.Status == "preparing" || i.Order.Status == "served") && i.MenuItemId != null)
            .GroupBy(i => i.MenuItemName)
            .Select(g => new { Name = g.Key, Sold = g.Sum(i => i.Quantity) })
            .OrderByDescending(x => x.Sold)
            .Take(10)
            .ToListAsync(cancellationToken);

        var topItems = topItemsList.Select(x => new PopularItemDto(x.Name, x.Sold)).ToList();

        var rawComboItems = await _context.OrderItems.AsNoTracking()
            .Where(i => i.Order.CreatedAt >= fromDate && i.Order.CreatedAt <= toDate && (i.Order.Status == "paid" || i.Order.Status == "preparing" || i.Order.Status == "served") && (i.ComboId != null || i.MenuItemId == null))
            .Include(i => i.Combo)
            .Select(i => new 
            { 
                i.ComboId,
                ComboName = i.Combo != null ? i.Combo.Name : null,
                i.MenuItemName,
                i.Quantity
            })
            .ToListAsync(cancellationToken);

        var popularCombos = rawComboItems
            .Select(i => 
            {
                if (!string.IsNullOrEmpty(i.ComboName)) return new { Name = i.ComboName, i.Quantity };
                if (!string.IsNullOrEmpty(i.MenuItemName)) return new { Name = i.MenuItemName, i.Quantity };
                return new { Name = "Unknown Combo", i.Quantity };
            })
            .GroupBy(x => x.Name)
            .Select(g => new PopularItemDto(g.Key, g.Sum(x => x.Quantity)))
            .OrderByDescending(x => x.Sold)
            .Take(10)
            .ToList();

        var serviceStats = await ordersQuery
            .GroupBy(o => o.Source != null ? o.Source.Name : "Khác")
            .Select(g => new { Name = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync(cancellationToken);

        var sourceStats = serviceStats.Select(x => new PopularItemDto(x.Name, x.Count)).ToList();

        return new DashboardStatsDto(
            totalRevenue,
            totalVat,
            totalOrders,
            revenueChart,
            ordersChart,
            topItems,
            popularCombos,
            sourceStats
        );
    }

    public async Task<List<SalesDataDto>> GetSalesDataAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        return await _context.Orders.AsNoTracking()
            .Where(o => o.CreatedAt >= fromDate && o.CreatedAt < toDate && (o.Status == "paid" || o.Status == "preparing" || o.Status == "served"))
            .GroupBy(o => o.CreatedAt!.Value.Date)
            .Select(g => new SalesDataDto
            {
                Date = g.Key,
                Revenue = (float)g.Sum(o => o.TotalAmount ?? 0)
            })
            .OrderBy(x => x.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task<DashboardLegacyDto> GetLegacyDashboardStatsAsync(CancellationToken cancellationToken = default)
    {
        var now = TimeUtils.GetVietnamTime();
        var todayStart = now.Date;
        var monthStart = new DateTime(now.Year, now.Month, 1);

        var paidOrders = await _context.Orders.AsNoTracking()
            .Where(o => o.Status == "paid" || o.Status == "preparing" || o.Status == "served")
            .Include(o => o.OrderItems)
            .Include(o => o.Source)
            .ToListAsync(cancellationToken);

        var todayOrders = paidOrders.Where(o => o.CreatedAt.HasValue && o.CreatedAt.Value >= todayStart).ToList();
        var todayRevenue = todayOrders.Sum(o => o.TotalAmount ?? 0);

        var monthOrders = paidOrders.Where(o => o.CreatedAt.HasValue && o.CreatedAt.Value >= monthStart).ToList();
        var monthRevenue = monthOrders.Sum(o => o.TotalAmount ?? 0);

        var totalRevenue = paidOrders.Sum(o => o.TotalAmount ?? 0);

        var popularItemsList = await _context.OrderItems.AsNoTracking()
            .Where(oi => (oi.Order.Status == "paid" || oi.Order.Status == "preparing" || oi.Order.Status == "served") && oi.Order.CreatedAt >= todayStart && oi.Order.CreatedAt < todayStart.AddDays(1) && oi.MenuItemId != null)
            .GroupBy(oi => new { oi.MenuItemId, Name = oi.MenuItemName })
            .Select(g => new
            {
                Name = g.Key.Name,
                Quantity = g.Sum(oi => oi.Quantity),
                Revenue = g.Sum(oi => oi.Quantity * oi.UnitPrice)
            })
            .OrderByDescending(x => x.Quantity)
            .Take(5)
            .ToListAsync(cancellationToken);

        var popularItems = popularItemsList.Select(x => new PopularItemLegacyDto(x.Name, x.Quantity, x.Revenue)).ToList();

        var popularCombosList = await _context.OrderItems.AsNoTracking()
            .Where(oi => (oi.Order.Status == "paid" || oi.Order.Status == "preparing" || oi.Order.Status == "served") && oi.Order.CreatedAt >= todayStart && oi.Order.CreatedAt < todayStart.AddDays(1) && oi.ComboId != null)
            .GroupBy(oi => new { oi.ComboId, Name = oi.MenuItemName })
            .Select(g => new
            {
                Name = g.Key.Name,
                Quantity = g.Sum(oi => oi.Quantity),
                Revenue = g.Sum(oi => oi.Quantity * oi.UnitPrice)
            })
            .OrderByDescending(x => x.Quantity)
            .Take(5)
            .ToListAsync(cancellationToken);

        var popularCombos = popularCombosList.Select(x => new PopularItemLegacyDto(x.Name, x.Quantity, x.Revenue)).ToList();

        var last7Days = Enumerable.Range(0, 7).Select(i => todayStart.AddDays(-i)).ToList();
        var dailyRevenue = last7Days.Select(day =>
        {
            var dayOrders = paidOrders.Where(o => o.CreatedAt.HasValue && o.CreatedAt.Value.Date == day).ToList();
            return new DailyRevenueDto(
                day.ToString("yyyy-MM-dd"),
                dayOrders.Sum(o => o.TotalAmount ?? 0),
                dayOrders.Count
            );
        }).OrderBy(x => x.Date).ToList();

        var last6Months = Enumerable.Range(0, 6).Select(i =>
        {
            var month = monthStart.AddMonths(-i);
            return new DateTime(month.Year, month.Month, 1);
        }).ToList();

        var monthlyRevenue = last6Months.Select(month =>
        {
            var nextMonth = month.AddMonths(1);
            var mOrders = paidOrders.Where(o => o.CreatedAt.HasValue && o.CreatedAt.Value >= month && o.CreatedAt.Value < nextMonth).ToList();
            return new MonthlyRevenueDto(
                month.ToString("yyyy-MM"),
                mOrders.Sum(o => o.TotalAmount ?? 0),
                mOrders.Count
            );
        }).OrderBy(x => x.Month).ToList();

        var avgOrderValue = paidOrders.Any() ? paidOrders.Average(o => o.TotalAmount ?? 0) : 0;

        var hourlyOrders = paidOrders
            .Where(o => o.CreatedAt.HasValue)
            .GroupBy(o => o.CreatedAt!.Value.Hour)
            .Select(g => new { Hour = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .FirstOrDefault();

        var peakHour = hourlyOrders != null ? $"{hourlyOrders.Hour}:00 - {hourlyOrders.Hour + 1}:00" : "N/A";

        var payments = await _context.Payments.AsNoTracking()
            .Where(p => p.Status == "success")
            .GroupBy(p => p.Method)
            .Select(g => new
            {
                Method = g.Key,
                Amount = g.Sum(p => p.Amount)
            })
            .ToListAsync(cancellationToken);

        var paymentBreakdown = payments.ToDictionary(p => p.Method, p => p.Amount);

        var totalCustomers = paidOrders.Select(o => o.SourceId).Distinct().Count();

        var serviceStats = todayOrders
            .GroupBy(o => o.Source != null ? o.Source.Name : "Khác")
            .Select(g => new { Name = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToList();

        var sourceStats = serviceStats.Select(x => new PopularItemLegacyDto(x.Name, x.Count, 0)).ToList();

        return new DashboardLegacyDto(
            new { Revenue = todayRevenue, Orders = todayOrders.Count },
            new { Revenue = monthRevenue, Orders = monthOrders.Count },
            new { Revenue = totalRevenue, Orders = paidOrders.Count },
            popularItems,
            popularCombos,
            dailyRevenue,
            monthlyRevenue,
            avgOrderValue,
            peakHour,
            paymentBreakdown,
            totalCustomers,
            sourceStats
        );
    }

    public async Task<List<ComboRecommendationDataDto>> GetComboRecommendationDataAsync(DateTime fromDate, CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .Where(o => o.CreatedAt >= fromDate && (o.Status == "paid" || o.Status == "preparing" || o.Status == "served"))
            .SelectMany(o => o.OrderItems
                .Where(oi => oi.MenuItemId != null)
                .Select(oi => new ComboRecommendationDataDto
                {
                    MenuItemId = oi.MenuItemId,
                    MenuItemName = oi.MenuItemName,
                    UnitPrice = oi.UnitPrice,
                    OrderId = o.Id
                }))
            .ToListAsync(cancellationToken);
    }
}




