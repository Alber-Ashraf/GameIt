using GameIt.Domain;

namespace GameIt.Application.Interfaces.Persistence;

public interface IPurchaseRepository : IGenericRepository<Purchase>
{
    Task<List<Purchase>> GetUserPurchasesAsync(string userId, CancellationToken cancellationToken);
    Task<Purchase?> GetByPaymentIntentIdAsync(string paymentIntentId);
}
