namespace GameIt.Application.Features.Wishlist.Queries.GetUserWishlist;

public class WishlistListDto
{
    public Guid GameId { get; set; }
    public decimal? Price { get; set; }
    public string ImageUrl { get; set; }
    public DateTime? AddedDate { get; set; }
}
