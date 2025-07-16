using GameIt.Application.Interfaces.Stripe;
using GameIt.Application.Models.Stripe;
using GameIt.Domain;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.V2;

namespace GameIt.Infrastructure.Stripe;

public class StripeService : IStripeService
{
    private readonly StripeSettings _stripeSettings;

    public StripeService(IOptions<StripeSettings> stripeSettings)
    {
        _stripeSettings = stripeSettings.Value;
        StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
    }
    public async Task<Purchase> CreatePurchaseAsync(decimal amount, string userId, Guid gameId, CancellationToken token = default)
    {
        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)(amount * 100),
            Currency = "usd",
            Metadata = new Dictionary<string, string>
        {
            { "user_id", userId },
            { "game_id", gameId.ToString() }
        }
        };

        var service = new PaymentIntentService();
        var paymentIntent = await service.CreateAsync(options, cancellationToken: token);

        return new Purchase
        {
            UserId = userId,
            GameId = gameId,
            AmountPaid = amount,
            Currency = "usd",
            StripePaymentIntentId = paymentIntent.Id,
            Status = PaymentStatus.Pending
        };
    }

    public async Task<RefundResult> RefundPurchaseAsync(string paymentIntentId, CancellationToken token)
    {
        try
        {
            var options = new RefundCreateOptions
            {
                PaymentIntent = paymentIntentId
            };

            var refund = await new RefundService().CreateAsync(options, cancellationToken: token);
            return new RefundResult { Success = true, RefundId = refund.Id };
        }
        catch (StripeException ex)
        {
            return new RefundResult { Success = false, Error = ex.Message };
        }
    }
}
