namespace GameIt.Application.Features.Wishlist.Queries.GetUserWishlist;

public class WishlistListDto
{
    public Guid GameId { get; set; }
    public string GameName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public bool IsFree { get; set; }
    public string? ImageUrl { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public DateTime? AddedDate { get; set; }
}
