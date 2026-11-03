using GameIt.BlazorUI.Core.Http;
using GameIt.Contracts.Purchases;
using System.Net.Http.Json;

namespace GameIt.BlazorUI.Features.Purchases.Services;

public interface IPurchasesApiClient
{
    Task<ApiResult<List<PurchaseItemDto>>> GetUserPurchasesAsync(CancellationToken ct = default);
    Task<ApiResult<PurchaseResultDto>> CreatePurchaseAsync(CreatePurchaseRequestDto request, CancellationToken ct = default);
    Task<ApiResult<ConfirmPaymentResultDto>> ConfirmPaymentSessionAsync(string sessionId, CancellationToken ct = default);
    Task<ApiResult<ConfirmPaymentResultDto>> VerifyPurchaseAsync(Guid purchaseId, CancellationToken ct = default);
}

public class PurchasesApiClient : IPurchasesApiClient
{
    private readonly HttpClient _http;

    public PurchasesApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<ApiResult<List<PurchaseItemDto>>> GetUserPurchasesAsync(CancellationToken ct = default)
    {
        var response = await _http.GetAsync(ApiEndpoints.Purchases, ct);
        return await ProblemDetailsParser.ToApiResultAsync<List<PurchaseItemDto>>(response);
    }

    public async Task<ApiResult<PurchaseResultDto>> CreatePurchaseAsync(CreatePurchaseRequestDto request, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync(ApiEndpoints.Purchases, request, ct);
        return await ProblemDetailsParser.ToApiResultAsync<PurchaseResultDto>(response);
    }

    public async Task<ApiResult<ConfirmPaymentResultDto>> ConfirmPaymentSessionAsync(string sessionId, CancellationToken ct = default)
    {
        var response = await _http.PostAsync(ApiEndpoints.ConfirmPaymentSession(sessionId), null, ct);
        return await ProblemDetailsParser.ToApiResultAsync<ConfirmPaymentResultDto>(response);
    }

    public async Task<ApiResult<ConfirmPaymentResultDto>> VerifyPurchaseAsync(Guid purchaseId, CancellationToken ct = default)
    {
        var response = await _http.PostAsync(ApiEndpoints.VerifyPurchase(purchaseId), null, ct);
        return await ProblemDetailsParser.ToApiResultAsync<ConfirmPaymentResultDto>(response);
    }
}
