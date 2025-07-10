using GameIt.Application.Interfaces.Persistence;
using GameIt.Domain;
using GameIt.Persistence.DatabaseContext;

namespace GameIt.Persistence.Repositories;

public class PurchaseRepository : GenericRepository<Purchase>, IPurchaseRepository
{
    public PurchaseRepository(GameItDbContext context) : base(context) { }
}
