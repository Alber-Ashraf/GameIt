using GameIt.Application.Exeptions;
using GameIt.Application.Interfaces.Persistence;
using MediatR;

namespace GameIt.Application.Features.Game.Commands.DeleteGame
{
    public class DeleteGameCommandHandler : IRequestHandler<DeleteGameCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        public DeleteGameCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Unit> Handle(DeleteGameCommand request, CancellationToken cancellationToken)
        {
            // Get existing game from DB
            var existingGame = await _unitOfWork.Games.GetByIdAsync(request.Id);

            // Validate if the game exists
            if (existingGame == null)
                throw new NotFoundException(nameof(Game), request.Id);

            // Delete the game entity from the repository
            await _unitOfWork.Games.DeleteAsync(existingGame);

            // Save changes to the database
            await _unitOfWork.SaveChangesAsync();

            // Return Unit.Value to indicate successful completion
            return Unit.Value;
        }
    }
}
