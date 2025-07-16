using GameIt.Application.Models.Stripe;
using MediatR;

namespace GameIt.Application.Features.Purchase.Commands.RefundPurchase;

public class RefundPurchaseCommand : IRequest<RefundResult>
{
    public string PaymentIntentId { get; set; }
    public Guid PurchaseId { get; set; }
    public string AdminUserId { get; set; }
    public string Reason { get; set; }
}
