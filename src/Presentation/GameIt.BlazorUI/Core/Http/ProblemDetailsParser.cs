using System.Net.Http.Json;
using System.Text.Json;

namespace GameIt.BlazorUI.Core.Http;

public static class ProblemDetailsParser
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static async Task<(string Message, Dictionary<string, string[]> Errors)> ParseAsync(HttpResponseMessage response)
    {
        try
        {
            var raw = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(raw))
            {
                return (GetDefaultMessage((int)response.StatusCode), new());
            }

            using var doc = JsonDocument.Parse(raw);
            var root = doc.RootElement;

            string? title = root.TryGetProperty("title", out var t) ? t.GetString() : null;
            string? detail = root.TryGetProperty("detail", out var d) ? d.GetString() : null;

            var errors = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
            if (root.TryGetProperty("errors", out var errsElement) && errsElement.ValueKind == JsonValueKind.Object)
            {
                foreach (var prop in errsElement.EnumerateObject())
                {
                    if (prop.Value.ValueKind == JsonValueKind.Array)
                    {
                        var messages = prop.Value.EnumerateArray()
                            .Select(e => e.GetString() ?? string.Empty)
                            .Where(s => !string.IsNullOrWhiteSpace(s))
                            .ToArray();
                        if (messages.Length > 0)
                        {
                            errors[prop.Name] = messages;
                        }
                    }
                }
            }

            if (errors.Count > 0)
            {
                var firstError = errors.Values.SelectMany(x => x).FirstOrDefault();
                return (firstError ?? title ?? "Validation failed.", errors);
            }

            var finalMessage = !string.IsNullOrWhiteSpace(title) && title != "BadRequestException"
                ? title
                : (!string.IsNullOrWhiteSpace(detail) ? detail : GetDefaultMessage((int)response.StatusCode));

            return (finalMessage, errors);
        }
        catch
        {
            return (GetDefaultMessage((int)response.StatusCode), new());
        }
    }

    public static async Task<ApiResult<T>> ToApiResultAsync<T>(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return ApiResult<T>.Success(default!, (int)response.StatusCode);
            }

            try
            {
                var data = await response.Content.ReadFromJsonAsync<T>(JsonOptions);
                return ApiResult<T>.Success(data!, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                return ApiResult<T>.Failure($"Failed to parse server response: {ex.Message}", (int)response.StatusCode);
            }
        }

        var (message, errors) = await ParseAsync(response);
        return ApiResult<T>.Failure(message, (int)response.StatusCode, errors);
    }

    public static async Task<ApiResult> ToApiResultAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return ApiResult.Success((int)response.StatusCode);
        }

        var (message, errors) = await ParseAsync(response);
        return ApiResult.Failure(message, (int)response.StatusCode, errors);
    }

    private static string GetDefaultMessage(int statusCode) => statusCode switch
    {
        400 => "The request could not be processed. Please check your input.",
        401 => "Your session has expired or you are not signed in.",
        403 => "You do not have permission to perform this action.",
        404 => "The requested item could not be found.",
        409 => "This item already exists or conflicts with current state.",
        429 => "Too many requests. Please wait a moment and try again.",
        _ => "An unexpected server error occurred. Please try again."
    };
}
