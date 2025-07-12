using GameIt.Domain.Common;

namespace GameIt.Domain;

public class Category : BaseEntity
{
    public string Name { get; set; }

    // Navigation
    public ICollection<Game> Games { get; set; } = new List<Game>();
}
