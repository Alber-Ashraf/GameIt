using GameIt.Application.Features.Game.Queries.GetAllGameLists;
using MediatR;

namespace GameIt.Application.Features.Game.Queries.GetGamesByCategory;

public record GetGamesByCategoryQuery(Guid CategoryId, int Limit = 10)
    : IRequest<List<GamesListDto>>;
