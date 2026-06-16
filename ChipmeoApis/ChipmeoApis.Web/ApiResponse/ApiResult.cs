using Microsoft.AspNetCore.Mvc;

namespace ChipmeoApis.Web.ApiResponse;

public static class ApiResult
{
    public static IActionResult Success<T>(T data, Meta? meta = null) =>
        new OkObjectResult(ApiResponse<T>.Success(data, meta));

    public static IActionResult Created<T>(string? actionName, object? routeValues, T data) =>
        new CreatedAtActionResult(actionName, null, routeValues, ApiResponse<T>.Success(data));

    public static IActionResult Paged<T>(List<T> data, int page, int pageSize, int totalCount)
    {
        var meta = new Meta
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
        return new OkObjectResult(new PagedResponse<T>
        {
            Data = data,
            Error = null,
            Meta = meta
        });
    }

    public static IActionResult NoContent() => new NoContentResult();

    public static IActionResult NotFound(string message = "Resource not found") =>
        new NotFoundObjectResult(ApiResponse<object>.Failure(new ErrorDetail
        {
            Code = "NOT_FOUND",
            Message = message
        }));

    public static IActionResult BadRequest(string message, List<FieldError>? errors = null) =>
        new BadRequestObjectResult(ApiResponse<object>.Failure(new ErrorDetail
        {
            Code = "VALIDATION_ERROR",
            Message = message,
            Details = errors
        }));
}
