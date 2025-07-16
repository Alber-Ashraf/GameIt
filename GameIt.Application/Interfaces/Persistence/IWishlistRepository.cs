using GameIt.Application.Features.Wishlist.Queries.GetUserWishlist;
using GameIt.Domain;

namespace GameIt.Application.Interfaces.Persistence;

public interface IWishlistRepository : IGenericRepository<Wishlist>
{
    Task<List<WishlistListDto>> GetUserWishlistWithDetailsAsync(
        string userId,
        CancellationToken token = default);

    Task<bool> IsGameInWishlistAsync(
        string userId,
        Guid gameId,
        CancellationToken token = default);

    Task RemoveFromWishlistAsync(
        string userId,
        Guid gameId,
        CancellationToken token = default);

    Task CleanWishlistAsync(
        string userId,
        CancellationToken token = default);

    Task<bool> AnyWishlistItemsAsync(
        string userId,
        CancellationToken token = default);
}