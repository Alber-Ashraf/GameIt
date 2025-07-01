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
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        [MaxLength(300)]
        public string ImageUrl { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public bool IsFree { get; set; } = false;
        public bool IsFeatured { get; set; } = false;

        [Required]
        [MaxLength(100)]
        public string Size { get; set; }

        [Required]
        [MaxLength(500)]
        public string DownloadLink { get; set; }

        public DateTime ReleaseDate { get; set; } = DateTime.UtcNow;

        // Relationships
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
        public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
    }
}
