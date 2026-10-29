using GameIt.BlazorUI.Core.Http;
using GameIt.Contracts.Games;
using System.Net.Http.Json;

namespace GameIt.BlazorUI.Features.Catalog.Services;

public interface IGamesApiClient
{
    Task<ApiResult<List<GameSummaryDto>>> GetAllGamesAsync(CancellationToken ct = default);
    Task<ApiResult<List<GameSummaryDto>>> GetFeaturedGamesAsync(int limit = 5, CancellationToken ct = default);
    Task<ApiResult<GameDetailDto>> GetGameByIdAsync(Guid id, CancellationToken ct = default);
    Task<ApiResult<List<GameSummaryDto>>> GetGamesByCategoryAsync(Guid categoryId, int limit = 20, CancellationToken ct = default);
    Task<ApiResult<List<GameSummaryDto>>> GetSimilarGamesAsync(Guid gameId, int limit = 4, CancellationToken ct = default);
    Task<ApiResult<Guid>> CreateGameAsync(UpsertGameRequestDto request, CancellationToken ct = default);
    Task<ApiResult> UpdateGameAsync(Guid id, UpsertGameRequestDto request, CancellationToken ct = default);
    Task<ApiResult> DeleteGameAsync(Guid id, CancellationToken ct = default);
}

public class GamesApiClient : IGamesApiClient
{
    private readonly HttpClient _http;

    public GamesApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<ApiResult<List<GameSummaryDto>>> GetAllGamesAsync(CancellationToken ct = default)
    {
        var response = await _http.GetAsync(ApiEndpoints.Games, ct);
        return await ProblemDetailsParser.ToApiResultAsync<List<GameSummaryDto>>(response);
    }

    public async Task<ApiResult<List<GameSummaryDto>>> GetFeaturedGamesAsync(int limit = 5, CancellationToken ct = default)
    {
        var response = await _http.GetAsync($"{ApiEndpoints.GamesFeatured}?limit={limit}", ct);
        return await ProblemDetailsParser.ToApiResultAsync<List<GameSummaryDto>>(response);
    }

    public async Task<ApiResult<GameDetailDto>> GetGameByIdAsync(Guid id, CancellationToken ct = default)
    {
        var response = await _http.GetAsync(ApiEndpoints.GameById(id), ct);
        return await ProblemDetailsParser.ToApiResultAsync<GameDetailDto>(response);
    }

    public async Task<ApiResult<List<GameSummaryDto>>> GetGamesByCategoryAsync(Guid categoryId, int limit = 20, CancellationToken ct = default)
    {
        var response = await _http.GetAsync(ApiEndpoints.GamesByCategory(categoryId, limit), ct);
        return await ProblemDetailsParser.ToApiResultAsync<List<GameSummaryDto>>(response);
    }

    public async Task<ApiResult<List<GameSummaryDto>>> GetSimilarGamesAsync(Guid gameId, int limit = 4, CancellationToken ct = default)
    {
        var response = await _http.GetAsync(ApiEndpoints.GamesSimilar(gameId, limit), ct);
        return await ProblemDetailsParser.ToApiResultAsync<List<GameSummaryDto>>(response);
    }

    public async Task<ApiResult<Guid>> CreateGameAsync(UpsertGameRequestDto request, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync(ApiEndpoints.Games, request, ct);
        return await ProblemDetailsParser.ToApiResultAsync<Guid>(response);
    }

    public async Task<ApiResult> UpdateGameAsync(Guid id, UpsertGameRequestDto request, CancellationToken ct = default)
    {
        request.Id = id;
        var response = await _http.PutAsJsonAsync(ApiEndpoints.GameById(id), request, ct);
        return await ProblemDetailsParser.ToApiResultAsync(response);
    }

    public async Task<ApiResult> DeleteGameAsync(Guid id, CancellationToken ct = default)
    {
        var response = await _http.DeleteAsync(ApiEndpoints.GameById(id), ct);
        return await ProblemDetailsParser.ToApiResultAsync(response);
    }
}
