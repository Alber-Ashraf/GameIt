using MediatR;

namespace GameIt.Application.Features.Game.Queries.GetAllGameDetails;

public record GetGameDetailsQuery(Guid Id) : IRequest<GameDetailsDto>;
