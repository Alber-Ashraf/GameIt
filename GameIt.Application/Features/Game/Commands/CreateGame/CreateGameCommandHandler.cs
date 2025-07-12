using AutoMapper;
using GameIt.Application.Exeptions;
using GameIt.Application.Interfaces.Persistence;
using MediatR;

namespace GameIt.Application.Features.Game.Commands.CreateGame
{
    public class CreateGameCommandHandler : IRequestHandler<CreateGameCommand, Guid>
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
            // Validate the request 
            var validator = new CreateGameCommandValidator(_unitOfWork);
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
                throw new BadRequestException("Invalid Game", validationResult);

            // Get the category by name from the repository
            var category = await _unitOfWork.Categories
                .GetByNameAsync(request.CategoryName);

            // Map the CreateGameCommand to a Game entity
            var gameToCreate = _mapper.Map<Domain.Game>(request);
            gameToCreate.CategoryId = category.Id;
            // Validate if the category exists
            if (category == null)
                throw new NotFoundException(nameof(category), request.CategoryName);

            // Add the game entity to the repository
            await _unitOfWork.Games.CreateAsync(gameToCreate);

            // Save changes to the database
            await _unitOfWork.SaveChangesAsync();

            // Return the ID of the created game
            return gameToCreate.Id;
        }
    }
}
