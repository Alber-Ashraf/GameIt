using MediatR;

namespace GameIt.Application.Features.Purchase.Commands.RefundPurchase;

public class RefundPurchaseCommand : IRequest<RefundResponse>
{
    public Guid PurchaseId { get; set; }
    public string AdminUserId { get; set; }
    public string Reason { get; set; }
}

public class RefundResponse
{
    public bool Success { get; set; }
    public decimal AmountRefunded { get; set; }
    public string Currency { get; set; }
    public DateTime RefundDate { get; set; }
    public string RefundId { get; set; }
}
