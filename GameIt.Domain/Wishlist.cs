using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameIt.Domain.Common;

namespace GameIt.Domain
{
    public class Wishlist : BaseEntity
    {
        // Relationships
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public Guid GameId { get; set; }
        public Game Game { get; set; }
    }
}
