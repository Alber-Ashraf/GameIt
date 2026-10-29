using GameIt.Domain.Common;

namespace GameIt.Domain;

public class Library : BaseEntity
{
    public string UserId { get; set; } = null!;
    public Guid GameId { get; set; }

    public DateTime PurchasedAt { get; set; }

    // Navigation properties
    public Game Game { get; set; } = null!;
}
