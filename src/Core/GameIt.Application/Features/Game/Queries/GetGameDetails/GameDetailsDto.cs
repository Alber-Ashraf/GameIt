using GameIt.Application.Features.Review.Queries.GetReviewsByGame;

namespace GameIt.Application.Features.Game.Queries.GetGameDetails;

public class GameDetailsDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsFree { get; set; }
    public long FileSizeInBytes { get; set; }
    public string DownloadLink { get; set; } = string.Empty;
    public string SystemRequirements { get; set; } = string.Empty;
    public DateTime ReleaseDate { get; set; }

    // Category details
    public string CategoryName { get; set; } = string.Empty;

    // Publisher details
    public string PublisherName { get; set; } = string.Empty;

    // Discount details
    public bool? IsDiscounted { get; set; } = false;  
    public decimal? DiscountPercentage { get; set; }
    public DateTime? DiscountStartDate { get; set; }
    public DateTime? DiscountEndDate { get; set; }
    public decimal? DiscountedPrice => Price * ((100 - (DiscountPercentage ?? 0)) / 100);
    public int? RemainingDiscountDays =>
        DiscountEndDate.HasValue ? (int)(DiscountEndDate.Value - DateTime.UtcNow).TotalDays : null;

    // Reviews
    public double? AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public List<ReviewListDto> Reviews { get; set; } = new();
}
