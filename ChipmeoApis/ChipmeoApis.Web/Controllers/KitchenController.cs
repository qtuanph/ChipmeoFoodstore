using ChipmeoApis.Usecase.Interfaces;
using ChipmeoApis.Web.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ChipmeoApis.Web.Hubs;

namespace ChipmeoApis.Web.Controllers;

[ApiController]
[Route("api/kitchen")]
[Authorize]
public class KitchenController(IOrderService orderService, IHubContext<AppHub> hubContext) : ControllerBase
{
    private readonly IOrderService _orderService = orderService;
    private readonly IHubContext<AppHub> _hubContext = hubContext;

    /// <summary>
    /// Get all paid orders for kitchen (status: paid or preparing)
    /// </summary>
    [HttpGet("orders")]
    [RequirePermission("order.view")]
    public async Task<IActionResult> GetOrders(CancellationToken cancellationToken)
    {
        var orders = await _orderService.GetPaidOrdersForKitchenAsync(cancellationToken);
        return Ok(orders);
    }

    /// <summary>
    /// Get only orders that are ready to be prepared (status: paid)
    /// </summary>
    [HttpGet("orders/pending")]
    [RequirePermission("order.view")]
    public async Task<IActionResult> GetPendingOrders(CancellationToken cancellationToken)
    {
        var orders = await _orderService.GetByStatusAsync("paid", cancellationToken);
        return Ok(orders);
    }

    /// <summary>
    /// Get orders currently being prepared (status: preparing)
    /// </summary>
    [HttpGet("orders/preparing")]
    [RequirePermission("order.view")]
    public async Task<IActionResult> GetPreparingOrders(CancellationToken cancellationToken)
    {
        var orders = await _orderService.GetByStatusAsync("preparing", cancellationToken);
        return Ok(orders);
    }

    /// <summary>
    /// Start preparing an order (paid → preparing)
    /// </summary>
    [HttpPut("orders/{id}/start")]
    [RequirePermission("order.update")]
    public async Task<IActionResult> StartPreparing(int id, CancellationToken cancellationToken)
    {
        try
        {
            var success = await _orderService.UpdateKitchenStatusAsync(id, "preparing", cancellationToken);
            if (!success) return NotFound();

            // Notify all clients about status change
            await _hubContext.Clients.All.SendAsync("ReceiveOrderUpdate", new { Id = id, Status = "preparing" }, cancellationToken);
            
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Complete an order (preparing → completed)
    /// </summary>
    [HttpPut("orders/{id}/complete")]
    [RequirePermission("order.update")]
    public async Task<IActionResult> CompleteOrder(int id, CancellationToken cancellationToken)
    {
        try
        {
            var success = await _orderService.UpdateKitchenStatusAsync(id, "served", cancellationToken);
            if (!success) return NotFound();

            // Notify all clients about completion
            await _hubContext.Clients.All.SendAsync("ReceiveOrderUpdate", new { Id = id, Status = "served" }, cancellationToken);
            
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}




