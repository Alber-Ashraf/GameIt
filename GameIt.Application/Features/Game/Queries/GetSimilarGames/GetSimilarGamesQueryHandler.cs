using AutoMapper;
using GameIt.Application.Features.Game.Queries.GetAllGameLists;
using GameIt.Application.Interfaces.Persistence;

namespace GameIt.Application.Features.Game.Queries.GetSimilarGames;

public class GetSimilarGamesQueryHandler
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
        // Convert the game entities to GameDetailsDto using AutoMapper
        var data = _mapper.Map<List<GamesListDto>>(games);
        // Return the list of GameDetailsDto
        return data;    
    }
}
