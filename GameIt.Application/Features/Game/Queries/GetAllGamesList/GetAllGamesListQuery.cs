using MediatR;

namespace GameIt.Application.Features.Game.Queries.GetAllGameLists
{
    public class GetAllGamesListQuery : IRequest<List<GamesListDto>> {}
}
