using GameIt.Application.Interfaces.Persistence;
using GameIt.Domain;
using GameIt.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace GameIt.Persistence.Repositories;

public class GameRepository : GenericRepository<Game>, IGameRepository
{
    public GameRepository(GameItDbContext context) : base(context) { }

    public async Task<List<Game>> GetAllWithCategoryAsync(CancellationToken token = default)
    {
        return await _context.Games
            .Include(g => g.Category)
            .Include(g => g.Reviews)
            .Include(g => g.Discount)
            .AsNoTracking()
            .ToListAsync(token);
    }

    public async Task<Game?> GetByIdWithDetailsAsync(Guid id,
        CancellationToken token = default)
    {
        return await _context.Games
            .Include(g => g.Category)
            .Include(g => g.Reviews)
            .Include(g => g.Discount)
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == id, token);
    }

    public async Task<bool> IsGameNameUniqueForUpdate(Guid id,
        string name,
        CancellationToken token = default)
    {
        return !await _context.Games
            .AsNoTracking()
            .AnyAsync(g => g.Id != id && g.Name == name, token);
    }

    public async Task<bool> IsGameUniqueForCreate(string name,
        CancellationToken token = default)
    {
        return !await _context.Games
            .AsNoTracking()
            .AnyAsync(g => g.Name == name, token);
    }

    public async Task<List<Game>> GetSimilarGamesAsync(Guid gameId,
        int limit = 5,
        CancellationToken token = default)
    {
        var baseGame = await _context.Games
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == gameId, token);

        if (baseGame == null)
            return new List<Game>();

        return await _context.Games
            .Where(g => g.CategoryId == baseGame.CategoryId && g.Id != gameId)
            .Include(g => g.Category)
            .Include(g => g.Reviews)
            .Include(g => g.Discount)
            .OrderByDescending(g => g.Reviews.Any() ? g.Reviews.Average(r => (double?)r.Rating) : 0.0)
            .Take(limit)
            .AsNoTracking()
            .ToListAsync(token);
    }

    public async Task<List<Game>> GetGamesByCategoryAsync(Guid categoryId,
        int limit = 10,
        CancellationToken cancellationToken = default)
    {
        return await _context.Games
            .Where(g => g.CategoryId == categoryId)
            .Include(g => g.Category)
            .Include(g => g.Reviews)
            .Include(g => g.Discount)
            .OrderByDescending(g => g.ReleaseDate)
            .Take(limit)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Game>> GetFeaturedGamesAsync(int limit = 5,
        CancellationToken token = default)
    {
        return await _context.Games
            .Where(g => g.IsFeatured)
            .Include(g => g.Category)
            .Include(g => g.Reviews)
            .Include(g => g.Discount)
            .OrderByDescending(g => g.ReleaseDate)
            .Take(limit)
            .AsNoTracking()
            .ToListAsync(token);
    }
}
