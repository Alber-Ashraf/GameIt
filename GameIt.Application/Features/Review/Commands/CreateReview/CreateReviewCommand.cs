using MediatR;

namespace GameIt.Application.Features.Review.Commands.CreateReview;

public class CreateReviewCommand : IRequest<Guid>
{
    public int Rating { get; set; }
    public string Comment { get; set; }
    public Guid GameId { get; set; } 
}
