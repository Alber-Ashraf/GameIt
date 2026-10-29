using GameIt.BlazorUI.Core.Http;
using GameIt.Contracts.Reviews;
using System.Net.Http.Json;

namespace GameIt.BlazorUI.Features.Reviews.Services;

public interface IReviewsApiClient
{
    Task<ApiResult<List<ReviewItemDto>>> GetReviewsByGameAsync(Guid gameId, CancellationToken ct = default);
    Task<ApiResult> CreateReviewAsync(UpsertReviewRequestDto request, CancellationToken ct = default);
    Task<ApiResult> UpdateReviewAsync(Guid id, UpsertReviewRequestDto request, CancellationToken ct = default);
    Task<ApiResult> DeleteReviewAsync(Guid id, CancellationToken ct = default);
}

public class ReviewsApiClient : IReviewsApiClient
{
    private readonly HttpClient _http;

    public ReviewsApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<ApiResult<List<ReviewItemDto>>> GetReviewsByGameAsync(Guid gameId, CancellationToken ct = default)
    {
        var response = await _http.GetAsync(ApiEndpoints.ReviewsByGame(gameId), ct);
        return await ProblemDetailsParser.ToApiResultAsync<List<ReviewItemDto>>(response);
    }

    public async Task<ApiResult> CreateReviewAsync(UpsertReviewRequestDto request, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync(ApiEndpoints.Reviews, request, ct);
        return await ProblemDetailsParser.ToApiResultAsync(response);
    }

    public async Task<ApiResult> UpdateReviewAsync(Guid id, UpsertReviewRequestDto request, CancellationToken ct = default)
    {
        request.Id = id;
        var response = await _http.PutAsJsonAsync(ApiEndpoints.ReviewById(id), request, ct);
        return await ProblemDetailsParser.ToApiResultAsync(response);
    }

    public async Task<ApiResult> DeleteReviewAsync(Guid id, CancellationToken ct = default)
    {
        var response = await _http.DeleteAsync(ApiEndpoints.ReviewById(id), ct);
        return await ProblemDetailsParser.ToApiResultAsync(response);
    }
}
