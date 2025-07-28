using GameIt.Application.Models.Stripe;
using GameIt.Domain;

namespace GameIt.Application.Interfaces.Stripe;

public interface IStripeService
{
    Task<StripeCheckoutResult> CreateCheckoutSessionAsync(decimal amount, string userId, Guid gameId, CancellationToken token = default);
}
