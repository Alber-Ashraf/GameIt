using GameIt.Domain;

namespace GameIt.Application.Interfaces.Persistence;

public interface ILibraryRepository : IGenericRepository<Library>
{
    Task<List<Library>> GetLibraryForUserAsync(string userId, CancellationToken token = default);
}
