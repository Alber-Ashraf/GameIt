using GameIt.Domain.Common;

namespace GameIt.Domain;

public class Game : BaseEntity
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public decimal? Price { get; set; }
    public bool IsFree { get; set; } = false;
    public bool IsFeatured { get; set; } = false;
    public long? FileSizeInBytes { get; set; }
    public string? DownloadLink { get; set; }
    public string? SystemRequirements { get; set; }

    public DateTime ReleaseDate { get; set; } = DateTime.UtcNow;

    // Relationships
    public Guid CategoryId { get; set; }
    public Category Category { get; set; }

    public string? PublisherId { get; set; }

    public Discount? Discount { get; set; }
    public double AverageRating { get; private set; }
    public int TotalReviews { get; private set; }

    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
    public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
    public ICollection<Library> Libraries { get; set; } = new List<Library>();
}
