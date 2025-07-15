namespace GameIt.Application.Features.Purchase.Queries.GetUserPurchase;

public class PurchaseListDto
{
    public Guid Id { get; set; }
    public string TransactionId { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal OriginalPrice { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime PurchaseDate { get; set; }
    public string GameName { get; set; }
    public string UserName { get; set; }
    public bool IsRefunded { get; set; } = false;
}
