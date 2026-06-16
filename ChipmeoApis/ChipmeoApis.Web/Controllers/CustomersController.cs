using ChipmeoApis.Usecase.Interfaces;
using ChipmeoApis.Usecase.DTOs;
using ChipmeoApis.Usecase.DTOs.Customer;
using ChipmeoApis.Web.Authorization;
using ChipmeoApis.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChipmeoApis.Web.ApiResponse;

namespace ChipmeoApis.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    /// <summary>
    /// Đăng ký tài khoản khách hàng
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] CustomerRegisterDto dto)
    {
        try
        {
            var customer = await _customerService.RegisterAsync(dto);
            return ApiResult.Success(customer);
        }
        catch (Exception ex)
        {
            return ApiResult.BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Đăng nhập khách hàng
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] CustomerLoginDto dto)
    {
        try
        {
            var result = await _customerService.LoginAsync(dto);
            return ApiResult.Success(result);
        }
        catch (Exception ex)
        {
            return ApiResult.BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Lấy thông tin khách hàng đang đăng nhập
    /// </summary>
    [HttpGet("me")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetMe()
    {
        var userId = User.GetUserId();
        var customer = await _customerService.GetCustomerByIdAsync(userId);
        if (customer == null) return ApiResult.NotFound();
        return ApiResult.Success(customer);
    }

    [HttpPut("me")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> UpdateProfile([FromBody] CustomerUpdateDto dto)
    {
        var userId = User.GetUserId();
        var customer = await _customerService.UpdateProfileAsync(userId, dto);
        if (customer == null) return ApiResult.NotFound();
        return ApiResult.Success(customer);
    }

    /// <summary>
    /// Lookup customer by phone number (POS)
    /// </summary>
    [HttpGet("lookup/{phone}")]
    [RequirePermission("customer.view")] // Or allow anonymous/pos? User logic implies cashier does it.
    public async Task<IActionResult> LookupByPhone(string phone)
    {
        var customer = await _customerService.GetByPhoneAsync(phone);
        if (customer == null) return ApiResult.NotFound();
        return ApiResult.Success(customer);
    }

    [HttpGet]
    [RequirePermission("customer.view")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var customers = await _customerService.GetAllAsync(cancellationToken);
        return ApiResult.Success(customers);
    }

    [HttpGet("{id:int}")]
    [RequirePermission("customer.view")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var customer = await _customerService.GetByIdAsync(id, cancellationToken);
        if (customer == null) return ApiResult.NotFound();
        return ApiResult.Success(customer);
    }

    [HttpPost]
    [RequirePermission("customer.create")]
    public async Task<IActionResult> Create(CreateCustomerDto dto, CancellationToken cancellationToken)
    {
        var created = await _customerService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [RequirePermission("customer.update")]
    public async Task<IActionResult> Update(int id, UpdateCustomerAdminDto dto, CancellationToken cancellationToken)
    {
        var ok = await _customerService.UpdateAsync(id, dto, cancellationToken);
        if (!ok) return ApiResult.NotFound();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [RequirePermission("customer.delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var ok = await _customerService.DeleteAsync(id, cancellationToken);
        if (!ok) return ApiResult.NotFound();
        return NoContent();
    }
}




