using FoodstoreApi.Core.Utils;
using Microsoft.AspNetCore.Mvc;
using FoodstoreApi.Web.ApiResponse;

namespace FoodstoreApi.Web.Controllers;

[ApiController]
[Route("/api/health")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() { return ApiResult.Success(new { status = "healthy", timestamp = TimeUtils.GetVietnamTime() }); }
}




