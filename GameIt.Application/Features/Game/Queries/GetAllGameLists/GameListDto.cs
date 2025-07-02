using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameIt.Application.Features.Game.Queries.GetAllGameLists
{
    public class GameListDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public bool IsFree { get; set; }
        public string CategoryName { get; set; }
    }
}
