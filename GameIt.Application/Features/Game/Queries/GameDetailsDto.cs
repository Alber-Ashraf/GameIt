using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameIt.Application.Features.Review.Queries;

namespace GameIt.Application.Features.Game.Queries
{
    public class GameDetailsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public bool IsFree { get; set; }
        public bool IsFeatured { get; set; }
        public string Size { get; set; }
        public string DownloadLink { get; set; }
        public DateTime ReleaseDate { get; set; }

        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }

        public double? AverageRating { get; set; }

        public int TotalReviews { get; set; }
        public List<ReviewDto> Reviews { get; set; }
    }
}
