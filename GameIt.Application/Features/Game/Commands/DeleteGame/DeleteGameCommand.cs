using MediatR;

namespace GameIt.Application.Features.Game.Commands.DeleteGame
{
    public class DeleteGameCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
