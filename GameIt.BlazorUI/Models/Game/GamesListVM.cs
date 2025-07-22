namespace GameIt.BlazorUI.Models.Game;

public class GamesListVM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsFree { get; set; }
    public bool IsOnSale { get; set; }
    public int? Discount { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public double? AverageRating { get; set; }
}
