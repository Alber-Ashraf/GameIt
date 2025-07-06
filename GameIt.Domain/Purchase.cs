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
    public class Purchase : BaseEntity
    {
        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;

        // Relationships
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public Guid GameId { get; set; }
        public Game Game { get; set; }
    }
}
