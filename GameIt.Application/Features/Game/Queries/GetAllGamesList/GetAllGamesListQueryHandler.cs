using AutoMapper;
using GameIt.Application.Exeptions;
using GameIt.Application.Interfaces.Persistence;
using MediatR;

namespace GameIt.Application.Features.Game.Queries.GetAllGameLists;

public class GetAllGamesListQueryHandler : IRequestHandler<GetAllGamesListQuery, List<GamesListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public GetAllGamesListQueryHandler(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<GamesListDto>> Handle(GetAllGamesListQuery request, CancellationToken cancellationToken)
    {
        // Query the database for all games Lists
        var games = await _unitOfWork.Games.GetAllWithCategoryAsync();

        // Validate if any games exist
        if (games == null || !games.Any())
            throw new NotFoundException(nameof(games), "No games found");

        // Convert the game entities to GameDetailsDto using AutoMapper
        var data = _mapper.Map<List<GamesListDto>>(games);

        // Return the list of GameDetailsDto
        return data;
    }
}
