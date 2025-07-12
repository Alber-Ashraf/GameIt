using GameIt.Application.Features.Game.Queries.GetAllGameLists;
using MediatR;

namespace GameIt.Application.Features.Game.Queries.GetSimilarGames;

public record GetSimilarGamesQuery(Guid GameId, int Limit = 5) 
    : IRequest<List<GamesListDto>>;
