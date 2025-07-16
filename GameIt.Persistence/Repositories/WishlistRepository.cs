using GameIt.Application.Features.Wishlist.Queries.GetUserWishlist;
using GameIt.Application.Interfaces.Persistence;
using GameIt.Domain;
using GameIt.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace GameIt.Persistence.Repositories;

public class WishlistRepository : GenericRepository<Wishlist>, IWishlistRepository
{
    public WishlistRepository(GameItDbContext context) : base(context) { }

    public async Task<List<WishlistListDto>> GetUserWishlistWithDetailsAsync(
        string userId,
        CancellationToken token = default)
    {
        return await _context.Wishlists
         .Where(w => w.UserId == userId)
         .Include(w => w.Game)
         .Select(w => new WishlistListDto
         {
             GameId = w.GameId,
             Price = w.Game.Price,
             ImageUrl = w.Game.ImageUrl,
             AddedDate = w.CreatedAt,
         })
         .AsNoTracking()
         .ToListAsync();
    }

    public async Task<bool> IsGameInWishlistAsync(
        string userId,
        Guid gameId,
        CancellationToken token = default)
    {
        return await _context.Wishlists
            .AnyAsync(w => w.UserId == userId && w.GameId == gameId, token);
    }

    public async Task RemoveFromWishlistAsync(
        string userId,
        Guid gameId,
        CancellationToken token = default)
    {
        var item = await _context.Wishlists
            .FirstOrDefaultAsync(w => w.UserId == userId && w.GameId == gameId, token);

        if (item != null)
        {
            _context.Wishlists.Remove(item);
        }
    }

    public async Task CleanWishlistAsync(
        string userId,
        CancellationToken token = default)
    {
        var items = await _context.Wishlists
            .Where(w => w.UserId == userId)
            .ToListAsync(token);

        _context.Wishlists.RemoveRange(items);
    }
}
