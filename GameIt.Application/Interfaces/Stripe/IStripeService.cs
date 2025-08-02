using Stripe;
using GameIt.Application.Models.Stripe;

namespace GameIt.Application.Interfaces.Stripe
{
    public interface IStripeService
    {
        Task<CheckoutSessionResponse> CreateCheckoutSessionAsync(
            decimal amount,
            string userId,
            Guid gameId,
            Guid purchaseId,
            CancellationToken token);
        Task<Event> ConstructWebhookEventAsync(string jsonPayload, string signatureHeader);
    }
}
