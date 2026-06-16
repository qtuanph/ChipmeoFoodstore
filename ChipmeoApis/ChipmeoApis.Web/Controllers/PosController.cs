using ChipmeoApis.Usecase.Interfaces;
using ChipmeoApis.Infrastructure.Data;
using ChipmeoApis.Usecase.DTOs.Customer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChipmeoApis.Web.Controllers;

[ApiController]
[Route("pos")]
public class PosController(
    ICategoryService categoryService,
    IMenuItemService menuItemService,
    IAddonService addonService,
    IDiscountService discountService,
    IComboService comboService,
    ISourceService sourceService,
    StoreDbContext context) : ControllerBase
{
    private readonly ICategoryService _categoryService = categoryService;
    private readonly IMenuItemService _menuItemService = menuItemService;
    private readonly IAddonService _addonService = addonService;
    private readonly IDiscountService _discountService = discountService;
    private readonly IComboService _comboService = comboService;
    private readonly ISourceService _sourceService = sourceService;
    private readonly StoreDbContext _context = context;

    [HttpGet("menu")]
    public async Task<IActionResult> GetMenuData(CancellationToken cancellationToken)
    {
        var categories = (await _categoryService.GetAllAsync(cancellationToken)).Where(x => x.IsActive == true);
        var menuItems = (await _menuItemService.GetAllAsync(cancellationToken)).Where(x => x.IsActive == true);
        var addons = (await _addonService.GetAllAsync(cancellationToken)).Where(x => x.IsActive == true);
        var discounts = (await _discountService.GetAllAsync(cancellationToken)).Where(x => x.IsActive == true);
        var combos = (await _comboService.GetAllAsync(cancellationToken)).Where(x => x.IsActive == true);

        return Ok(new
        {
            categories,
            menuItems,
            addons,
            discounts,
            combos
        });
    }


    [HttpGet("sources")]
    public async Task<IActionResult> GetSources(CancellationToken cancellationToken)
    {
        var sources = (await _sourceService.GetAllAsync(cancellationToken)).Where(x => x.IsActive == true).ToList();
        return Ok(sources);
    }
}




