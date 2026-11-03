using GameIt.Domain;

namespace GameIt.Application.Models.Stripe;

public class ConfirmPaymentResult
{
    public bool IsSuccess { get; set; }
    public Guid PurchaseId { get; set; }
    public Guid GameId { get; set; }
    public string GameName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public PaymentStatus Status { get; set; }
}
