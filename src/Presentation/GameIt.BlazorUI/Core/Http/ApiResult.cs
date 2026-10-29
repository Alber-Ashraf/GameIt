namespace GameIt.BlazorUI.Core.Http;

public sealed record ApiResult<T>
{
    public bool IsSuccess { get; init; }
    public T? Data { get; init; }
    public string? ErrorMessage { get; init; }
    public int StatusCode { get; init; }
    public Dictionary<string, string[]> ValidationErrors { get; init; } = new();

    public static ApiResult<T> Success(T data, int statusCode = 200) => new()
    {
        IsSuccess = true,
        Data = data,
        StatusCode = statusCode
    };

    public static ApiResult<T> Failure(string message, int statusCode = 400, Dictionary<string, string[]>? validationErrors = null) => new()
    {
        IsSuccess = false,
        ErrorMessage = message,
        StatusCode = statusCode,
        ValidationErrors = validationErrors ?? new()
    };
}

public sealed record ApiResult
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public int StatusCode { get; init; }
    public Dictionary<string, string[]> ValidationErrors { get; init; } = new();

    public static ApiResult Success(int statusCode = 200) => new()
    {
        IsSuccess = true,
        StatusCode = statusCode
    };

    public static ApiResult Failure(string message, int statusCode = 400, Dictionary<string, string[]>? validationErrors = null) => new()
    {
        IsSuccess = false,
        ErrorMessage = message,
        StatusCode = statusCode,
        ValidationErrors = validationErrors ?? new()
    };
}
