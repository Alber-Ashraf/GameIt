using GameIt.Domain;

namespace GameIt.Application.Models.Stripe;

public class PurchaseResult
{
    public Guid Id { get; set; }
    public PaymentStatus Status { get; set; }
    public DateTime PurchaseDate { get; set; }
}
