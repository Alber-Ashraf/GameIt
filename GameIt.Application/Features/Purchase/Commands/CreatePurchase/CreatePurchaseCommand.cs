using GameIt.Application.Models.Stripe;
using MediatR;

namespace GameIt.Application.Features.Purchase.Commands.CreatePurchase;

public class CreatePurchaseCommand : IRequest<PurchaseResult>
{
    public Guid GameId { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal OriginalPrice { get; set; }
    public string Currency { get; set; } = "USD";
    public string TransactionId { get; set; }
    public string UserId { get; set; }
}
