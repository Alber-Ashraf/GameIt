using GameIt.Application.Interfaces.Persistence;
using GameIt.Domain;
using GameIt.Persistence.DatabaseContext;

namespace GameIt.Persistence.Repositories;

public class WishlistRepository : GenericRepository<Wishlist>, IWishlistRepository
{
    public WishlistRepository(GameItDbContext context) : base(context) { }
}
