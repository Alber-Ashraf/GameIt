using GameIt.Domain;
using MediatR;

namespace GameIt.Application.Features.Purchase.Commands.CreatePurchase;

public class CreatePurchaseCommand : IRequest<PurchaseResponse>
{
    public Guid GameId { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal OriginalPrice { get; set; }
    public string Currency { get; set; } = "USD";
    public PaymentMethod PaymentMethod { get; set; }
    public string TransactionId { get; set; }
    public string UserId { get; set; }
}

public class PurchaseResponse
{
    public Guid Id { get; set; }
    public string TransactionId { get; set; }
    public PaymentStatus Status { get; set; }
    public DateTime PurchaseDate { get; set; }
}
