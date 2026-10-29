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

        var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId)
            ?? throw new NotFoundException(nameof(Domain.Category), request.CategoryId);

        var gameToCreate = _mapper.Map<Domain.Game>(request);
        gameToCreate.CategoryId = category.Id;

        string? publisherId = _contextAccessor.HttpContext?.User?.FindFirstValue("uid");

        if (string.IsNullOrEmpty(publisherId))
            throw new UnauthorizedAccessException("User must be authenticated");

        gameToCreate.PublisherId = publisherId;

        // Add the game entity to the repository
        await _unitOfWork.Games.CreateAsync(gameToCreate);

        // Save changes to the database
        await _unitOfWork.SaveChangesAsync();

        // Return the ID of the created game
        return gameToCreate.Id;
    }
}
