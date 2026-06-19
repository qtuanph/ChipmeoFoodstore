using System.Net;
using System.Text.Json;
using FoodstoreApi.Web.ApiResponse;

namespace FoodstoreApi.Web.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Resource not found: {Message}", ex.Message);
            await WriteErrorResponse(context, HttpStatusCode.NotFound, "NOT_FOUND", ex.Message);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed: {Message}", ex.Message);
            await WriteErrorResponse(context, HttpStatusCode.BadRequest, "VALIDATION_ERROR", ex.Message, ex.Errors);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access: {Message}", ex.Message);
            await WriteErrorResponse(context, HttpStatusCode.Forbidden, "FORBIDDEN", ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation: {Message}", ex.Message);
            await WriteErrorResponse(context, HttpStatusCode.BadRequest, "INVALID_OPERATION", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await WriteErrorResponse(context, HttpStatusCode.InternalServerError, "INTERNAL_ERROR", "An unexpected error occurred.");
        }
    }

    private static async Task WriteErrorResponse(HttpContext context, HttpStatusCode statusCode, string code, string message, List<FieldError>? details = null)
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        var response = ApiResponse<object>.Failure(new ErrorDetail
        {
            Code = code,
            Message = message,
            Details = details,
            RequestId = context.TraceIdentifier
        }, new Meta { RequestId = context.TraceIdentifier });

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}

public class NotFoundException(string message) : Exception(message);

public class ValidationException(string message, List<FieldError>? errors = null) : Exception(message)
{
    public List<FieldError>? Errors { get; } = errors;
}
