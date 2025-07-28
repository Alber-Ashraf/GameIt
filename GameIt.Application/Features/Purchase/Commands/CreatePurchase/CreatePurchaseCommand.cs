using GameIt.Application.Models.Stripe;
using MediatR;

namespace GameIt.Application.Features.Purchase.Commands.CreatePurchase;

public class CreatePurchaseCommand : IRequest<PurchaseResult>
{
    public Guid GameId { get; set; }
    public decimal AmountPaid { get; set; }
}
