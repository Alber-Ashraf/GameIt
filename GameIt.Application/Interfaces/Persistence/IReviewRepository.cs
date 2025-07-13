using GameIt.Domain;

namespace GameIt.Application.Interfaces.Persistence;

public interface IReviewRepository : IGenericRepository<Review>
{
    Task<List<Review>> GetReviewsByGameAsync(Guid gameId, CancellationToken token = default);
}
