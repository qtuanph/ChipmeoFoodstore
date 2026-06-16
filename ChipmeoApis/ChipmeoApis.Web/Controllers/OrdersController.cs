using ChipmeoApis.Usecase.Interfaces;
using ChipmeoApis.Web.Authorization;
using ChipmeoApis.Usecase.DTOs.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

using Microsoft.AspNetCore.SignalR;
using ChipmeoApis.Web.Hubs;

namespace ChipmeoApis.Web.Controllers;

[ApiController]
[Route("admin/orders")]
[Authorize]
public class OrdersController(IOrderService service, IHubContext<AppHub> hubContext) : ControllerBase
{
    private readonly IOrderService _service = service;
    private readonly IHubContext<AppHub> _hubContext = hubContext;

    [HttpGet]
    [RequirePermission("order.view")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var orders = await _service.GetAllAsync(cancellationToken);
        return Ok(orders);
    }

    [HttpGet("{id:int}")]
    [RequirePermission("order.view")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        int? userId = null;
        if (int.TryParse(userIdClaim, out int parsedId))
        {
            userId = parsedId;
        }

        // Extract permissions from claims
        var permissions = User.FindAll("permission").Select(c => c.Value).ToList();

        // Pass context to Service for Anti-IDOR check
        var order = await _service.GetByIdAsync(id, userId, permissions, cancellationToken);
        
        if (order == null) return NotFound(); // Returns 404 (Stealth Mode) if unauthorized or not found
        return Ok(order);
    }

    [HttpPost]
    [RequirePermission("order.create")]
    public async Task<IActionResult> Create([FromBody] CreateOrderDto dto, CancellationToken cancellationToken)
    {
        return await ProcessCreateOrder(dto, cancellationToken);
    }

    [HttpPost("/pos/orders")]
    [RequirePermission("order.create")]
    public async Task<IActionResult> PosCreate([FromBody] CreateOrderDto dto, CancellationToken cancellationToken)
    {
        return await ProcessCreateOrder(dto, cancellationToken);
    }

