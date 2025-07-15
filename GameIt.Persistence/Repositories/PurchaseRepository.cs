using GameIt.Application.Interfaces.Persistence;
using GameIt.Domain;
using GameIt.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace GameIt.Persistence.Repositories;

public class PurchaseRepository : GenericRepository<Purchase>, IPurchaseRepository
{
    public PurchaseRepository(GameItDbContext context) : base(context) { }

    public async Task<List<Purchase>> GetUserPurchasesAsync(
    string userId,
    CancellationToken token = default)
    {
        return await _context.Purchases
            .Include(p => p.Game)
            .Include(p => p.User)
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.PurchaseDate)
            .AsNoTracking()
            .ToListAsync(token);
    }
}
