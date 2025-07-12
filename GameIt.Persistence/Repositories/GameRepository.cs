using System.Linq.Expressions;
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
            .AsNoTracking()
            .AnyAsync(g => g.Id != id && g.Name == name);
    }

    public async Task<bool> IsGameUniqueForCreate(string name,
        CancellationToken token = default)
    {
        return !await _context.Games
            .AsNoTracking()
            .AnyAsync(g => g.Name == name);
    }
    public async Task<List<Game>> GetSimilarGamesAsync(Guid gameId,
    int limit = 5,
    CancellationToken token = default)
    {
        var baseGame = await _context.Games
            .FirstOrDefaultAsync(g => g.Id == gameId, token);

        var query = _context.Games
            .Where(g => g.CategoryId == baseGame.CategoryId && g.Id != gameId)
            .Select(g => new {
                Game = g,
                AvgRating = g.Reviews.Average(r => (decimal?)r.Rating)
            })
            .OrderByDescending(x => x.AvgRating)
            .Take(limit)
            .Select(x => x.Game);

        return await query
            .AsNoTracking()
            .ToListAsync(token);
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
    public async Task<List<Game>> GetFeaturedGamesAsync(int limit = 5,
    CancellationToken token = default)
    {
        var query = _context.Games
            .Where(g => g.IsFeatured)
            .OrderByDescending(g => g.ReleaseDate)
            .Take(limit);

        return await query
            .AsNoTracking()
            .ToListAsync(token);
    }
}