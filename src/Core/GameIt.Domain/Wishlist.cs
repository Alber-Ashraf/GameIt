using GameIt.Domain.Common;

namespace GameIt.Domain;

public class Wishlist : BaseEntity
{
    // Relationships
    public string UserId { get; set; }

    public Guid GameId { get; set; }
    public Game Game { get; set; }
}
