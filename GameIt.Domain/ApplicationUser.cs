using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Identity;

namespace GameIt.Domain
{
    public class ApplicationUser : IdentityUser
    {
        public string DisplayName { get; set; }
        public string ProfilePictureUrl { get; set; }

        // Navigation Properties
        public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
    }
}
