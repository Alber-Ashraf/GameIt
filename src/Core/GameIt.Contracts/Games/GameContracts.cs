using System.ComponentModel.DataAnnotations;

namespace GameIt.Contracts.Games;

public class GameSummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsFree { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public DateTime ReleaseDate { get; set; }
    public bool IsFeatured { get; set; }
    public long FileSizeInMB { get; set; }
    public string SystemRequirements { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public double? AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public decimal? Discount { get; set; }

    public decimal FinalPrice => IsFree
        ? 0m
        : (Discount is > 0m and <= 100m
            ? Math.Round(Price * (1m - (Discount.Value / 100m)), 2)
            : Price);
}

public class GameDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsFree { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public DateTime ReleaseDate { get; set; }
    public bool IsFeatured { get; set; }
    public long FileSizeInMB { get; set; }
    public string DownloadLink { get; set; } = string.Empty;
    public string? AdditionalDownloadLink { get; set; }
    public string SystemRequirements { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string PublisherId { get; set; } = string.Empty;
    public double? AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public decimal? DiscountPercentage { get; set; }

    public decimal FinalPrice => IsFree
        ? 0m
        : (DiscountPercentage is > 0m and <= 100m
            ? Math.Round(Price * (1m - (DiscountPercentage.Value / 100m)), 2)
            : Price);
}

public class UpsertGameRequestDto
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Game title is required.")]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required.")]
    [StringLength(1000, MinimumLength = 10)]
    public string Description { get; set; } = string.Empty;

    [Range(0, 999.99, ErrorMessage = "Price must be between $0.00 and $999.99.")]
    public decimal Price { get; set; }

    public bool IsFree { get; set; }

    [Required(ErrorMessage = "Cover image URL is required.")]
    [StringLength(300)]
    public string ImageUrl { get; set; } = string.Empty;

    public DateTime ReleaseDate { get; set; } = DateTime.UtcNow.AddDays(-1);

    public bool IsFeatured { get; set; }

    [Range(1, long.MaxValue, ErrorMessage = "File size in MB must be greater than 0.")]
    public long FileSizeInMB { get; set; } = 512;

    [Required(ErrorMessage = "Download link is required.")]
    [StringLength(500)]
    public string DownloadLink { get; set; } = string.Empty;

    [StringLength(500)]
    public string? AdditionalDownloadLink { get; set; }

    [Required(ErrorMessage = "System requirements are required.")]
    [StringLength(1000, MinimumLength = 10)]
    public string SystemRequirements { get; set; } = string.Empty;

    [Required(ErrorMessage = "Category is required.")]
    public Guid CategoryId { get; set; }

    [Required(ErrorMessage = "Publisher ID is required.")]
    public string PublisherId { get; set; } = "GameIt Studios";
}
