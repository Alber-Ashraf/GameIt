using GameIt.Application.Interfaces.Stripe;
using GameIt.Application.Models.Stripe;
using GameIt.Domain;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
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

    public async Task<StripeCheckoutResult> CreateCheckoutSessionAsync(decimal amount, string userId, Guid gameId, CancellationToken token = default)
    {
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
        {
            new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(amount * 100), // convert to cents
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "Game Purchase"
                    },
                },
                Quantity = 1,
            },
        },
            Mode = "payment",
            SuccessUrl = $"https://yourdomain.com/purchase-success?session_id={{CHECKOUT_SESSION_ID}}",
            CancelUrl = $"https://yourdomain.com/purchase-cancelled",
            Metadata = new Dictionary<string, string>
        {
            { "user_id", userId },
            { "game_id", gameId.ToString() }
        }
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options);

        return new StripeCheckoutResult
        {
            CheckoutUrl = session.Url,
            SessionId = session.Id,
            StripePaymentIntentId = session.PaymentIntentId
        };
    }
}
