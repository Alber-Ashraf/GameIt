using GameIt.Application.Models.Stripe;
using GameIt.Domain;

namespace GameIt.Application.Interfaces.Stripe;

public interface IStripeService
{
    Task<Purchase> CreatePurchaseAsync(decimal amount, string userId, Guid gameId, CancellationToken token = default);
    Task<RefundResult> RefundPurchaseAsync(string paymentIntentId, CancellationToken token = default);
}
