using AutoMapper;
using GameIt.Application.Exeptions;
using GameIt.Application.Features.Game.Queries.GetAllGameLists;
using GameIt.Application.Features.Game.Queries.GetFeaturedGames;
using GameIt.Application.Interfaces.Persistence;

namespace GameIt.Application.Features.Game.Queries.GetGamesByCategory;

public class GetGamesByCategoryQueryHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public GetGamesByCategoryQueryHandler(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<List<GamesListDto>> Handle(GetGamesByCategoryQuery request, CancellationToken cancellationToken)
    {
        // Query the database for all featured games
        var games = await _unitOfWork.Games.GetGamesByCategoryAsync(request.CategoryId,
                    request.Limit,
                    cancellationToken);

        // Validate if the games exist
        if (games == null || !games.Any())
            throw new NotFoundException(nameof(games), $"No Games Found in this category: {request.CategoryId}");

        // Convert the game entities to GameDetailsDto using AutoMapper
        var data = _mapper.Map<List<GamesListDto>>(games);
        // Return the list of GameDetailsDto
        return data;
    }
}
