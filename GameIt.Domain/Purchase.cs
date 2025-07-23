using GameIt.Domain.Common;

namespace GameIt.Domain;

public class Purchase : BaseEntity
{
    public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
    public decimal AmountPaid { get; set; }
    public decimal? OriginalPrice { get; set; }
    public string Currency { get; set; } = "USD";

    // Stripe Essentials
    public string StripePaymentIntentId { get; set; }

    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    // Refunds
    public string? RefundId { get; set; }
    public bool IsRefunded { get; set; } = false;
    public DateTime? RefundDate { get; set; }

    // Relationships
    public string UserId { get; set; }

    public Guid GameId { get; set; }
    public Game Game { get; set; }
}
public enum PaymentStatus
{
    Pending,
    Succeeded,
    Failed,
    Refunded
}
