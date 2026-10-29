using GameIt.Application.Interfaces.Persistence;
using GameIt.Domain;
using GameIt.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace GameIt.Persistence.Repositories;

public class DiscountRepository : GenericRepository<Discount>, IDiscountRepository
{
    public DiscountRepository(GameItDbContext context) : base(context) { }

    public async Task<List<Discount>> GetActiveDiscountsAsync(CancellationToken token = default)
    {
        var now = DateTime.UtcNow;

        return await _context.Discounts
            .Include(d => d.Game) 
            .Where(d => d.StartDate <= now && d.EndDate >= now) 
            .OrderByDescending(d => d.Percentage)
            .AsNoTracking() 
            .ToListAsync(token);
    }
}
