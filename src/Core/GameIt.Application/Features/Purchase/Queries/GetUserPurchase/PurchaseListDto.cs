using GameIt.Domain;

namespace GameIt.Application.Features.Purchase.Queries.GetUserPurchase;

public class PurchaseListDto
{
    public Guid Id { get; set; }
    public Guid GameId { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public string? StripePaymentIntentId => TransactionId;
    public decimal AmountPaid { get; set; }
    public decimal OriginalPrice { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime PurchaseDate { get; set; }
    public string GameName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public PaymentStatus Status { get; set; }
    public bool IsRefunded { get; set; } = false;
}
