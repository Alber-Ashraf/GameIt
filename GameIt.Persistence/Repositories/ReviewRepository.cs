using GameIt.Application.Interfaces.Persistence;
using GameIt.Domain;
using GameIt.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace GameIt.Persistence.Repositories;

public class ReviewRepository : GenericRepository<Review>, IReviewRepository
{
    public ReviewRepository(GameItDbContext context) : base(context) {}
    // Get all reviews for a specific game, ordered by creation date descending
    public async Task<List<Review>> GetReviewsByGameAsync(
        Guid gameId,
        CancellationToken token = default)
    {
        return await _context.Reviews
            .Where(r => r.GameId == gameId)
            .Include(r => r.User) 
            .OrderByDescending(r => r.CreatedAt) 
            .AsNoTracking()
            .ToListAsync(token);
    }
}
