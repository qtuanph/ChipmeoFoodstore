using ChipmeoApis.Usecase.Interfaces;
using ChipmeoApis.Web.Authorization;
using ChipmeoApis.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChipmeoApis.Web.Controllers;

[ApiController]
[Route("admin/payment-settings")]
public class PaymentSettingsController : ControllerBase
{
    private readonly IPaymentSettingService _service;

    public PaymentSettingsController(IPaymentSettingService service)
    {
        _service = service;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var setting = await _service.GetAsync(cancellationToken);
        return Ok(setting ?? new PaymentSetting());
    }

    /// <summary>
    /// Get all payment settings (for admin UI)
    /// </summary>
    [HttpGet("all")]
    [RequirePermission("payment.view")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var settings = await _service.GetAllAsync(cancellationToken);
        return Ok(settings);
    }

    /// <summary>
    /// Get payment setting by ID
    /// </summary>
    [HttpGet("{id}")]
    [RequirePermission("payment.view")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var setting = await _service.GetByIdAsync(id, cancellationToken);
        if (setting == null)
        {
            return NotFound(new { message = $"Payment setting with ID {id} not found" });
        }
        return Ok(setting);
    }

    /// <summary>
    /// Create or update payment setting
    /// </summary>
    [HttpPost]
    [RequirePermission("payment.update")]
    public async Task<IActionResult> Save([FromBody] PaymentSetting setting, CancellationToken cancellationToken)
    {
        try
        {
            var saved = await _service.SaveAsync(setting, cancellationToken);
            return Ok(saved);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update specific payment setting
    /// </summary>
    [HttpPut("{id}")]
    [RequirePermission("payment.update")]
    public async Task<IActionResult> Update(int id, [FromBody] PaymentSetting setting, CancellationToken cancellationToken)
    {
        setting.Id = id;
        try
        {
            var saved = await _service.SaveAsync(setting, cancellationToken);
            return Ok(saved);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Set payment setting as default
    /// </summary>
    [HttpPut("{id}/set-default")]
    [RequirePermission("payment.update")]
    public async Task<IActionResult> SetDefault(int id, CancellationToken cancellationToken)
    {
        try
        {
            await _service.SetDefaultAsync(id, cancellationToken);
            return Ok(new { message = "Payment setting set as default successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Delete payment setting (cannot delete default)
    /// </summary>
    [HttpDelete("{id}")]
    [RequirePermission("payment.delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            await _service.DeleteAsync(id, cancellationToken);
            return Ok(new { message = "Payment setting deleted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}




