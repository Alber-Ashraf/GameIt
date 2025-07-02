using MediatR;

namespace GameIt.Application.Features.Game.Queries.GetAllGameDetails
{
    public class GetAllGameDetailsQuery : IRequest<List<GameDetailsDto>> {}
}
