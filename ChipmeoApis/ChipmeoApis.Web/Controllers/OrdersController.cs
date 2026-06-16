using ChipmeoApis.Core.Constants;
using ChipmeoApis.Usecase.Interfaces;
using ChipmeoApis.Usecase.DTOs.Order;
using ChipmeoApis.Web.Authorization;
using ChipmeoApis.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ChipmeoApis.Web.Hubs;
using ChipmeoApis.Web.ApiResponse;

namespace ChipmeoApis.Web.Controllers;

[ApiController]
[Route("api/admin/orders")]
[Authorize]
public class OrdersController(IOrderService service, IHubContext<AppHub> hubContext) : ControllerBase
{
    [HttpGet]
    [RequirePermission("order.view")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var orders = await service.GetAllAsync(cancellationToken);
        return ApiResult.Success(orders);
    }

    [HttpGet("{id:int}")]
    [RequirePermission("order.view")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var order = await service.GetByIdAsync(id, cancellationToken);
        if (order == null) return ApiResult.NotFound();
        return ApiResult.Success(order);
    }

    [HttpPost]
    [RequirePermission("order.create")]
    public async Task<IActionResult> Create([FromBody] CreateOrderDto dto, CancellationToken cancellationToken)
    {
        return await ProcessCreateOrder(dto, cancellationToken);
    }

    [HttpPost("/api/pos/orders")]
    [RequirePermission("order.create")]
    public async Task<IActionResult> PosCreate([FromBody] CreateOrderDto dto, CancellationToken cancellationToken)
    {
        return await ProcessCreateOrder(dto, cancellationToken);
    }

    [HttpPut("{id:int}")]
    [RequirePermission("order.update")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateOrderDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var employeeId = User.GetUserId();
            if (employeeId == 0) return Unauthorized();

            var updated = await service.UpdateAsync(id, dto, employeeId, cancellationToken);
            await hubContext.Clients.All.SendAsync("ReceiveOrderUpdate", updated, cancellationToken);
            await hubContext.Clients.All.SendAsync("ReceiveTableUpdate", cancellationToken);
            return ApiResult.Success(updated);
        }
        catch (Exception ex)
        {
            return ApiResult.BadRequest(ex.Message);
        }
    }

    [HttpPost("/api/pos/orders/{id}/payment")]
    [RequirePermission("order.update")]
    public async Task<IActionResult> ProcessPayment(int id, [FromBody] ProcessPaymentDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var employeeId = User.GetUserId();
            var order = await service.ProcessPaymentAsync(id, dto, employeeId > 0 ? employeeId : null, cancellationToken);

            await hubContext.Clients.All.SendAsync("ReceiveOrderUpdate", order, cancellationToken);
            await hubContext.Clients.All.SendAsync("ReceiveTableUpdate", cancellationToken);
            await hubContext.Clients.Group("Kitchen").SendAsync("ReceiveNewOrder", order, cancellationToken);
            return ApiResult.Success(order);
        }
        catch (Exception ex)
        {
            return ApiResult.BadRequest(ex.Message);
        }
    }

    private async Task<IActionResult> ProcessCreateOrder(CreateOrderDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var employeeId = User.GetUserId();
            if (employeeId == 0) return Unauthorized();

            var created = await service.CreateAsync(dto, employeeId, cancellationToken);
            await hubContext.Clients.All.SendAsync("ReceiveOrderUpdate", created, cancellationToken);
            await hubContext.Clients.All.SendAsync("ReceiveTableUpdate", cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            return ApiResult.BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}/status")]
    [RequirePermission("order.update")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusRequest request, CancellationToken cancellationToken)
    {
        return await ProcessUpdateStatus(id, request, cancellationToken);
    }

    [HttpPut("/api/pos/orders/{id}/status")]
    [RequirePermission("order.update")]
    public async Task<IActionResult> PosUpdateStatus(int id, [FromBody] UpdateOrderStatusRequest request, CancellationToken cancellationToken)
    {
        return await ProcessUpdateStatus(id, request, cancellationToken);
    }

    private async Task<IActionResult> ProcessUpdateStatus(int id, UpdateOrderStatusRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var employeeId = User.GetUserId();
            var result = await service.UpdateStatusAsync(id, request.Status, employeeId > 0 ? employeeId : null, request.PaymentMethod, request.PaymentAmount, cancellationToken);

            if (!result) return ApiResult.NotFound();
            await hubContext.Clients.All.SendAsync("ReceiveOrderUpdate", new { Id = id, Status = request.Status }, cancellationToken);
            if (request.Status == OrderStatus.Paid)
                await hubContext.Clients.All.SendAsync("ReceiveSourceUpdate", cancellationToken);

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Failure(new ErrorDetail { Code = "INTERNAL_ERROR", Message = ex.Message }));
        }
    }

    [HttpGet("status/{status}")]
    [RequirePermission("order.view")]
    public async Task<IActionResult> GetByStatus(string status, CancellationToken cancellationToken)
    {
        var orders = await service.GetByStatusAsync(status, cancellationToken);
        return ApiResult.Success(orders);
    }

    [HttpGet("date-range")]
    [RequirePermission("order.view")]
    public async Task<IActionResult> GetByDateRange([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate, CancellationToken cancellationToken)
    {
        var orders = await service.GetByDateRangeAsync(fromDate, toDate, cancellationToken);
        return ApiResult.Success(orders);
    }

    [HttpPut("{id}/set-unpaid")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SetUnpaid(int id, CancellationToken cancellationToken)
    {
        try
        {
            var order = await service.GetByIdAsync(id, cancellationToken: cancellationToken);
            if (order == null) return ApiResult.NotFound();

            var employeeId = User.GetUserId();
            var result = await service.UpdateStatusAsync(id, OrderStatus.Pending, employeeId > 0 ? employeeId : null, null, null, cancellationToken);
            if (!result) return ApiResult.BadRequest("Failed to update order status");

            await hubContext.Clients.All.SendAsync("ReceiveOrderUpdate", new { Id = id, Status = OrderStatus.Pending }, cancellationToken);
            return ApiResult.Success(new { message = "Order set to unpaid successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Failure(new ErrorDetail { Code = "INTERNAL_ERROR", Message = ex.Message }));
        }
    }

    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await service.GetPagedAsync(page, pageSize, fromDate, toDate, cancellationToken);
        return ApiResult.Paged(items.ToList(), page, pageSize, totalCount);
    }

    [HttpDelete("{id:int}")]
    [RequirePermission("order.delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await service.DeleteAsync(id, cancellationToken);
        if (!result) return ApiResult.NotFound();
        return NoContent();
    }

    public class UpdateOrderStatusRequest
    {
        public string Status { get; set; } = null!;
        public string? PaymentMethod { get; set; }
        public decimal? PaymentAmount { get; set; }
    }
}
