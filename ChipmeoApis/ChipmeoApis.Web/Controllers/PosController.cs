using ChipmeoApis.Usecase.Interfaces;
using ChipmeoApis.Usecase.DTOs.Customer;
using Microsoft.AspNetCore.Mvc;
using ChipmeoApis.Web.ApiResponse;

namespace ChipmeoApis.Web.Controllers;

[ApiController]
[Route("api/pos")]
public class PosController(
    ICategoryService categoryService,
    IMenuItemService menuItemService,
    IAddonService addonService,
    IDiscountService discountService,
    IComboService comboService,
    ISourceService sourceService) : ControllerBase
{
    [HttpGet("menu")]
    public async Task<IActionResult> GetMenuData(CancellationToken cancellationToken)
    {
        var categories = (await categoryService.GetAllAsync(cancellationToken)).Where(x => x.IsActive == true);
        var menuItems = (await menuItemService.GetAllAsync(cancellationToken)).Where(x => x.IsActive == true);
        var addons = (await addonService.GetAllAsync(cancellationToken)).Where(x => x.IsActive == true);
        var discounts = (await discountService.GetAllAsync(cancellationToken)).Where(x => x.IsActive == true);
        var combos = (await comboService.GetAllAsync(cancellationToken)).Where(x => x.IsActive == true);

        return ApiResult.Success(new { categories, menuItems, addons, discounts, combos });
    }

    [HttpGet("sources")]
    public async Task<IActionResult> GetSources(CancellationToken cancellationToken)
    {
        var sources = (await sourceService.GetAllAsync(cancellationToken)).Where(x => x.IsActive == true).ToList();
        return ApiResult.Success(sources);
    }
}
