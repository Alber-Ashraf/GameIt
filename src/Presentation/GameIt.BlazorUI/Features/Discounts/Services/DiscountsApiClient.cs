using GameIt.BlazorUI.Core.Http;
using GameIt.Contracts.Discounts;
using System.Net.Http.Json;

namespace GameIt.BlazorUI.Features.Discounts.Services;

public interface IDiscountsApiClient
{
    Task<ApiResult<List<ActiveDiscountItemDto>>> GetActiveDiscountsAsync(CancellationToken ct = default);
    Task<ApiResult<Guid>> CreateDiscountAsync(UpsertDiscountRequestDto request, CancellationToken ct = default);
    Task<ApiResult> UpdateDiscountAsync(Guid id, UpsertDiscountRequestDto request, CancellationToken ct = default);
    Task<ApiResult> DeleteDiscountAsync(Guid id, CancellationToken ct = default);
}

public class DiscountsApiClient : IDiscountsApiClient
{
    private readonly HttpClient _http;

    public DiscountsApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<ApiResult<List<ActiveDiscountItemDto>>> GetActiveDiscountsAsync(CancellationToken ct = default)
    {
        var response = await _http.GetAsync(ApiEndpoints.DiscountsActive, ct);
        return await ProblemDetailsParser.ToApiResultAsync<List<ActiveDiscountItemDto>>(response);
    }

    public async Task<ApiResult<Guid>> CreateDiscountAsync(UpsertDiscountRequestDto request, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync(ApiEndpoints.Discounts, request, ct);
        return await ProblemDetailsParser.ToApiResultAsync<Guid>(response);
    }

    public async Task<ApiResult> UpdateDiscountAsync(Guid id, UpsertDiscountRequestDto request, CancellationToken ct = default)
    {
        request.Id = id;
        var response = await _http.PutAsJsonAsync(ApiEndpoints.DiscountById(id), request, ct);
        return await ProblemDetailsParser.ToApiResultAsync(response);
    }

    public async Task<ApiResult> DeleteDiscountAsync(Guid id, CancellationToken ct = default)
    {
        var response = await _http.DeleteAsync(ApiEndpoints.DiscountById(id), ct);
        return await ProblemDetailsParser.ToApiResultAsync(response);
    }
}
