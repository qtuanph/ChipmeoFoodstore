using ChipmeoApis.Usecase.Interfaces;
using ChipmeoApis.Web.Authorization;
using ChipmeoApis.Usecase.DTOs.Category;
using Microsoft.AspNetCore.Mvc;

namespace ChipmeoApis.Web.Controllers;

[ApiController]
[Route("admin/categories")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _service;

    public CategoriesController(ICategoryService service)
    {
        _service = service;
    }

    [HttpGet]
    [RequirePermission("category.view")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var items = await _service.GetAllAsync(cancellationToken);
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    [RequirePermission("category.view")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var item = await _service.GetByIdAsync(id, cancellationToken);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    [RequirePermission("category.create")]
    public async Task<IActionResult> Create(CreateCategoryDto dto, CancellationToken cancellationToken)
    {
        var created = await _service.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [RequirePermission("category.update")]
    public async Task<IActionResult> Update(int id, CreateCategoryDto dto, CancellationToken cancellationToken)
    {
        var ok = await _service.UpdateAsync(id, dto, cancellationToken);
        if (!ok) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [RequirePermission("category.delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var ok = await _service.DeleteAsync(id, cancellationToken);
        if (!ok) return NotFound();
        return NoContent();
    }
}




