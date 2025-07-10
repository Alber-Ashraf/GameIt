using GameIt.Application.Features.Game.Queries.GetAllGameLists;
using MediatR;

namespace GameIt.Application.Features.Game.Queries.GetSimilarGames;

public class GetSimilarGamesQuery : IRequest<List<GamesListDto>> 
{
    public Guid GameId { get; set; }
    public int Limit { get; set; }
}
