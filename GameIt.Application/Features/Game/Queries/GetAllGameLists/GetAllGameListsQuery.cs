using MediatR;

namespace GameIt.Application.Features.Game.Queries.GetAllGameLists
{
    public class GetAllGameListsQuery : IRequest<List<GameListDto>> {}
}
