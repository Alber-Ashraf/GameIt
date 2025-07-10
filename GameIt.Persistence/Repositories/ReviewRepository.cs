using GameIt.Application.Interfaces.Persistence;
using GameIt.Domain;
using GameIt.Persistence.DatabaseContext;

namespace GameIt.Persistence.Repositories;

public class ReviewRepository : GenericRepository<Review>, IReviewRepository
{
    public ReviewRepository(GameItDbContext context) : base(context) { }
}
