using ChipmeoApis.Usecase.Interfaces;
using ChipmeoApis.Web.Authorization;
using ChipmeoApis.Usecase.DTOs.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChipmeoApis.Web.Controllers;

[ApiController]
[Route("admin/roles")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly IRoleService _service;

    public RolesController(IRoleService service)
    {
        _service = service;
    }

    [HttpGet]
    [RequirePermission("role.view")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var roles = await _service.GetAllAsync(cancellationToken);
        return Ok(roles);
    }

    [HttpGet("{id:int}")]
    [RequirePermission("role.view")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var role = await _service.GetByIdAsync(id, cancellationToken);
        if (role == null) return NotFound();
        return Ok(role);
    }

    [HttpPost]
    [RequirePermission("role.create")]
    public async Task<IActionResult> Create([FromBody] CreateRoleDto dto, CancellationToken cancellationToken)
    {
        var created = await _service.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [RequirePermission("role.update")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateRoleDto dto, CancellationToken cancellationToken)
    {
        var ok = await _service.UpdateAsync(id, dto, cancellationToken);
        if (!ok) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [RequirePermission("role.delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            var ok = await _service.DeleteAsync(id, cancellationToken);
            if (!ok) return NotFound();
            return NoContent();
        }
        catch (Exception)
        {
            return BadRequest(new { error = "Không thể xóa vai trò này vì đang có nhân viên nắm giữ." });
        }
    }

    [HttpPost("{id:int}/permissions")]
    [RequirePermission("role.update")]
    public async Task<IActionResult> AssignPermissions(int id, [FromBody] AssignPermissionsDto dto, CancellationToken cancellationToken)
    {
        var ok = await _service.AssignPermissionsAsync(id, dto, cancellationToken);
        if (!ok) return NotFound();
        return NoContent();
    }
}




