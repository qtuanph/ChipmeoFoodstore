namespace ChipmeoApis.Web.ApiResponse;

public class ApiResponse<T>
{
    public T? Data { get; set; }
    public ErrorDetail? Error { get; set; }
    public Meta? Meta { get; set; }

    protected internal ApiResponse() { }

    public static ApiResponse<T> Success(T data, Meta? meta = null) => new()
    {
        Data = data,
        Error = null,
        Meta = meta
    };

    public static ApiResponse<T> Failure(ErrorDetail error, Meta? meta = null) => new()
    {
        Data = default,
        Error = error,
        Meta = meta
    };
}

public class PagedResponse<T> : ApiResponse<List<T>>
{
    public new Meta Meta { get; set; } = new();

    public static PagedResponse<T> Success(List<T> data, int page, int pageSize, int totalCount) => new()
    {
        Data = data,
        Error = null,
        Meta = new Meta
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        }
    };
}

public class Meta
{
    public int? Page { get; set; }
    public int? PageSize { get; set; }
    public int? TotalCount { get; set; }
    public int? TotalPages { get; set; }
    public string? RequestId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class ErrorDetail
{
    public string Code { get; set; } = "INTERNAL_ERROR";
    public string Message { get; set; } = "An unexpected error occurred.";
    public List<FieldError>? Details { get; set; }
    public string? RequestId { get; set; }
}

public class FieldError
{
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
