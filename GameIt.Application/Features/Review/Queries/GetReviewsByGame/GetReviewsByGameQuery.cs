using MediatR;

namespace GameIt.Application.Features.Review.Queries.GetReviewsByGame;

public record GetReviewsByGameQuery(Guid GameId) : IRequest<List<ReviewListDto>>;
