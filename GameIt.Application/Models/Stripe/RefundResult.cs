namespace GameIt.Application.Models.Stripe;

public class RefundResult
{
    public bool Success { get; set; }
    public string? RefundId { get; set; }
    public string? Error { get; set; }
    public DateTime? RefundDate { get; set; }
    public string Status { get; set; }
}
