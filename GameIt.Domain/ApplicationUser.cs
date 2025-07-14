using Microsoft.AspNetCore.Identity;

namespace GameIt.Domain;

public class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; }
    public string ProfilePictureUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginDate { get; set; }

    // Navigation Properties
    public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
    public ICollection<Game> OwnedGames => Purchases.Select(p => p.Game).ToList();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
