using GameIt.Application.Interfaces.Persistence;
using GameIt.Domain;
using GameIt.Persistence.DatabaseContext;

namespace GameIt.Persistence.Repositories;

public class DiscountRepository : GenericRepository<Discount>, IDiscountRepository
{
    public DiscountRepository(GameItDbContext context) : base(context) { }
}
