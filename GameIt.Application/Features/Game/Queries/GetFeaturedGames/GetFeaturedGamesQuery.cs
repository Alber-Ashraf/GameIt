using GameIt.Application.Features.Game.Queries.GetAllGameLists;
using MediatR;

namespace GameIt.Application.Features.Game.Queries.GetFeaturedGames;

public class GetFeaturedGamesQuery : IRequest<List<GamesListDto>> 
{
    public int Limit { get; set; }
}
