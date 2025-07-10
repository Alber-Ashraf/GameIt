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
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Game?> GetByIdWithDetailsAsync(Guid id,
        CancellationToken token = default)
    {
        return await _context.Games
            .Include(g => g.Category)
            .Include(g => g.Publisher)
            .Include(g => g.Reviews)
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<bool> IsGameNameUniqueForUpdate(Guid id,
        string name,
        CancellationToken token = default)
    {
        return !await _context.Games
            .AnyAsync(g => g.Id != id && g.Name == name);
    }

    public async Task<bool> IsGameUniqueForCreate(string name,
        CancellationToken token = default)
    {
        return !await _context.Games
            .AnyAsync(g => g.Name == name);
    }

    public async Task<List<Game>> GetSimilarGamesAsync(Guid gameId,
        int limit = 5,
        CancellationToken token = default)
    {
        var game = await _context.Games
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == gameId);

        if (game == null) return new List<Game>();

        return await _context.Games
            .Where(g => g.CategoryId == game.CategoryId && g.Id != gameId)
            .OrderByDescending(g => g.Reviews.Average(r => r.Rating))
            .Take(limit)
            .AsNoTracking()
            .ToListAsync();
    }
    public async Task<List<Game>> GetGamesByCategoryAsync(Guid categoryId,
        int limit = 10,  
        CancellationToken cancellationToken = default)
    {
        var query = _context.Games
            .Where(g => g.CategoryId == categoryId)
            .OrderByDescending(g => g.ReleaseDate)
            .Take(limit);

        return await query
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<List<Game>> GetFeaturedGamesAsync(int count = 5,
        CancellationToken token = default)
    {
        return await _context.Games
            .Where(g => g.IsFeatured)
            .OrderByDescending(g => g.ReleaseDate)
            .Take(count)
            .AsNoTracking()
            .ToListAsync();
    }
}