using ChipmeoApis.Usecase.Interfaces;
using ChipmeoApis.Web.Authorization;
using ChipmeoApis.Usecase.DTOs.Source;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.SignalR;
using ChipmeoApis.Web.Hubs;

namespace ChipmeoApis.Web.Controllers;

[ApiController]
[Route("admin/sources")]
[Authorize]
public class SourcesController(ISourceService service, IHubContext<AppHub> hubContext) : ControllerBase
{
    private readonly ISourceService _service = service;
    private readonly IHubContext<AppHub> _hubContext = hubContext;

    [HttpGet]
    [RequirePermission("source.view")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var sources = await _service.GetAllAsync(cancellationToken);
        return Ok(sources);
    }



    [HttpGet("{id:int}")]
    [RequirePermission("source.view")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var source = await _service.GetByIdAsync(id, cancellationToken);
        if (source == null) return NotFound();
        return Ok(source);
    }

    [HttpPost]
    [RequirePermission("source.create")]
    public async Task<IActionResult> Create([FromBody] CreateSourceDto dto, CancellationToken cancellationToken)
    {
        var created = await _service.CreateAsync(dto, cancellationToken);
        await _hubContext.Clients.All.SendAsync("ReceiveSourceUpdate", cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [RequirePermission("source.update")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateSourceDto dto, CancellationToken cancellationToken)
    {
        var ok = await _service.UpdateAsync(id, dto, cancellationToken);
        if (!ok) return NotFound();
        await _hubContext.Clients.All.SendAsync("ReceiveSourceUpdate", cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [RequirePermission("source.delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            var ok = await _service.DeleteAsync(id, cancellationToken);
            if (!ok) return NotFound();
            await _hubContext.Clients.All.SendAsync("ReceiveSourceUpdate", cancellationToken);
            return NoContent();
        }
        catch (Exception)
        {
            return BadRequest(new { error = "Không thể xóa nguồn này vì đang có đơn hàng liên kết." });
        }
    }
}




