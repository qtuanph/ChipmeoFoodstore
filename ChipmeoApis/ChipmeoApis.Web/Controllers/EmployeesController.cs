using ChipmeoApis.Usecase.Interfaces;
using ChipmeoApis.Web.Authorization;
using ChipmeoApis.Usecase.DTOs.Employee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChipmeoApis.Web.Controllers;

[ApiController]
[Route("admin/employees")]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _service;

    public EmployeesController(IEmployeeService service)
    {
        _service = service;
    }

    [HttpGet]
    [RequirePermission("employee.view")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var employees = await _service.GetAllAsync(cancellationToken);
        return Ok(employees);
    }

    [HttpGet("{id:int}")]
    [RequirePermission("employee.view")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var employee = await _service.GetByIdAsync(id, cancellationToken);
        if (employee == null) return NotFound();
        return Ok(employee);
    }

    [HttpPost]
    [RequirePermission("employee.create")]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var created = await _service.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    [RequirePermission("employee.update")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEmployeeDto dto, CancellationToken cancellationToken)
    {
        var ok = await _service.UpdateAsync(id, dto, cancellationToken);
        if (!ok) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [RequirePermission("employee.delete")]
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
            return BadRequest(new { error = "Không thể xóa nhân viên này vì đang liên kết với đơn hàng hoặc dữ liệu khác." });
        }
    }
}




