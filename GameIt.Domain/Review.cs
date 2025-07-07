using GameIt.Domain.Common;

namespace GameIt.Domain
{
    public class Review : BaseEntity
    {
        public int Rating { get; set; }
        public string Comment { get; set; }

        // Relationships
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public Guid GameId { get; set; }
        public Game Game { get; set; }
    }
}
