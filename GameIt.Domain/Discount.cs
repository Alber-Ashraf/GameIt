using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameIt.Domain.Common;

namespace GameIt.Domain
{
    public class Discount : BaseEntity
    {
        [Required]
        public Guid GameId { get; set; }
        public Game Game { get; set; }

        [Required]
        [Range(1, 100)]
        public int Percentage { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        [MaxLength(200)]
        public string Description { get; set; }
    }
}
