using AutoMapper;
using GameIt.Application.Interfaces.Persistence;
using MediatR;

namespace GameIt.Application.Features.Game.Commands.CreateGame
{
    class CreateGameCommandHandler : IRequestHandler<CreateGameCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CreateGameCommandHandler(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Guid> Handle(CreateGameCommand request, CancellationToken cancellationToken)
        {
            // Validate the request (you can add validation logic here if needed)


            var category = await _unitOfWork.Categories
                .GetByNameAsync(request.CategoryName);

            // Map the CreateGameCommand to a Game entity
            var gameToCreate = _mapper.Map<Domain.Game>(request);
            gameToCreate.CategoryId = category.Id;

            // Add the game entity to the repository
            await _unitOfWork.Games.AddAsync(gameToCreate);

            // Save changes to the database
            await _unitOfWork.SaveChangesAsync();

            // Return the ID of the created game
            return gameToCreate.Id;
        }
    }
}
