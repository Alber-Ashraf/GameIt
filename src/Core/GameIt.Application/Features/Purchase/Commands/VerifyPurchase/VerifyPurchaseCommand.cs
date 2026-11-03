using GameIt.Application.Models.Stripe;
using MediatR;

namespace GameIt.Application.Features.Purchase.Commands.VerifyPurchase;

public class VerifyPurchaseCommand : IRequest<ConfirmPaymentResult>
{
    public Guid PurchaseId { get; set; }

    public VerifyPurchaseCommand()
    {
    }

    public VerifyPurchaseCommand(Guid purchaseId)
    {
        PurchaseId = purchaseId;
    }
}
