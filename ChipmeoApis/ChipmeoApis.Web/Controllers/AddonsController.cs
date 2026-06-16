using ChipmeoApis.Usecase.Interfaces;
using ChipmeoApis.Web.Authorization;
using ChipmeoApis.Usecase.DTOs.Addon;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChipmeoApis.Web.ApiResponse;

namespace ChipmeoApis.Web.Controllers;

[ApiController]
[Route("api/admin/addons")]
[Authorize]
public class AddonsController(IAddonService service) : ControllerBase
{
    private readonly IAddonService _service = service;

    [HttpGet]
    [RequirePermission("addon.view")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var addons = await _service.GetAllAsync(cancellationToken);
        return ApiResult.Success(addons);
    }

    [HttpGet("/api/pos/addons")]
    [AllowAnonymous]
    public async Task<IActionResult> PosGetAll(CancellationToken cancellationToken)
    {
        var addons = await _service.GetAllAsync(cancellationToken);
        return ApiResult.Success(addons);
    }

    [HttpGet("{id:int}")]
    [RequirePermission("addon.view")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var addon = await _service.GetByIdAsync(id, cancellationToken);
        if (addon == null) return ApiResult.NotFound();
        return ApiResult.Success(addon);
    }

    [HttpPost]
    [RequirePermission("addon.create")]
    public async Task<IActionResult> Create([FromBody] CreateAddonDto dto, CancellationToken cancellationToken)
    {
        var created = await _service.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [RequirePermission("addon.update")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateAddonDto dto, CancellationToken cancellationToken)
    {
        var ok = await _service.UpdateAsync(id, dto, cancellationToken);
        if (!ok) return ApiResult.NotFound();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [RequirePermission("addon.delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            var ok = await _service.DeleteAsync(id, cancellationToken);
            if (!ok) return ApiResult.NotFound();
            return NoContent();
        }
        catch (Exception)
        {
            return ApiResult.BadRequest("Không thể xóa topping này vì đang được sử dụng trong đơn hàng.");
        }
    }
}




