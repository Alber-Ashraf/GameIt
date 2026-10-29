    using AutoMapper;
using GameIt.Application.Exeptions;
using GameIt.Application.Interfaces.Persistence;
using MediatR;

namespace GameIt.Application.Features.Game.Commands.UpdateGame;

public class UpdateGameCommandhandler : IRequestHandler<UpdateGameCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public UpdateGameCommandhandler(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<Unit> Handle(UpdateGameCommand request, CancellationToken cancellationToken)
    {
        // Validate the request
        var validator = new UpdateGameCommandValidator(_unitOfWork);
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
            throw new BadRequestException("Invalid Game", validationResult);

        // Get existing game from DB
        var existingGame = await _unitOfWork.Games.GetByIdAsync(request.Id);
        // Validate if the game exists
        if (existingGame == null)
            throw new NotFoundException(nameof(Game), request.Id);

        // Get the category by name from the repository
        var category = await _unitOfWork.Categories
            .GetByIdAsync(request.CategoryId);
        // Validate if the category exists
        if (category == null)
            throw new NotFoundException(nameof(category), request.CategoryId);

        // Map the CreateGameCommand to a Game entity
        _mapper.Map(request, existingGame);
        existingGame.CategoryId = category.Id;

        // Add the game entity to the repository
        _unitOfWork.Games.Update(existingGame);

        // Save changes to the database
        await _unitOfWork.SaveChangesAsync();

        // Return the ID of the created game
        return Unit.Value;
    }
}
