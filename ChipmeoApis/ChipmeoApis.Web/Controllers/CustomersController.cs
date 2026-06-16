using ChipmeoApis.Usecase.Interfaces;
using ChipmeoApis.Web.Authorization;
using ChipmeoApis.Usecase.DTOs;
using ChipmeoApis.Usecase.DTOs.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
            return Ok(customer);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
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
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy thông tin khách hàng đang đăng nhập
    /// </summary>
    [HttpGet("me")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetMe()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var customer = await _customerService.GetCustomerByIdAsync(userId);
        if (customer == null) return NotFound();
        return Ok(customer);
    }

    /// <summary>
    /// Cập nhật thông tin khách hàng
    /// </summary>
    [HttpPut("me")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> UpdateProfile([FromBody] CustomerUpdateDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var customer = await _customerService.UpdateProfileAsync(userId, dto);
        if (customer == null) return NotFound();
        return Ok(customer);
    }

    /// <summary>
    /// Lookup customer by phone number (POS)
    /// </summary>
    [HttpGet("lookup/{phone}")]
    [RequirePermission("customer.view")] // Or allow anonymous/pos? User logic implies cashier does it.
    public async Task<IActionResult> LookupByPhone(string phone)
    {
        var customer = await _customerService.GetByPhoneAsync(phone);
        if (customer == null) return NotFound();
        return Ok(customer);
    }

    [HttpGet]
    [RequirePermission("customer.view")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var customers = await _customerService.GetAllAsync(cancellationToken);
        return Ok(customers);
    }

    [HttpGet("{id:int}")]
    [RequirePermission("customer.view")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var customer = await _customerService.GetByIdAsync(id, cancellationToken);
        if (customer == null) return NotFound();
        return Ok(customer);
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
        if (!ok) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [RequirePermission("customer.delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var ok = await _customerService.DeleteAsync(id, cancellationToken);
        if (!ok) return NotFound();
        return NoContent();
    }
}




