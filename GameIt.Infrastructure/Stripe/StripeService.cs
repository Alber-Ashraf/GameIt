using Stripe;
using Stripe.Checkout;
using GameIt.Application.Interfaces.Stripe;
using GameIt.Application.Models.Stripe;
using Microsoft.Extensions.Configuration;

namespace GameIt.Infrastructure.Stripe
{
    public class StripeService : IStripeService
    {
        private readonly StripeClient _stripeClient;
        private readonly string _webhookSecret;
        private readonly string _successUrl;
        private readonly string _cancelUrl;

        public StripeService(IConfiguration config)
        {
            var stripeConfig = config.GetSection("Stripe");
            if (stripeConfig == null)
                throw new ArgumentNullException("Stripe configuration not found");

            var secretKey = stripeConfig["SecretKey"];
            if (string.IsNullOrWhiteSpace(secretKey))
                throw new ArgumentException("Stripe SecretKey is missing from configuration");

            _stripeClient = new StripeClient(secretKey);
            _webhookSecret = stripeConfig["WebhookSecret"];
            _successUrl = stripeConfig["SuccessUrl"] ?? "https://yourdomain.com/success";
            _cancelUrl = stripeConfig["CancelUrl"] ?? "https://yourdomain.com/cancel";

            StripeConfiguration.ApiKey = secretKey; // Needed for static API access
        }

        public async Task<CheckoutSessionResponse> CreateCheckoutSessionAsync(
    decimal amount,
    string userId,
    Guid gameId,
    Guid purchaseId,
    CancellationToken token)
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
        {
            new()
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(amount * 100),
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = $"Game Purchase: {gameId}"
                    }
                },
                Quantity = 1
            }
        },
                Mode = "payment",
                SuccessUrl = $"{_successUrl}?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = _cancelUrl,
                Metadata = new Dictionary<string, string>
        {
            { "userId", userId },
            { "gameId", gameId.ToString() },
            { "purchase_id", purchaseId.ToString() }
        }
            };

            var service = new SessionService(_stripeClient);
            var session = await service.CreateAsync(options, cancellationToken: token);

            return new CheckoutSessionResponse
            {
                CheckoutUrl = session.Url,
                StripePaymentIntentId = session.PaymentIntentId
            };
        }


        public Task<Event> ConstructWebhookEventAsync(string jsonPayload, string signatureHeader)
        {
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    jsonPayload,
                    signatureHeader,
                    _webhookSecret,
                    tolerance: 300 // 5 minutes
                );

                return Task.FromResult(stripeEvent);
            }
            catch (StripeException ex)
            {
                throw new ApplicationException("Stripe webhook validation failed", ex);
            }
        }
    }
}
