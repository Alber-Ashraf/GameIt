using GameIt.Application.Interfaces.Persistence;
using GameIt.Domain;
using GameIt.Persistence.DatabaseContext;

namespace GameIt.Persistence.Repositories;

public class LibraryRepository : GenericRepository<Library>, ILibraryRepository
{
    public LibraryRepository(GameItDbContext context) : base(context) {}

    public Task<List<Library>> GetLibraryForUserAsync(string userId, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}
