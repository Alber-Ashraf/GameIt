using AutoMapper;
using GameIt.Application.Exeptions;
using GameIt.Application.Features.Game.Queries.GetAllGameLists;
using GameIt.Application.Interfaces.Persistence;
using MediatR;

namespace GameIt.Application.Features.Game.Queries.GetSimilarGames;

public class GetSimilarGamesQueryHandler : IRequestHandler<GetSimilarGamesQuery, List<GamesListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public GetSimilarGamesQueryHandler(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<List<GamesListDto>> Handle(GetSimilarGamesQuery request, CancellationToken cancellationToken)
    {
        // Query the database for similar games
        var games = await _unitOfWork.Games.GetSimilarGamesAsync(request.GameId,
                    request.Limit,
                    cancellationToken);

        // Validate if any similar games were found
        if (games == null || !games.Any())
            throw new NotFoundException(nameof(games), "No similar games found for the specified game ID.");

        // Convert the game entities to GameDetailsDto using AutoMapper
        var data = _mapper.Map<List<GamesListDto>>(games);
        // Return the list of GameDetailsDto
        return data;    
    }
}
