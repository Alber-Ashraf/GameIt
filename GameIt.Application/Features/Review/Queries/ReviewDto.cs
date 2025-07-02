using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameIt.Application.Features.Review.Queries
{
    public class ReviewDto
    {
        public int Rating { get; set; }
        public string Comment { get; set; }

        public string UserDisplayName { get; set; }
        public string UserProfilePictureUrl { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
