using GameIt.Domain.Common;

namespace GameIt.Domain
{
    public class Comment : BaseEntity
    {
        public string Content { get; set; }

        // Relationships
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public Guid GameId { get; set; }
        public Game Game { get; set; }
    }
}
