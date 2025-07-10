using GameIt.Application.Interfaces.Persistence;
using GameIt.Domain;
using GameIt.Persistence.DatabaseContext;

namespace GameIt.Persistence.Repositories;

public class PublisherRepository : GenericRepository<Publisher>, IPublisherRepository
{
    public PublisherRepository(GameItDbContext context) : base(context) { }
}
