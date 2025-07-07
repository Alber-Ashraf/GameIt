using GameIt.Domain.Common;

namespace GameIt.Domain
{
    public class Publisher : BaseEntity
    {
        public string Name { get; set; }
        public string LogoUrl { get; set; }

        // Navigation
        public ICollection<Game> Games { get; set; } = new List<Game>();
    }
}
