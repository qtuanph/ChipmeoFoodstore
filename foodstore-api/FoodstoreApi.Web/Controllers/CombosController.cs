using FoodstoreApi.Usecase.Interfaces;
using FoodstoreApi.Web.Authorization;
using FoodstoreApi.Usecase.DTOs.Combo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using FoodstoreApi.Web.Hubs;
using FoodstoreApi.Web.ApiResponse;

namespace FoodstoreApi.Web.Controllers;

[ApiController]
[Route("api/admin/combos")]
[Authorize]
public class CombosController : ControllerBase
{
    private readonly IComboService _service;
    private readonly IHubContext<AppHub> _hubContext;

    public CombosController(IComboService service, IHubContext<AppHub> hubContext)
    {
        _service = service;
        _hubContext = hubContext;
    }

    [HttpGet]
    [RequirePermission("combo.view")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var items = await _service.GetAllAsync(cancellationToken);
        return ApiResult.Success(items);
    }

    [HttpGet("{id:guid}")]
    [RequirePermission("combo.view")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var item = await _service.GetByIdAsync(id, cancellationToken);
        if (item == null) return ApiResult.NotFound();
        return ApiResult.Success(item);
    }

    [HttpPost]
    [RequirePermission("combo.create")]
    public async Task<IActionResult> Create(CreateComboDto dto, CancellationToken cancellationToken)
    {
        var created = await _service.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [RequirePermission("combo.update")]
    public async Task<IActionResult> Update(Guid id, CreateComboDto dto, CancellationToken cancellationToken)
    {
        var ok = await _service.UpdateAsync(id, dto, cancellationToken);
        if (!ok) return ApiResult.NotFound();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [RequirePermission("combo.delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var ok = await _service.DeleteAsync(id, cancellationToken);
        if (!ok) return ApiResult.NotFound();
        return NoContent();
    }

    // POS endpoints for read-only combos
    [HttpGet("/api/pos/combos")]
    [AllowAnonymous]
    public async Task<IActionResult> PosGetAll(CancellationToken cancellationToken)
    {
        var combos = await _service.GetAllAsync(cancellationToken);
        var activeCombos = combos.Where(c => c.IsActive);
        return ApiResult.Success(activeCombos);
    }

    [HttpGet("/api/pos/combos/{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> PosGetById(Guid id, CancellationToken cancellationToken)
    {
        var combo = await _service.GetByIdAsync(id, cancellationToken);
        if (combo == null || !combo.IsActive) return ApiResult.NotFound();
        return ApiResult.Success(combo);
    }
}
