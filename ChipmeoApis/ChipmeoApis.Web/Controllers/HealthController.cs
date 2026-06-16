using ChipmeoApis.Core.Utils;
using Microsoft.AspNetCore.Mvc;
using ChipmeoApis.Web.ApiResponse;

namespace ChipmeoApis.Web.Controllers;

[ApiController]
[Route("/api/health")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() { return ApiResult.Success(new { status = "healthy", timestamp = TimeUtils.GetVietnamTime() }); }
}




