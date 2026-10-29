using GameIt.BlazorUI.Core.Http;
using GameIt.Contracts.Wishlists;
using System.Net.Http.Json;

namespace GameIt.BlazorUI.Features.Wishlist.Services;

public interface IWishlistApiClient
{
    Task<ApiResult<List<WishlistItemDto>>> GetUserWishlistAsync(CancellationToken ct = default);
    Task<ApiResult> AddToWishlistAsync(Guid gameId, CancellationToken ct = default);
    Task<ApiResult> RemoveFromWishlistAsync(Guid gameId, CancellationToken ct = default);
    Task<ApiResult> ClearWishlistAsync(CancellationToken ct = default);
}

public class WishlistApiClient : IWishlistApiClient
{
    private readonly HttpClient _http;

    public WishlistApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<ApiResult<List<WishlistItemDto>>> GetUserWishlistAsync(CancellationToken ct = default)
    {
        var response = await _http.GetAsync(ApiEndpoints.Wishlist, ct);
        return await ProblemDetailsParser.ToApiResultAsync<List<WishlistItemDto>>(response);
    }

    public async Task<ApiResult> AddToWishlistAsync(Guid gameId, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync(ApiEndpoints.WishlistItems, new AddToWishlistRequestDto { GameId = gameId }, ct);
        return await ProblemDetailsParser.ToApiResultAsync(response);
    }

    public async Task<ApiResult> RemoveFromWishlistAsync(Guid gameId, CancellationToken ct = default)
    {
        var response = await _http.DeleteAsync(ApiEndpoints.WishlistItemByGame(gameId), ct);
        return await ProblemDetailsParser.ToApiResultAsync(response);
    }

    public async Task<ApiResult> ClearWishlistAsync(CancellationToken ct = default)
    {
        var response = await _http.DeleteAsync(ApiEndpoints.Wishlist, ct);
        return await ProblemDetailsParser.ToApiResultAsync(response);
    }
}
