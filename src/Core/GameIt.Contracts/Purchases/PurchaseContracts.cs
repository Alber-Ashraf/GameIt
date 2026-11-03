namespace GameIt.Contracts.Purchases;

public enum PaymentStatusContract
{
    Pending = 0,
    Succeeded = 1,
    Failed = 2
}

public class PurchaseItemDto
{
    public Guid Id { get; set; }
    public Guid GameId { get; set; }
    public string GameName { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTime PurchaseDate { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal OriginalPrice { get; set; }
    public string? StripePaymentIntentId { get; set; }
    public PaymentStatusContract Status { get; set; }
}

public class CreatePurchaseRequestDto
{
    public Guid GameId { get; set; }
    public decimal AmountPaid { get; set; }
}

public class PurchaseResultDto
{
    public Guid Id { get; set; }
    public PaymentStatusContract Status { get; set; }
    public string? StripeCheckoutUrl { get; set; }
    public DateTime PurchaseDate { get; set; }
}

public class ConfirmPaymentResultDto
{
    public bool IsSuccess { get; set; }
    public Guid PurchaseId { get; set; }
    public Guid GameId { get; set; }
    public string GameName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public PaymentStatusContract Status { get; set; }
}
