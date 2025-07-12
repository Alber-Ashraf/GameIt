using System.Linq.Expressions;
using GameIt.Domain;

namespace GameIt.Application.Interfaces.Persistence;

public interface IGameRepository : IGenericRepository<Game>
{
    Task<Game> GetByIdWithDetailsAsync(Guid id, CancellationToken token = default);
    Task<List<Game>> GetAllWithCategoryAsync(CancellationToken token = default);
    Task<List<Game>> GetFeaturedGamesAsync(int count = 5, CancellationToken token = default);
    Task<List<Game>> GetGamesByCategoryAsync(Guid categoryId, int limit = 10, CancellationToken token = default);
    Task<List<Game>> GetSimilarGamesAsync(Guid gameId, int limit = 5, CancellationToken token = default);
    Task<bool> IsGameUniqueForCreate(string name, CancellationToken token = default);
    Task<bool> IsGameNameUniqueForUpdate(Guid id, string name, CancellationToken token = default);
}
