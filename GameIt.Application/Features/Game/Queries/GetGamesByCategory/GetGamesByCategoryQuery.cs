using GameIt.Application.Features.Game.Queries.GetAllGameLists;
using MediatR;

namespace GameIt.Application.Features.Game.Queries.GetGamesByCategory;

public class GetGamesByCategoryQuery : IRequest<List<GamesListDto>>
{
    public Guid CategoryId { get; set; }
    public int Limit { get; set; }
}
