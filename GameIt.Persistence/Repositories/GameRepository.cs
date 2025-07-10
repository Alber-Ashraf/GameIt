using GameIt.Application.Interfaces.Persistence;
using GameIt.Domain;
using GameIt.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace GameIt.Persistence.Repositories;

public class GameRepository : GenericRepository<Game>, IGameRepository
{
    public GameRepository(GameItDbContext context) : base(context) { }

    public async Task<List<Game>> GetAllWithCategoryAsync()
    {
        return await _context.Games
            .Include(g => g.Category)
            .Include(g => g.Publisher)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Game?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _context.Games
            .Include(g => g.Category)
            .Include(g => g.Publisher)
            .Include(g => g.Reviews)
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<bool> IsGameNameUniqueForUpdate(Guid id, string name)
    {
        return !await _context.Games
            .AnyAsync(g => g.Id != id && g.Name == name);
    }

    public async Task<bool> IsGameUniqueForCreate(string name)
    {
        return !await _context.Games
            .AnyAsync(g => g.Name == name);
    }

    public async Task<List<Game>> GetFeaturedGamesAsync()
    {
        return await _context.Games
            .Where(g => g.IsFeatured)
            .OrderByDescending(g => g.ReleaseDate)
            .Take(10)
            .AsNoTracking()
            .ToListAsync();
    }
}