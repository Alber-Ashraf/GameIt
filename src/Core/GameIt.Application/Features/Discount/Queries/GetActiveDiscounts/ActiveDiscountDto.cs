namespace GameIt.Application.Features.Discount.Queries.GetActiveDiscounts;

public class ActiveDiscountDto
{
    public Guid Id { get; set; }
    public string GameName { get; set; } = string.Empty;
    public string GameImageUrl { get; set; } = string.Empty;
    public decimal OriginalPrice { get; set; }
    public decimal DiscountedPrice { get; set; }
    public decimal Percentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public int DaysRemaining => (EndDate - DateTime.UtcNow).Days;  // Computed  

}
