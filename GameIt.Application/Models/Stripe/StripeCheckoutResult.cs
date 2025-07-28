namespace GameIt.Application.Models.Stripe;

public class StripeCheckoutResult
{
    public string CheckoutUrl { get; set; }
    public string SessionId { get; set; }
    public string? StripePaymentIntentId { get; set; }
}
