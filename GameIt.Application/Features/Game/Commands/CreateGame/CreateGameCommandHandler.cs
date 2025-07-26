using AutoMapper;
using GameIt.Application.Exeptions;
using GameIt.Application.Interfaces.Persistence;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace GameIt.Application.Features.Game.Commands.CreateGame;

public class CreateGameCommandHandler : IRequestHandler<CreateGameCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _contextAccessor;
    public CreateGameCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _contextAccessor = contextAccessor;
    }
    public async Task<Guid> Handle(CreateGameCommand request, CancellationToken cancellationToken)
    {
        // Validate the request 
        var validator = new CreateGameCommandValidator(_unitOfWork);
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
            throw new BadRequestException("Invalid Game", validationResult);

        if (request.IsFree && request.Price != 0)
            throw new ArgumentException("Price must be 0 for free games.");

        // Get the category by name from the repository
        var category = await _unitOfWork.Categories
            .GetByIdAsync(request.CategoryId);

        // Map the CreateGameCommand to a Game entity
        var gameToCreate = _mapper.Map<Domain.Game>(request);
        gameToCreate.CategoryId = category.Id;

        // Get the publisher ID from the HTTP context
        string? publisherId = _contextAccessor.HttpContext?.User?.FindFirstValue("uid");

        if (string.IsNullOrEmpty(publisherId))
            throw new UnauthorizedAccessException("User must be authenticated");

        // Set the publisher ID
        gameToCreate.PublisherId = publisherId;

        // Validate if the category exists
        if (category == null)
            throw new NotFoundException(nameof(category), request);

        // Add the game entity to the repository
        await _unitOfWork.Games.CreateAsync(gameToCreate);

        // Save changes to the database
        await _unitOfWork.SaveChangesAsync();

        // Return the ID of the created game
        return gameToCreate.Id;
    }
}
