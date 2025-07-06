using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameIt.Domain.Common;

namespace GameIt.Domain
{
    public class Game : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public bool IsFree { get; set; } = false;
        public bool IsFeatured { get; set; } = false;
        public string Size { get; set; }
        public string DownloadLink { get; set; }

        public DateTime ReleaseDate { get; set; } = DateTime.UtcNow;

        // Relationships
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }

        public Discount Discount { get; set; }

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
        public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
    }
}
