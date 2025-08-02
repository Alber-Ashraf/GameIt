using GameIt.Application.Interfaces.Persistence;
using GameIt.Domain;
using GameIt.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace GameIt.Persistence.Repositories;

public class LibraryRepository : GenericRepository<Library>, ILibraryRepository
{
    public LibraryRepository(GameItDbContext context) : base(context) {}

    public async Task<List<Library>> GetLibraryForUserAsync(string userId, CancellationToken token = default)
    {
        return await _context.Libraries
            .Include(g => g.Game)
            .ThenInclude(g => g.Category)
            .Where(l => l.UserId == userId)
            .AsNoTracking()
            .ToListAsync(token);
    }
}
