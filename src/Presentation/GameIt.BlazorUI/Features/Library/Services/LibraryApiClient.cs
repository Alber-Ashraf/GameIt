using GameIt.BlazorUI.Core.Http;
using GameIt.Contracts.Libraries;

namespace GameIt.BlazorUI.Features.Library.Services;

public interface ILibraryApiClient
{
    Task<ApiResult<List<LibraryItemDto>>> GetUserLibraryAsync(CancellationToken ct = default);
}

public class LibraryApiClient : ILibraryApiClient
{
    private readonly HttpClient _http;

    public LibraryApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<ApiResult<List<LibraryItemDto>>> GetUserLibraryAsync(CancellationToken ct = default)
    {
        var response = await _http.GetAsync(ApiEndpoints.Library, ct);
        return await ProblemDetailsParser.ToApiResultAsync<List<LibraryItemDto>>(response);
    }
}