    [HttpPut("/pos/orders/{id}")]
    [RequirePermission("order.update")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateOrderDto dto, CancellationToken cancellationToken)
    {
        try
        {
            // Get employee ID from JWT claims
            var employeeIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(employeeIdClaim) || !int.TryParse(employeeIdClaim, out int employeeId))
            {
                return Unauthorized(new { error = "Invalid employee ID" });
            }

            var updated = await _service.UpdateAsync(id, dto, employeeId, cancellationToken);
            
            // Notify all clients about order update
            await _hubContext.Clients.All.SendAsync("ReceiveOrderUpdate", updated, cancellationToken);
            await _hubContext.Clients.All.SendAsync("ReceiveTableUpdate", cancellationToken);
            
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Process payment for a pending order
    /// </summary>
    [HttpPost("/pos/orders/{id}/payment")]
    [RequirePermission("order.update")]
    public async Task<IActionResult> ProcessPayment(int id, [FromBody] ProcessPaymentDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var employeeIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int? employeeId = null;
            if (int.TryParse(employeeIdClaim, out int parsedId))
            {
                employeeId = parsedId;
            }

            var order = await _service.ProcessPaymentAsync(id, dto, employeeId, cancellationToken);
            
            // Notify all clients about payment completion
            await _hubContext.Clients.All.SendAsync("ReceiveOrderUpdate", order, cancellationToken);
            await _hubContext.Clients.All.SendAsync("ReceiveTableUpdate", cancellationToken);
            
            // Notify kitchen about new paid order
            await _hubContext.Clients.Group("Kitchen").SendAsync("ReceiveNewOrder", order, cancellationToken);
            
            return Ok(order);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    private async Task<IActionResult> ProcessCreateOrder(CreateOrderDto dto, CancellationToken cancellationToken)
    {
        try
        {
            // Get employee ID from JWT claims
            var employeeIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(employeeIdClaim) || !int.TryParse(employeeIdClaim, out int employeeId))
            {
                return Unauthorized(new { error = "Invalid employee ID" });
            }

            var created = await _service.CreateAsync(dto, employeeId, cancellationToken);
            await _hubContext.Clients.All.SendAsync("ReceiveOrderUpdate", created, cancellationToken);
            await _hubContext.Clients.All.SendAsync("ReceiveTableUpdate", cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}/status")]
    [RequirePermission("order.update")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusRequest request, CancellationToken cancellationToken)
    {
        return await ProcessUpdateStatus(id, request, cancellationToken);
    }

    [HttpPut("/pos/orders/{id}/status")]
    [RequirePermission("order.update")]
    public async Task<IActionResult> PosUpdateStatus(int id, [FromBody] UpdateOrderStatusRequest request, CancellationToken cancellationToken)
    {
        return await ProcessUpdateStatus(id, request, cancellationToken);
    }

    private async Task<IActionResult> ProcessUpdateStatus(int id, UpdateOrderStatusRequest request, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine($"[OrdersController] UpdateStatus called: OrderId={id}, Status={request.Status}, Method={request.PaymentMethod}, Amount={request.PaymentAmount}");
            
            var employeeIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int? employeeId = null;
            if (int.TryParse(employeeIdClaim, out int parsedId))
            {
                employeeId = parsedId;
            }

            var result = await _service.UpdateStatusAsync(id, request.Status, employeeId, request.PaymentMethod, request.PaymentAmount, cancellationToken);
            
            if (!result) 
            {
                Console.WriteLine($"[OrdersController] Order not found: {id}");
                return NotFound();
            }
            
            Console.WriteLine($"[OrdersController] UpdateStatus successful for order {id}");
            await _hubContext.Clients.All.SendAsync("ReceiveOrderUpdate", new { Id = id, Status = request.Status }, cancellationToken);
            
            // If status is paid, notify table update
            if (request.Status == "paid")
            {
                await _hubContext.Clients.All.SendAsync("ReceiveSourceUpdate", cancellationToken);
            }
            
            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[OrdersController] ERROR in UpdateStatus: {ex.Message}");
            Console.WriteLine($"[OrdersController] Stack trace: {ex.StackTrace}");
            return StatusCode(500, new { error = ex.Message, detail = ex.InnerException?.Message });
        }
    }

    [HttpGet("status/{status}")]
    [RequirePermission("order.view")]
    public async Task<IActionResult> GetByStatus(string status, CancellationToken cancellationToken)
    {
        var orders = await _service.GetByStatusAsync(status, cancellationToken);
        return Ok(orders);
    }

    [HttpGet("date-range")]
    [RequirePermission("order.view")]
    public async Task<IActionResult> GetByDateRange([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate, CancellationToken cancellationToken)
    {
        var orders = await _service.GetByDateRangeAsync(fromDate, toDate, cancellationToken);
        return Ok(orders);
    }

    [HttpPut("{id}/set-unpaid")]
    [RequirePermission("order.update")] // Or a specific permission if needed, but order.update seems appropriate + Admin role check
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SetUnpaid(int id, CancellationToken cancellationToken)
    {
        try
        {
            var order = await _service.GetByIdAsync(id, cancellationToken: cancellationToken);
            if (order == null) return NotFound();

            var employeeIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int? employeeId = null;
            if (int.TryParse(employeeIdClaim, out int parsedId))
            {
                employeeId = parsedId;
            }

            // Update status to pending (unpaid)
            // We pass null for payment method and amount since it's unpaid now
            var result = await _service.UpdateStatusAsync(id, "pending", employeeId, null, null, cancellationToken);

            if (!result) return BadRequest(new { error = "Failed to update order status" });

            await _hubContext.Clients.All.SendAsync("ReceiveOrderUpdate", new { Id = id, Status = "pending" }, cancellationToken);
            return Ok(new { message = "Order set to unpaid successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
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
        var (items, totalCount) = await _service.GetPagedAsync(page, pageSize, fromDate, toDate, cancellationToken);
        return Ok(new { items, totalCount, page, pageSize, totalPages = (int)Math.Ceiling((double)totalCount / pageSize) });
    }

    [HttpDelete("{id:int}")]
    [RequirePermission("order.delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(id, cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    public class UpdateOrderStatusRequest
    {
        public string Status { get; set; } = null!;
        public string? PaymentMethod { get; set; }
        public decimal? PaymentAmount { get; set; }
    }
}




