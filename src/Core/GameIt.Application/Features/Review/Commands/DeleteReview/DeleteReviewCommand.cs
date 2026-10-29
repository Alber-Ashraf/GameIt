using MediatR;

namespace GameIt.Application.Features.Review.Commands.DeleteReview;

public class DeleteReviewCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; } 
}