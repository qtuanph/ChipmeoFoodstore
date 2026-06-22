using FoodstoreApi.Usecase.DTOs.EInvoice;
using FoodstoreApi.Usecase.Interfaces;
using FoodstoreApi.Web.ApiResponse;
using FoodstoreApi.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodstoreApi.Web.Controllers;

[ApiController]
[Route("api/admin/e-invoice")]
[Authorize]
public class EInvoiceController(IEInvoiceService service) : ControllerBase
{
    private readonly IEInvoiceService _service = service;

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard(CancellationToken ct)
    {
        var data = await _service.GetDashboardAsync(ct);
        return ApiResult.Success(data);
    }

    [HttpGet("providers")]
    public async Task<IActionResult> GetAllProviders(CancellationToken ct)
    {
        var data = await _service.GetAllProvidersAsync(ct);
        return ApiResult.Success(data);
    }

    [HttpGet("providers/{id:guid}")]
    public async Task<IActionResult> GetProviderById(Guid id, CancellationToken ct)
    {
        var data = await _service.GetProviderByIdAsync(id, ct);
        if (data == null) return ApiResult.NotFound("Provider not found");
        return ApiResult.Success(data);
    }

    [HttpPost("providers")]
    public async Task<IActionResult> CreateProvider([FromBody] CreateEInvoiceProviderDto dto, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return ApiResult.BadRequest("Provider name is required");

        var userId = User.GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        var data = await _service.CreateProviderAsync(dto, userId, ct);
        return CreatedAtAction(nameof(GetProviderById), new { id = data.Id }, ApiResult.Success(data));
    }

    [HttpPut("providers/{id:guid}")]
    public async Task<IActionResult> UpdateProvider(Guid id, [FromBody] UpdateEInvoiceProviderDto dto, CancellationToken ct)
    {
        var userId = User.GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        try
        {
            await _service.UpdateProviderAsync(id, dto, userId, ct);
            return ApiResult.Success(new { message = "Provider updated" });
        }
        catch (Exception ex)
        {
            return ApiResult.NotFound(ex.Message);
        }
    }

    [HttpDelete("providers/{id:guid}")]
    public async Task<IActionResult> DeleteProvider(Guid id, CancellationToken ct)
    {
        try
        {
            await _service.DeleteProviderAsync(id, ct);
            return ApiResult.Success(new { message = "Provider deleted" });
        }
        catch (Exception ex)
        {
            return ApiResult.NotFound(ex.Message);
        }
    }

    [HttpPost("providers/{id:guid}/test")]
    public async Task<IActionResult> TestProviderConnection(Guid id, CancellationToken ct)
    {
        try
        {
            var ok = await _service.TestProviderConnectionAsync(id, ct);
            return ApiResult.Success(new { connected = ok });
        }
        catch (Exception ex)
        {
            return ApiResult.BadRequest(ex.Message);
        }
    }

    [HttpGet("invoices")]
    public async Task<IActionResult> GetAllInvoices(CancellationToken ct)
    {
        var data = await _service.GetAllInvoicesAsync(ct);
        return ApiResult.Success(data);
    }

    [HttpGet("invoices/{id:guid}")]
    public async Task<IActionResult> GetInvoiceById(Guid id, CancellationToken ct)
    {
        var data = await _service.GetInvoiceByIdAsync(id, ct);
        if (data == null) return ApiResult.NotFound("Invoice not found");
        return ApiResult.Success(data);
    }

    [HttpPost("orders/{orderId:guid}/invoice")]
    public async Task<IActionResult> IssueInvoice(Guid orderId, [FromBody] IssueInvoiceDto dto, CancellationToken ct)
    {
        var userId = User.GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        try
        {
            var data = await _service.IssueInvoiceAsync(orderId, dto, userId, ct);
            return ApiResult.Success(data);
        }
        catch (Exception ex)
        {
            return ApiResult.BadRequest(ex.Message);
        }
    }

    [HttpPost("invoices/{id:guid}/cancel")]
    public async Task<IActionResult> CancelInvoice(Guid id, [FromBody] CancelInvoiceDto dto, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.Reason))
            return ApiResult.BadRequest("Cancel reason is required");

        try
        {
            var data = await _service.CancelInvoiceAsync(id, dto, ct);
            return ApiResult.Success(data);
        }
        catch (Exception ex)
        {
            return ApiResult.BadRequest(ex.Message);
        }
    }

    [HttpGet("settings")]
    public async Task<IActionResult> GetSettings(CancellationToken ct)
    {
        var data = await _service.GetSettingsAsync(ct);
        if (data == null) return ApiResult.Success(new { });
        return ApiResult.Success(data);
    }

    [HttpPut("settings")]
    public async Task<IActionResult> UpdateSettings([FromBody] UpdateEInvoiceSettingDto dto, CancellationToken ct)
    {
        var data = await _service.UpdateSettingsAsync(dto, ct);
        return ApiResult.Success(data);
    }
}
