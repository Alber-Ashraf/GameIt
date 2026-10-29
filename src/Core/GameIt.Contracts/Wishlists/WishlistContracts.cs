namespace GameIt.Contracts.Wishlists;

public class WishlistItemDto
{
    public string UserId { get; set; } = string.Empty;
    public Guid GameId { get; set; }
    public string GameName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsFree { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public decimal? DiscountPercentage { get; set; }

    public decimal FinalPrice => IsFree
        ? 0m
        : (DiscountPercentage is > 0m and <= 100m
            ? Math.Round(Price * (1m - (DiscountPercentage.Value / 100m)), 2)
            : Price);
}

public class AddToWishlistRequestDto
{
    public Guid GameId { get; set; }
}
