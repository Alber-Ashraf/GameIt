using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameIt.Domain.Common;

namespace GameIt.Domain
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }

        // Navigation
        public ICollection<Game> Games { get; set; } = new List<Game>();
    }
}
