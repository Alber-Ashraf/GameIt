using MediatR;

namespace GameIt.Application.Features.Game.Queries.GetGameDetails;

public record GetGameDetailsQuery(Guid Id) : IRequest<GameDetailsDto>;
