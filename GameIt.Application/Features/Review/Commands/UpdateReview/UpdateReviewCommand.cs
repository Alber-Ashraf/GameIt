using MediatR;

namespace GameIt.Application.Features.Review.Commands.UpdateReview;

public class UpdateReviewCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public int? Rating { get; set; }
    public string? Comment { get; set; }
    public Guid GameId { get; set; }
    public Guid UserId { get; set; }
}
