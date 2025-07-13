using MediatR;

namespace GameIt.Application.Features.Review.Queries.GetReviewsByGame;

public record GetReviewsListQuery(Guid Id) : IRequest<ReviewListDto>;
