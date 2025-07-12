namespace GameIt.Application.Features.Review.Queries;

public class ReviewDto
{
    public Guid Id { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; }

    public string UserDisplayName { get; set; }
    public string UserProfilePictureUrl { get; set; }

    public DateTime CreatedAt { get; set; }
}
