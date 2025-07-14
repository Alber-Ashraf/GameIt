using GameIt.Domain;

namespace GameIt.Application.Interfaces.Persistence;

public interface IDiscountRepository : IGenericRepository<Discount>
{
    Task<List<Discount>> GetActiveDiscountsAsync(CancellationToken token = default);
}
